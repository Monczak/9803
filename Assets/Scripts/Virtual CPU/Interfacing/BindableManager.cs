using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    public class BindableManager : MonoBehaviour
    {
        private ConcurrentDictionary<ushort, Bindable> bindables = new();

        private ConcurrentBag<ushort> dirtyAddresses = new();

        private Memory memory;

        public void RegisterBindable(Bindable bindable)
        {
            foreach (ushort address in bindable.addresses)
                bindables.TryAdd(address, bindable);
        }

        public void UnregisterBindable(Bindable bindable)
        {
            foreach (ushort address in bindable.addresses)
            {
                if (bindables.ContainsKey(address))
                    bindables.TryRemove(address, out _);
            }
        }

        public void SetDirty(ushort address)
        {
            dirtyAddresses.Add(address);
        }

        public void Synchronize()
        {
            foreach (Bindable bindable in bindables.Values)
            {
                if (!bindable.enabled) continue;

                byte[] bytes = new byte[bindable.Bytes];
                bool shouldUpdate = false;

                for (int i = 0; i < bindable.Bytes; i++)
                {
                    ushort address = bindable.IsPointer ? (ushort)(bindable.addresses[0] + i) : bindable.addresses[i];
                    bytes[i] = memory.Read(address);
                    if (dirtyAddresses.Contains(address))
                    {
                        shouldUpdate = true;
                    }
                }

                if (shouldUpdate)
                {
                    bindable.SetValueFromBytes(bytes);
                }
                
            }

            foreach (Bindable bindable in bindables.Values)
            {
                if (!bindable.enabled) continue;

                byte[] bytes = bindable.GetBytes();

                for (var i = 0; i < bindable.Bytes; i++)
                {
                    ushort address = bindable.IsPointer ? (ushort)(bindable.addresses[0] + i) : bindable.addresses[i];
                    memory.Write(address, bytes[i], false);
                }
            }

            dirtyAddresses.Clear();
        }


        private void Awake()
        {
            memory = GetComponent<Memory>();
        }
    }
}
