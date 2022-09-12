using NineEightOhThree.VirtualCPU.Interfacing;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    [RequireComponent(typeof(BindableManager))]
    public class Memory : MonoBehaviour
    {
        private byte[] data;

        public byte[] Data => data;
        public int size;

        public HashSet<ushort> writeProtectedAddresses;

        private BindableManager bindableManager;

        private void Awake()
        {
            Clear();
            writeProtectedAddresses = new HashSet<ushort>();

            bindableManager = GetComponent<BindableManager>();
        }

        public void Clear()
        {
            data = new byte[size];
        }

        public byte Read(ushort address)
        {
            return data[address];
        }

        public byte Read(ushort address, byte offset)
        {
            return Read((ushort)(address + offset));
        }

        public bool Write(ushort address, byte value, bool setDirty = true)
        {
            if (writeProtectedAddresses.Contains(address))
                return false;
            data[address] = value;
            if (setDirty) bindableManager.SetDirty(address);
            return true;
        }

        public bool Write(ushort address, byte offset, byte value)
        {
            return Write((ushort)(address + offset), value);
        }

        public byte[] ReadBlock(ushort address, int count)
        {
            return data[address..(address + count)];
        }

        public bool WriteBlock(ushort address, byte[] bytes, bool setDirty = true)
        {
            bool success = true;
            for (ushort addr = address; addr < address + bytes.Length; addr++)
                success &= Write(addr, bytes[addr - address], setDirty);
            return success;
        }

        public void ProtectWrites(params ushort[] addresses)
        {
            foreach (ushort address in addresses)
                writeProtectedAddresses.Add(address);
        }

        public void UnprotectWrites(params ushort[] addresses)
        {
            foreach (ushort address in addresses)
                writeProtectedAddresses.Remove(address);
        }
    }
}
