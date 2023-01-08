using System;
using System.Collections.Generic;
using NineEightOhThree.VirtualCPU;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NineEightOhThree.Managers
{
    public class LevelLoader : MonoBehaviour
    {
        private void Awake()
        {
            LoadScripts();
        }

        private bool LoadScripts()
        {
            string levelName = SceneManager.GetActiveScene().name;
            TextAsset[] scriptAssets = Resources.LoadAll<TextAsset>($"GameData/LevelScripts/TestScene");

            Dictionary<string, string> scripts = new();
            foreach (TextAsset asset in scriptAssets)
            {
                string text = asset.text.Replace("\r\n", "\n");
                scripts.Add(asset.name, text);
            }

            if (!scripts.ContainsKey("main"))
            {
                Logger.LogError($"Could not find main.asm for level {levelName}");
                return false;
            }
            Logger.Log($"Loaded {scripts.Count} scripts");

            // TODO: Assemble all scripts (figure out dependencies and stuff)
            // TODO: Make this asyncable
            // TODO: Handle errors better
            var result = AssemblerInterface.Assembler.Assemble(scripts["main"])();

            if (result.Errors.Count > 0)
            {
                Logger.LogError($"There were {result.Errors.Count} errors when loading main.asm!");
                foreach (var error in result.Errors)
                {
                    if (error is not null)
                    {
                        switch (error.Value.Type)
                        {
                            case AssemblerError.ErrorType.Lexical:
                                Logger.LogError($"Lexical error: {error.Value.Message} (line {error.Value.Line}, col {error.Value.Column})");
                                break;
                            case AssemblerError.ErrorType.Syntax:
                                Logger.LogError($"Syntax error: {error.Value.Message} ({error.Value.Token})");
                                break;
                            case AssemblerError.ErrorType.Internal:
                                Logger.LogError($"Internal error: {error.Value.Message}");
                                break;
                        }
                        
                    }
                }
                return false;
            }
            else
            {
                CPU.Instance.WriteCode(result);
            }

            return true;
        }
    }
}