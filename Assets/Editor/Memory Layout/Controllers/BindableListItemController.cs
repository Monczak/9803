using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout.Controllers
{
    public class BindableListItemController
    {
        private Label bindableNameLabel, bindableOriginLabel, bindableAddressesLabel;
        
        public int Index { get; private set; }
        
        public Bindable Bindable { get; private set; }

        public void Setup(VisualElement root)
        {
            bindableNameLabel = root.Q<Label>("BindableName");
            bindableOriginLabel = root.Q<Label>("BindableOrigin");
            bindableAddressesLabel = root.Q<Label>("BindableAddresses");
        }

        public void SetData(Bindable bindable, int index)
        {
            Index = index;
            Bindable = bindable;
            
            bindableNameLabel.text = $"{bindable.parentClassName}.{bindable.fieldName}";
            bindableOriginLabel.text = bindable.parentObjectName;
            bindableAddressesLabel.text = BeautifyAddresses(bindable);
        }

        private string BeautifyAddresses(Bindable bindable)
        {
            StringBuilder builder = new StringBuilder();

            if (bindable.IsPointer)
            {
                builder.Append("Pointer ");
                if (bindable.value is not null)
                {
                    builder.Append(bindable.addresses[0].ToString("X4")).Append("-")
                        .Append((bindable.addresses[0] + bindable.Bytes).ToString("X4"));
                }
                else
                {
                    builder.Append(bindable.addresses[0].ToString("X4")).Append(" (unknown size)");
                }
            }
            else
            {
                ushort spanStart = bindable.addresses[0];
                bool isInSpan = false;

                List<(ushort start, ushort end)> addressSpans = new();

                for (int i = 0; i < bindable.addresses.Length; i++)
                {
                    if (i + 1 != bindable.addresses.Length && System.Math.Abs(bindable.addresses[i + 1] - bindable.addresses[i]) == 1)
                    {
                        if (!isInSpan)
                            spanStart = bindable.addresses[i];
                        isInSpan = true;
                    }
                    else if (isInSpan)
                    {
                        isInSpan = false;
                        ushort spanEnd = bindable.addresses[i];
                        addressSpans.Add((spanStart, spanEnd));
                    }
                    else
                    {
                        addressSpans.Add((bindable.addresses[i], bindable.addresses[i]));
                    }
                }

                builder.AppendJoin(", ", addressSpans.Select(span =>
                    span.start == span.end
                        ? $"{span.start:X4}"
                        : $"{span.start:X4}-{span.end:X4}"));
            }
            
            return builder.ToString();
        }
    }
}