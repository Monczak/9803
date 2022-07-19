using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    public class Memory : MonoBehaviour
    {
        private byte[] data;

        public byte[] Data => data;
        public int size;

        public HashSet<int> writeProtectedAddresses;

        private void Awake()
        {
            Clear();
        }

        public void Clear()
        {
            data = new byte[size];
        }

        public byte Read(int address)
        {
            return data[address];
        }

        public bool Write(int address, byte value)
        {
            if (writeProtectedAddresses.Contains(address))
                return false;
            data[address] = value;
            return true;
        }

        public byte[] ReadBlock(int address, int count)
        {
            return data[address..(address + count)];
        }

        public void ProtectWrites(params int[] addresses)
        {
            foreach (int address in addresses)
                writeProtectedAddresses.Add(address);
        }

        public void UnprotectWrites(params int[] addresses)
        {
            foreach (int address in addresses)
                writeProtectedAddresses.Remove(address);
        }
    }
}
