using System;
using System.Collections.Generic;
using System.Text;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class CodeGenerator
    {
        private static List<AbstractStatement> statements;
        private static Dictionary<string, Label> labels;

        private static ushort programCounter;

        private const int MemorySize = 65536;
        
        public static bool HadError { get; private set; }
        private static event ErrorHandler OnError;
        
        public static byte[] GenerateCode(List<AbstractStatement> stmts)
        {
            byte[] code = new byte[MemorySize];

            statements = stmts;
            labels = new Dictionary<string, Label>();
            programCounter = 0;

            // Pass 1: Find labels
            foreach (AbstractStatement stmt in statements)
            {
                OperationResult result = FindLabel(stmt);
                if (result.Failed) ThrowError(result.TheError);
            }

            StringBuilder builder = new();
            builder.AppendJoin(" ", labels);
            Debug.Log(builder.ToString());

            // Pass 2: Convert statements to a compiled form
            List<CompiledStatement> compiledStatements = new();
            programCounter = 0;
            foreach (AbstractStatement stmt in statements)
            {
                OperationResult<CompiledStatement> result = CompileStatement(stmt);
                if (result.Failed) ThrowError(result.TheError);

                if (result.Result is not null)
                    compiledStatements.Add(result.Result);
            }

            // Pass 3: Replace label references with label addresses
            programCounter = 0;
            foreach (CompiledStatement cStmt in compiledStatements)
            {
                OperationResult result = UpdateLabelRefs(cStmt);
                if (result.Failed) ThrowError(result.TheError);
            }
            
            // Pass 4: Emit bytes
            if (!HadError)
            {
                foreach (CompiledStatement cStmt in compiledStatements)
                {
                    ushort pc = cStmt.StartProgramCounter;
                    foreach (byte b in cStmt.Bytes)
                    {
                        // TODO: Protect from overwriting existing code
                        code[pc++] = b;
                    }
                }
            }

            return HadError ? null : code;
        }

        private static OperationResult UpdateLabelRefs(CompiledStatement cStmt)
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

        private static void ThrowError(AssemblerError? error)
        {
            HadError = true;
            OnError?.Invoke(error);
        }
        
        private static OperationResult<CompiledStatement> CompileStatement(AbstractStatement stmt)
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
                    break;
                }
            }

            if (cStmt is not null)
            {
                programCounter += cStmt.ByteCount;
                Debug.Log(debugMsgBuilder.Append(cStmt).ToString());
            }

            return OperationResult<CompiledStatement>.Success(cStmt);
        }

        private static OperationResult<CompiledStatement> CompileInstruction(InstructionStatement stmt)
        {
            CompiledStatement cStmt = stmt switch
            {
                InstructionStatementOperand s => new CompiledStatement(programCounter, (s.AddressingMode, s.Metadata), s.Operand),
                _ => new CompiledStatement(programCounter, (stmt.AddressingMode, stmt.Metadata))
            };

            return OperationResult<CompiledStatement>.Success(cStmt);
        }

        private static OperationResult<CompiledStatement> CompileDirective(DirectiveStatement stmt)
        {
            var result = stmt.Directive.Build(stmt is DirectiveStatementOperands s ? s.Args : null);
            if (result.Failed)
                return OperationResult<CompiledStatement>.Error(result.TheError, stmt.Tokens[0]);

            var evalResult = EvaluateDirective(result.Result);
            if (evalResult.Failed)
                return OperationResult<CompiledStatement>.Error(evalResult.TheError);
            
            return OperationResult<CompiledStatement>.Success(new CompiledStatement(programCounter, null, evalResult.Result));
        }

        private static OperationResult<List<Operand>> EvaluateDirective(Directive directive)
        {
            var evalResult = directive.Evaluate(ref programCounter);
            if (evalResult.Failed)
            {
                return OperationResult<List<Operand>>.Error(evalResult.TheError);
            }

            return OperationResult<List<Operand>>.Success(evalResult.Result);
        }

        private static OperationResult FindLabel(AbstractStatement stmt)
        {
            OperationResult result = TryAddLabel(stmt);
            return result.Failed ? result : OperationResult.Success();
        }

        private static OperationResult TryAddLabel(AbstractStatement stmt)
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

        public static void RegisterErrorHandler(ErrorHandler handler)
        {
            OnError += handler;
        }
    }
}