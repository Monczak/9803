﻿using System;
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

            BuildQueue buildQueue = new();
            buildQueue.Add($"GameData/LevelScripts/{levelName}/main");
            buildQueue.Add($"GameData/LevelScripts/{levelName}/not_main");
            var result = buildQueue.Build();

            if (result.Failed)
            {
                Logger.LogError("Building level scripts failed!");
                foreach (var error in result.BuildErrors)
                    Logger.LogError($"{error.Job.ResourceLocation} - {error.Message}");
                foreach (var pair in result.JobAssemblerErrors)
                {
                    foreach (var error in pair.Value)
                        Logger.LogError($"Assembler: {pair.Key.ResourceLocation} - {error.Message}");
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