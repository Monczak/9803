using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.MemoryViewer
{
    public class MemoryCell : MonoBehaviour
    {
        public TMP_Text text;

        private byte value;
        public byte Value
        {
            get => value;
            set
            {
                this.value = value;
                UpdateUI();
            }
        }
        public ushort address;
        public bool showDecimal = false;

        public bool highlighted;
        public bool isWriteProtected;
        public bool isMachineCode;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateUI()
        {
            text.text = value.ToString(showDecimal ? "D3" : "X2");
        }
    }
}

