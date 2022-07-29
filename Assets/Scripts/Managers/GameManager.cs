using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance != this) Destroy(gameObject);

            // Disable Unity URP Debug Canvas
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

            // DontDestroyOnLoad(gameObject);

            LoadEverything();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LoadEverything()
        {
            LoadInstructions();
        }

        private void LoadInstructions()
        {
            VirtualCPU.CPUInstructionRegistry.RegisterInstructions();
        }
    }
}


