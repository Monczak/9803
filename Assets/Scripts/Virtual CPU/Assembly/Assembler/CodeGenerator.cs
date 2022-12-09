using System;
using System.Collections.Generic;
using System.Text;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class CodeGenerator
    {
        private static List<AbstractStatement> statements;
        private static Dictionary<string, Label> labels;
        
        public static bool HadError { get; private set; }
        private static event ErrorHandler OnError;
        
        public static List<byte> GenerateCode(List<AbstractStatement> stmts)
        {
            List<byte> code = new();

            OperationResult result;
            statements = stmts;
            labels = new Dictionary<string, Label>();

            result = FindLabels();
            if (result.Failed)
            {
                ThrowError(result.TheError);
                return null;
            }

            StringBuilder builder = new();
            builder.AppendJoin(" ", labels);
            Debug.Log(builder.ToString());
            
            return code;
        }

        private static void ThrowError(AssemblerError? error)
        {
            HadError = true;
            OnError?.Invoke(error);
        }

        private static OperationResult FindLabels()
        {
            foreach (AbstractStatement stmt in statements)
            {
                OperationResult result = TryAddLabel(stmt);
                if (result.Failed) return result;
            }
            return OperationResult.Success();
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

        private static IEnumerator<(ushort programCounter, AbstractStatement stmt)> Walk()
        {
            ushort programCounter = 0;
            foreach (AbstractStatement stmt in statements)
            {
                yield return (programCounter, stmt);
            }
        }

        public static void RegisterErrorHandler(ErrorHandler handler)
        {
            OnError += handler;
        }
    }
}