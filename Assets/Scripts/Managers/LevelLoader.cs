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

            BuildQueue buildQueue = new();
            buildQueue.Add($"GameData/LevelScripts/{levelName}/bogus");
            buildQueue.Add($"GameData/LevelScripts/{levelName}/main");
            var result = buildQueue.Build();

            if (result.Failed)
            {
                Logger.LogError("Building level scripts failed!");
                foreach (var error in result.BuildErrors)
                    Logger.LogError($"{error.Job.ResourceLocation} - {error.Message}");
                foreach (var job in result.FailedJobs)
                {
                    if (job.Result is not null)
                    {
                        foreach (var error in job.Result.Errors)
                            Logger.LogError($"Assembler: {job.ResourceLocation} - {error.Message}");
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