using NineEightOhThree.VirtualCPU;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.UI.MemoryViewer
{
    public class MemoryViewer : MonoBehaviour
    {
        public Memory memory;

        public int cellCount = 256;
        public GameObject cellPrefab;

        private List<MemoryCell> cells;
        public Transform memoryCellParent;

        public byte currentPage;

        public bool @decimal;

        private void Awake()
        {
            CreateCells();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateCells();
        }

        private void CreateCells()
        {
            cells = new List<MemoryCell>();

            for (int i = 0; i < cellCount; i++)
            {
                MemoryCell cell = Instantiate(cellPrefab, memoryCellParent).GetComponent<MemoryCell>();
                cells.Add(cell);

                cell.gameObject.name = $"Memory Cell {i:X2}";
            }
        }

        private void UpdateCells()
        {
            byte[] memoryDump = memory.Data;

            for (int i = 0; i < 0x100; i++)
            {
                ushort address = (ushort)((currentPage << 8) + i);
                cells[i].address = address;
                cells[i].Value = memoryDump[address];
                cells[i].showDecimal = @decimal;
            }
        }
    }
}

