using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NineEightOhThree.Classes;

namespace NineEightOhThree.Managers
{
    public class BugManager : MonoBehaviour
    {
        public static BugManager Instance { get; private set; }

        private Dictionary<string, BugInfo> bugs;
        private Dictionary<string, bool> activeBugs;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance != this) Destroy(gameObject);
            
            LoadBugs();
        }

        private void LoadBugs()
        {
            TextAsset bugFile = Resources.Load<TextAsset>("GameData/Bugs");
            if (bugFile == null)
            {
                Logger.LogError($"Failed to load bugs (file couldn't be loaded)");
                return;
            }
            
            List<BugInfo> bugList = JsonConvert.DeserializeObject<List<BugInfo>>(bugFile.text);
            if (bugList == null)
            {
                Logger.LogError($"Failed to load bugs (something went wrong with JSON)");
                return;
            }
            Logger.Log($"Loaded {bugList.Count} bugs");

            bugs = new Dictionary<string, BugInfo>();
            activeBugs = new Dictionary<string, bool>();
            foreach (BugInfo bug in bugList)
            {
                activeBugs[bug.Id] = false;
                bugs[bug.Id] = bug;
            }
        }

        public bool IsActive(string id) => activeBugs[id];

    }
}