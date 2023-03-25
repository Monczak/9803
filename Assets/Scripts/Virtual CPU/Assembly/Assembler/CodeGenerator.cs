using System;
using System.Collections.Generic;
using System.Text;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class CodeGenerator : LogErrorProducer
    {
        private List<AbstractStatement> statements;
        private SymbolTable symbols;   

        private Dictionary<Type, int> directiveUseCounts;

        private ushort programCounter;

        private byte[] code;
        private bool[] codeMask;

        private Vectors vectors;
        private ushort firstInstructionAddress;
        private bool firstInstructionFound;

        private const int MemorySize = 65536;
        
        public bool HadError { get; private set; }
        private event ErrorHandler OnWarning;
        
        public AssembledCode GenerateCode(List<AbstractStatement> stmts)
        {
            code = new byte[MemorySize];
            codeMask = new bool[MemorySize];

            statements = stmts;
            symbols = new SymbolTable();
            programCounter = 0;

            directiveUseCounts = new Dictionary<Type, int>();

            vectors = new Vectors();

            firstInstructionAddress = 0;
            firstInstructionFound = false;
            
            HadError = false;

            // Pass 1: Find labels
            foreach (AbstractStatement stmt in statements)
            {
                var result = FindLabelInStatement(stmt);
                if (result.Failed) ThrowError((AssemblerError)result.TheError);
            }

            StringBuilder builder = new();
            builder.AppendJoin(" ", symbols);
            MakeLog(builder.ToString());

            // Pass 2: Convert statements to a compiled form
            List<CompiledStatement> compiledStatements = new();
            programCounter = 0;
            foreach (AbstractStatement stmt in statements)
            {
                var result = CompileStatement(stmt);
                if (result.Failed) ThrowError((AssemblerError)result.TheError);

                if (result.Result is not null)
                    compiledStatements.Add(result.Result);
            }

            // Pass 3: Replace label references with label addresses
            programCounter = 0;
            foreach (CompiledStatement cStmt in compiledStatements)
            {
                var result = UpdateLabelRefs(cStmt);
                if (result.Failed) ThrowError((AssemblerError)result.TheError);
            }
            
            // Pass 4: Emit bytes
            if (!HadError)
            {
                foreach (CompiledStatement cStmt in compiledStatements)
                {
                    var result = EmitBytes(cStmt);
                    if (result.Failed) ThrowError((AssemblerError)result.TheError);
                }
            }
            
            // TODO: Warn if code overlaps with vectors or otherwise disallow this

            if (!vectors.ResetSet && firstInstructionFound)
                vectors.Reset = firstInstructionAddress;

            return HadError 
                ? new AssembledCode(null, null, vectors) 
                : new AssembledCode(code, codeMask, vectors);
        }

        private OperationResult EmitBytes(CompiledStatement cStmt)
        {
            ushort pc = cStmt.StartProgramCounter;

            StringBuilder debugStringBuilder = new();

            var bytesResult = cStmt.GetBytes(pc);
            if (bytesResult.Failed)
                return OperationResult.Error((AssemblerError)bytesResult.TheError);
            foreach (byte b in bytesResult.Result)
            {
                if (codeMask[pc])
                {
                    return OperationResult.Error(SyntaxErrors.OverlappingCode(cStmt.Stmt.Tokens[0], pc));
                }
                        
                code[pc] = b;
                codeMask[pc] = true;
                pc++;

                debugStringBuilder.Append($"{b:X2} ");
            }
            MakeLog($"{cStmt.StartProgramCounter:X4} {cStmt}: {debugStringBuilder}");
            return OperationResult.Success();
        }

        private OperationResult UpdateLabelRefs(CompiledStatement cStmt)
        {
            if (cStmt.Operands is not null)
            {
                foreach (Operand op in cStmt.Operands)
                {
                    if (!op.IsDefined)
                    {
                        Label label = symbols.Get<Label>(op.LabelRef);
                        if (!label.IsDeclared)
                            return OperationResult.Error(SyntaxErrors.UseOfUndeclaredLabel(op.Token, label));
                        op.Number = label.Address;
                    }
                }
            }
            
            return OperationResult.Success();
        }

        private void ThrowError(AssemblerError error)
        {
            HadError = true;
            MakeError(error);
        }

        private void ThrowWarning(AssemblerError warning)
        {
            OnWarning?.Invoke(warning);
        }
        
        private OperationResult<CompiledStatement> CompileStatement(AbstractStatement stmt)
        {
            StringBuilder debugMsgBuilder = new StringBuilder($"PC {programCounter}: ");
            CompiledStatement cStmt = null;
            switch (stmt)
            {
                case LabelStatement s:
                {
                    symbols.Get<Label>(s.LabelName).Address = programCounter;
                    break;
                }
                case DirectiveStatement s:
                {
                    var result = CompileDirective(s);
                    if (result.Failed)
                        return OperationResult<CompiledStatement>.Error(result.TheError);
                    cStmt = result.Result;
                    break;
                }
                case InstructionStatement s:
                {
                    var result = CompileInstruction(s);
                    if (result.Failed)
                        return OperationResult<CompiledStatement>.Error(result.TheError);
                    cStmt = result.Result;

                    if (!firstInstructionFound)
                    {
                        firstInstructionAddress = programCounter;
                        firstInstructionFound = true;
                    }
                    
                    break;
                }
            }

            if (cStmt is not null)
            {
                programCounter += cStmt.ByteCount;
                MakeLog(debugMsgBuilder.Append(cStmt).ToString());
            }

            return OperationResult<CompiledStatement>.Success(cStmt);
        }

        private OperationResult<CompiledStatement> CompileInstruction(InstructionStatement stmt)
        {
            CompiledStatement cStmt = stmt switch
            {
                InstructionStatementOperand s => new CompiledStatement(stmt, programCounter, (s.AddressingMode, s.Metadata), s.Operand),
                _ => new CompiledStatement(stmt, programCounter, (stmt.AddressingMode, stmt.Metadata))
            };

            return OperationResult<CompiledStatement>.Success(cStmt);
        }

        private OperationResult<CompiledStatement> CompileDirective(DirectiveStatement stmt)
        {
            var result = stmt.Directive.Build(stmt is DirectiveStatementOperands s ? s.Args : null);
            if (result.Failed)
                return OperationResult<CompiledStatement>.Error((AssemblerError)result.TheError, stmt.Tokens[0]);

            if (!directiveUseCounts.ContainsKey(stmt.Directive.GetType()))
                directiveUseCounts[stmt.Directive.GetType()] = 0;
            
            if (stmt.Directive.Single && directiveUseCounts[stmt.Directive.GetType()] >= 1)
                return OperationResult<CompiledStatement>.Error(
                    SyntaxErrors.DuplicateSingleDirective(stmt.Tokens[0]));

            var evalResult = EvaluateDirective(result.Result);
            if (evalResult.Failed)
                return OperationResult<CompiledStatement>.Error(evalResult.TheError);

            directiveUseCounts[stmt.Directive.GetType()]++;
            
            return OperationResult<CompiledStatement>.Success(new CompiledStatement(stmt, programCounter, null, evalResult.Result));
        }

        private OperationResult<List<Operand>> EvaluateDirective(Directive directive)
        {
            var evalResult = directive.Evaluate(ref programCounter, vectors);
            if (evalResult.Failed)
            {
                return OperationResult<List<Operand>>.Error(evalResult.TheError);
            }

            return OperationResult<List<Operand>>.Success(evalResult.Result);
        }

        private OperationResult FindLabelInStatement(AbstractStatement stmt)
        {
            OperationResult result = TryAddLabel(stmt);
            return result.Failed ? result : OperationResult.Success();
        }

        private OperationResult TryAddLabel(AbstractStatement stmt)
        {
            Label newLabel = stmt switch
            {
                LabelStatement labelStmt => new Label(labelStmt.LabelName, stmt.FileName, null, true),
                InstructionStatementOperand opStmt => new Label(opStmt.Operand.LabelRef, null, null, false),
                _ => null
            };
            if (newLabel is null) return OperationResult.Success();

            bool isDeclaration = stmt is LabelStatement;

            if (newLabel.Name is not null)
            {
                if (symbols.Contains(newLabel.Name))
                {
                    Label label = symbols.Get<Label>(newLabel.Name);
                    if (isDeclaration && label.IsDeclared)
                        return OperationResult.Error(
                            SyntaxErrors
                                .LabelAlreadyDeclared(
                                    stmt.Tokens[0])); // Assuming LabelStatements are created from 1 token
                    
                    label.IsDeclared = true;
                }
                else
                {
                    symbols.Add(newLabel.Name, newLabel);
                }
            }
            return OperationResult.Success();
        }
    }
}