using System;
using System.Collections.Generic;
using NineEightOhThree.VirtualCPU;
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
            // TODO: Handle errors
            var result = AssemblerInterface.Assembler.Assemble(scripts["main"])();
            CPU.Instance.WriteCode(result);
            
            return true;
        }
    }
}