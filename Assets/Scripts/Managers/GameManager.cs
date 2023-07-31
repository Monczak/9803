using System;
using NineEightOhThree.Controllers;
using NineEightOhThree.UI.BindableFormatters;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool debug;

        [Header("Hack Mode")]
        public bool canActivateHackMode = true;
        public bool hackModeActive = false;
        
        // Controls
        private UIControls uiControls;
        
        
        // Events
        public event EventHandler<bool> OnHackModeToggled; 

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

            SetupControls();

            // DontDestroyOnLoad(gameObject);

            LoadEverything();
        }

        private void SetupControls()
        {
            uiControls = new UIControls();
            uiControls.Game.ToggleHackMode.performed += _ =>
            {
                if (!canActivateHackMode) return;
                hackModeActive = !hackModeActive;
                OnHackModeToggled?.Invoke(this, hackModeActive);
            };
            
            uiControls.Enable();
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
            BindableFormatterRegistry.RegisterFormatters();
        }

        private void OnDestroy()
        {
            Logger.Finish();
        }
    }
}


