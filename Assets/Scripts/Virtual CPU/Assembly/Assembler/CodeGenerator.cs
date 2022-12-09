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

            statements = stmts;
            labels = new Dictionary<string, Label>();

            OperationResult result = FindLabels();
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

        private static OperationResult TryAddLabel(AbstractStatement stmt, bool isDeclaration = true)
        {
            string labelName = stmt switch
            {
                LabelStatement labelStmt => labelStmt.LabelName,
                InstructionStatementOperand opStmt => opStmt.Operand.LabelRef,
                _ => null
            };

            if (labelName != null)
            {
                if (isDeclaration && labels.ContainsKey(labelName))
                    return OperationResult.Error(SyntaxErrors.LabelAlreadyDefined(stmt.Tokens[0])); // Assuming LabelStatements are created from 1 token
                labels.Add(labelName, new Label(labelName, null));
            }
            return OperationResult.Success();
        }

        public static void RegisterErrorHandler(ErrorHandler handler)
        {
            OnError += handler;
        }
    }
}