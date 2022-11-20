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

        public void Setup(VisualElement root)
        {
            bindableNameLabel = root.Q<Label>("BindableName");
            bindableOriginLabel = root.Q<Label>("BindableOrigin");
            bindableAddressesLabel = root.Q<Label>("BindableAddresses");
        }

        public void SetData(Bindable bindable)
        {
            bindableNameLabel.text = $"{bindable.parentClassName}.{bindable.fieldName}";
            bindableOriginLabel.text = bindable.parentObjectName;
            bindableAddressesLabel.text = BeautifyAddresses(bindable.addresses);
        }

        private string BeautifyAddresses(ushort[] addresses)
        {
            StringBuilder builder = new StringBuilder();

            ushort spanStart = addresses[0], spanEnd;
            bool isInSpan = false;

            List<(ushort, ushort)> addressSpans = new List<(ushort, ushort)>();

            for (int i = 0; i < addresses.Length; i++)
            {
                if (i + 1 != addresses.Length && System.Math.Abs(addresses[i + 1] - addresses[i]) == 1)
                {
                    if (!isInSpan)
                        spanStart = addresses[i];
                    isInSpan = true;
                }
                else if (isInSpan)
                {
                    isInSpan = false;
                    spanEnd = addresses[i];
                    addressSpans.Add((spanStart, spanEnd));
                }
                else
                {
                    addressSpans.Add((addresses[i], addresses[i]));
                }
            }

            builder.AppendJoin(", ", addressSpans.Select(span =>
                span.Item1 == span.Item2
                    ? $"{span.Item1:X4}"
                    : $"{span.Item1:X4}-{span.Item2:X4}"));

            return builder.ToString();
        }
    }
}