using System;
using NineEightOhThree.Controllers;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool debug;
        
        public Camera GameCamera { get; private set; }
        public Camera UICamera { get; private set; }
        public PlayerController Player { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance != this) Destroy(gameObject);

            // Disable Unity URP Debug Canvas
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

            if (!Application.isEditor)
                Debug.developerConsoleVisible = debug;

            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 0;
            
            Logger.Setup();

            // DontDestroyOnLoad(gameObject);

            LoadEverything();
        }

        // Start is called before the first frame update
        void Start()
        {
            FindObjects();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FindObjects()
        {
            GameCamera = GameObject.FindWithTag("GameCamera").GetComponent<Camera>();
            UICamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            
            Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }

        private void LoadEverything()
        {
            Inventories.ItemRegistry.RegisterItems();
            VirtualCPU.CPUInstructionRegistry.RegisterInstructions();
            VirtualCPU.Assembly.Assembler.Directives.DirectiveRegistry.RegisterDirectives();
        }

        private void OnDestroy()
        {
            Logger.Finish();
        }
    }
}


