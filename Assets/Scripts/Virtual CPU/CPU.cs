using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    [RequireComponent(typeof(Memory))]
    public class CPU : MonoBehaviour
    {
        private Memory memory;

        private int programCounter;
        public int ProgramCounter => programCounter;

        private void Awake()
        {
            memory = GetComponent<Memory>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Tick()
        {

        }
    }
}

