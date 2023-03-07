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

            BuildQueue buildQueue = new();
            buildQueue.Add(scripts["main"]);
            var result = buildQueue.Build();

            if (result.Failed)
            {
                Logger.LogError("Building level scripts failed!");
                foreach (var error in result.BuildErrors)
                    Logger.LogError($"{error.Job.ResourceLocation} - {error.Message}");
            }
            else
            {
                CPU.Instance.WriteCode(result.Code, result.CodeMask);
            }

            return true;
        }
    }
}