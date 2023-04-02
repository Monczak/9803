using System;
using System.Collections.Generic;
using NineEightOhThree.VirtualCPU;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using NineEightOhThree.VirtualCPU.Assembly.Build;
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

            BuildQueue buildQueue = new(debug: true);
            buildQueue.Add($"GameData/LevelScripts/{levelName}/main");
            var result = buildQueue.Build();
            
            foreach (var log in result.Logs)
                Logger.Log(log);

            if (result.Failed)
            {
                Logger.LogError("Building level scripts failed!");
                foreach (var error in result.BuildErrors)
                    Logger.LogError($"{error.Job.ResourceLocation} - {error.Message}");
                foreach (var pair in result.JobAssemblerErrors)
                {
                    foreach (var error in pair.Value)
                    {
                        switch (error.Type)
                        {
                            case AssemblerError.ErrorType.Internal:
                                Logger.LogError($"Internal assembler error: {error.Message}");
                                break;
                            case AssemblerError.ErrorType.Lexical:
                            case AssemblerError.ErrorType.Syntax:
                            default:
                                Logger.LogError($"Assembler: {error.Token.ResourceLocation} - {error.Message} ({error.Token})");
                                break;
                        }
                    }
                }
            }
            else
            {
                CPU.Instance.WriteCode(result.Code, result.CodeMask);
            }

            return true;
        }
    }
}