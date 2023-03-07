using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using NineEightOhThree.VirtualCPU.Assembly;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class CodeGenerator : LogErrorProducer
    {
        private List<AbstractStatement> statements;
        private Dictionary<string, Label> labels;

        private ushort programCounter;

        private byte[] code;
        private bool[] codeMask;

        private ushort resetVector;
        private bool resetVectorSet;
        private ushort firstInstructionAddress;
        private bool firstInstructionFound;
        
        private const int MemorySize = 65536;
        
        public bool HadError { get; private set; }
        private event ErrorHandler OnWarning;
        
        public (byte[] code, bool[] mask) GenerateCode(List<AbstractStatement> stmts)
        {
            code = new byte[MemorySize];
            codeMask = new bool[MemorySize];

            statements = stmts;
            labels = new Dictionary<string, Label>();
            programCounter = 0;

            resetVector = 0;
            resetVectorSet = false;

            firstInstructionAddress = 0;
            firstInstructionFound = false;
            
            HadError = false; 

            // Pass 1: Find labels
            foreach (AbstractStatement stmt in statements)
            {
                var result = FindLabel(stmt);
                if (result.Failed) ThrowError((AssemblerError)result.TheError);
            }

            StringBuilder builder = new();
            builder.AppendJoin(" ", labels);
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
            
            // Put reset vector at addresses FFFC and FFFD, like in the real 6502
            // TODO: Warn if code overlaps with reset vector or otherwise disallow this

            if (!resetVectorSet && firstInstructionFound)
                resetVector = firstInstructionAddress;
            
            code[0xFFFC] = (byte)(resetVector & 0xFF);
            code[0xFFFD] = (byte)((resetVector >> 8) & 0xFF);
            codeMask[0xFFFC] = codeMask[0xFFFD] = true;

            return HadError ? (null, null) : (code, codeMask);
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
                        Label label = labels[op.LabelRef];
                        if (!label.IsDeclared)
                            return OperationResult.Error(SyntaxErrors.UseOfUndeclaredLabel(op.Token, label));
                        op.Number = labels[op.LabelRef].Address;
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
                    labels[s.LabelName].Address = programCounter;
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

            var evalResult = EvaluateDirective(result.Result);
            if (evalResult.Failed)
                return OperationResult<CompiledStatement>.Error(evalResult.TheError);
            
            // Special handling for the .begin directive
            if (stmt.Directive is BeginDirective)
            {
                if (resetVectorSet)
                    return OperationResult<CompiledStatement>.Error(SyntaxErrors.DuplicateBeginDirective(stmt.Tokens[0], resetVector));
                
                resetVector = programCounter;
                resetVectorSet = true;
            }
            
            return OperationResult<CompiledStatement>.Success(new CompiledStatement(stmt, programCounter, null, evalResult.Result));
        }

        private OperationResult<List<Operand>> EvaluateDirective(Directive directive)
        {
            var evalResult = directive.Evaluate(ref programCounter);
            if (evalResult.Failed)
            {
                return OperationResult<List<Operand>>.Error(evalResult.TheError);
            }

            return OperationResult<List<Operand>>.Success(evalResult.Result);
        }

        private OperationResult FindLabel(AbstractStatement stmt)
        {
            OperationResult result = TryAddLabel(stmt);
            return result.Failed ? result : OperationResult.Success();
        }

        private OperationResult TryAddLabel(AbstractStatement stmt)
        {
            Label label = stmt switch
            {
                LabelStatement labelStmt => new Label(labelStmt.LabelName, null, true),
                InstructionStatementOperand opStmt => new Label(opStmt.Operand.LabelRef, null, false),
                _ => null
            };
            if (label is null) return OperationResult.Success();

            bool isDeclaration = stmt is LabelStatement;

            if (label.Name is not null)
            {
                if (labels.ContainsKey(label.Name))
                {
                    if (isDeclaration && labels[label.Name].IsDeclared)
                        return OperationResult.Error(
                            SyntaxErrors
                                .LabelAlreadyDeclared(
                                    stmt.Tokens[0])); // Assuming LabelStatements are created from 1 token
                    
                    labels[label.Name].IsDeclared = true;
                }
                else
                {
                    labels.Add(label.Name, label);
                }
            }
            return OperationResult.Success();
        }
    }
}