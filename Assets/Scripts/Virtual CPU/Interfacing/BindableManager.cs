using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    public class BindableManager : MonoBehaviour
    {
        private Dictionary<ushort, Bindable> bindables = new();

        private HashSet<ushort> dirtyAddresses = new();

        private Memory memory;

        public void RegisterBindable(Bindable bindable)
        {
            bindables.Add(bindable.address, bindable);
        }

        public void UnregisterBindable(Bindable bindable)
        {
            bindables.Remove(bindable.address);
        }

        public void SetDirty(ushort address)
        {
            dirtyAddresses.Add(address);
        }

        public void Synchronize()
        {
            foreach (Bindable bindable in bindables.Values)
            {
                for (ushort address = bindable.address; address < bindable.address + bindable.Bytes; address++)
                {
                    if (dirtyAddresses.Contains(address))
                    {
                        bindable.SetValueFromBytes(memory.ReadBlock(bindable.address, bindable.Bytes));
                        break;
                    }
                }
            }

            foreach (Bindable bindable in bindables.Values)
            {
                memory.WriteBlock(bindable.address, bindable.GetBytes());
            }

            dirtyAddresses.Clear();
        }


        private void Awake()
        {
            memory = GetComponent<Memory>();
        }
    }
}
