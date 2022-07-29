using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    public class Memory : MonoBehaviour
    {
        private byte[] data;

        public byte[] Data => data;
        public int size;

        public HashSet<byte> writeProtectedAddresses;

        private void Awake()
        {
            Clear();
            writeProtectedAddresses = new HashSet<byte>();
        }

        public void Clear()
        {
            data = new byte[size];
        }

        public byte Read(byte address)
        {
            return data[address];
        }

        public byte Read(byte address, byte offset)
        {
            return Read((byte)(address + offset));
        }

        public bool Write(byte address, byte value)
        {
            if (writeProtectedAddresses.Contains(address))
                return false;
            data[address] = value;
            return true;
        }

        public bool Write(byte address, byte offset, byte value)
        {
            return Write((byte)(address + offset), value);
        }

        public byte[] ReadBlock(byte address, int count)
        {
            return data[address..(address + count)];
        }

        public void ProtectWrites(params byte[] addresses)
        {
            foreach (byte address in addresses)
                writeProtectedAddresses.Add(address);
        }

        public void UnprotectWrites(params byte[] addresses)
        {
            foreach (byte address in addresses)
                writeProtectedAddresses.Remove(address);
        }
    }
}
