using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NineEightOhThree.Managers;
using NineEightOhThree.VirtualCPU;
using NineEightOhThree.VirtualCPU.Assembly;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Token = NineEightOhThree.VirtualCPU.Assembly.Token;

namespace NineEightOhThree.UI.CodeEditor
{
    [RequireComponent(typeof(TMPTextFormatter))]
    public class TestCodeEditorController : MonoBehaviour
    {
        private TMP_InputField inputField;
        private TMPTextFormatter formatter;

        private string currentCode;
        private float timeToAssembly;

        public float assemblerCooldown = 1;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
            formatter = GetComponent<TMPTextFormatter>();
            
            inputField.caretWidth = 6;
            inputField.onValueChanged.AddListener(OnInputChanged);

            timeToAssembly = 0;
        }

        private void OnInputChanged(string input)
        {
            currentCode = input;
            timeToAssembly = assemblerCooldown;
        }

        private void Update()
        {
            if (timeToAssembly > 0)
            {
                timeToAssembly -= Time.deltaTime;
                if (timeToAssembly <= 0)
                {
                    AssembleCode();
                }
            }

            if (inputField.text.Length == 0)
            {
                ClearFormatting();
            }
            else if (inputField.text.Length > 0 && inputField.textComponent.textInfo.characterInfo[0].character != '\xFF')  // Check shibboleth left by formatter
            {
                FormatText();
            }
        }

        private void ClearFormatting()
        {
            formatter.Begin(inputField.textComponent);
            formatter.Apply();
        }

        private void FormatText()
        {
            formatter.Begin(inputField.textComponent);

            // TODO: Make this asyncable (wait for assembler to finish assembling using the task queue and then format)
            var result = AssemblerInterface.Assemble(currentCode);

            foreach (Token token in result.Tokens)
            {
                if (token.Type is TokenType.Newline or TokenType.EndOfFile) continue;
            
                Color32 color = new Color32(200, 200, 200, 255);
                if (token.MetaType is TokenMetaType.None)
                    color = SyntaxColors.First(item => item.pattern == (token.Type, TokenMetaType.All)).color;
                else
                {
                    foreach (var pair in SyntaxColors)
                    {
                        if ((pair.pattern.type & token.Type) != 0)
                        {
                            if (pair.pattern.metaType == TokenMetaType.All)
                            {
                                color = SyntaxColors.First(item => item.pattern == (token.Type, TokenMetaType.All)).color;
                                break;
                            }
                            if ((pair.pattern.metaType & token.MetaType) != 0)
                            {
                                if (SyntaxColors.Any(item => item.pattern == (token.Type, token.MetaType)))
                                    color = SyntaxColors.First(item => item.pattern == (token.Type, token.MetaType)).color;
                                else if (SyntaxColors.Any(item => item.pattern == (token.Type, TokenMetaType.AllValid)))
                                    color = SyntaxColors.First(item => item.pattern == (token.Type, TokenMetaType.AllValid)).color;
                                else
                                    color = SyntaxColors.First(item => item.pattern == (token.Type, TokenMetaType.All)).color;
                                break;
                            }
                        }
                    }
                }
                
                formatter.Color(token.CharIndex, token.Content.Length, color);
            }

            foreach (AssemblerError error in result.Errors)
            {
                switch (error.Type)
                {
                    case AssemblerError.ErrorType.Syntax:
                        if (error.Token is null)
                        {
                            Logger.LogError("Token is null");
                            break;
                        }
                        formatter.Underline(error.Token.CharIndex, error.Token.Content.Length);
                        break;
                    case AssemblerError.ErrorType.Lexical:
                        if (error.CharIndex is null || error.Length is null)
                        {
                            Logger.LogError("Error char index or length is null");
                            break;
                        }
                        formatter.Underline(error.CharIndex.Value, error.Length.Value);
                        break;
                    case AssemblerError.ErrorType.Internal:
                        Logger.LogError(error.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            formatter.Apply();
        }
        
        
        private void AssembleCode()
        {
            AssemblerInterface.ScheduleAssembly(currentCode, CPU.Instance.WriteCode);
        }

        /*public Dictionary<TokenType, Color32> SyntaxColors => new()
        {
            { TokenType.Identifier, new Color32(245, 144, 66, 255) },
            { TokenType.Number, new Color32(132, 245, 66, 255) },
            { TokenType.LeftParen, new Color32(66, 149, 245, 255) },
            { TokenType.RightParen, new Color32(66, 149, 245, 255) },
            { TokenType.Comma, new Color32(66, 149, 245, 255) },
            { TokenType.ImmediateOp, new Color32(122, 225, 56, 255) },
            { TokenType.LabelDecl, new Color32(245, 81, 66, 255) },
            { TokenType.RegisterA, new Color32(155, 66, 245, 255) },
            { TokenType.RegisterX, new Color32(245, 66, 209, 255) },
            { TokenType.RegisterY, new Color32(245, 66, 129, 255) },
            { TokenType.Directive, new Color32(245, 239, 66, 255) },
        };*/
        
        // TODO: Extract this to a config file
        public List<((TokenType type, TokenMetaType metaType) pattern, Color32 color)> SyntaxColors => new()
        {
            ((TokenType.Identifier, TokenMetaType.Label), new Color32(245, 81, 66, 255)),
            ((TokenType.Identifier, TokenMetaType.Constant), new Color32(245, 21, 146, 255)),
            ((TokenType.Identifier, TokenMetaType.Instruction), new Color32(245, 144, 66, 255)),
            ((TokenType.Identifier, TokenMetaType.AllValid), new Color32(245, 144, 66, 255)),
            ((TokenType.Number, TokenMetaType.All), new Color32(132, 245, 66, 255)),
            ((TokenType.LeftParen, TokenMetaType.All), new Color32(66, 149, 245, 255)),
            ((TokenType.RightParen, TokenMetaType.All), new Color32(66, 149, 245, 255)),
            ((TokenType.Comma, TokenMetaType.All), new Color32(66, 149, 245, 255)),
            ((TokenType.ImmediateOp, TokenMetaType.All), new Color32(122, 225, 56, 255)),
            ((TokenType.LabelDecl, TokenMetaType.All), new Color32(245, 81, 66, 255)),
            ((TokenType.RegisterA, TokenMetaType.All), new Color32(155, 66, 245, 255)),
            ((TokenType.RegisterX, TokenMetaType.All), new Color32(245, 66, 209, 255)),
            ((TokenType.RegisterY, TokenMetaType.All), new Color32(245, 66, 129, 255)),
            ((TokenType.Directive, TokenMetaType.AllValid), new Color32(245, 239, 66, 255)),
        };
    }
}

