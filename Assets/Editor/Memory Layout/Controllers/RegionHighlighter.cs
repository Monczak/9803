using System;
using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Interfacing;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace NineEightOhThree.Editor.MemoryLayout.Controllers
{
    public class RegionHighlighter
    {
        public class Region
        {
            public ushort[] Addresses { get; init; }
            public int Bytes { get; init; }
            public Color Color { get; init; }
            public bool IsPointer { get; init; }
            public ushort Offset { get; init; }
        }

        private List<VisualElement> memoryCells;
        private Dictionary<Bindable, Region> regions;

        private const float BorderWidth = 3;

        [Flags]
        private enum BorderSide
        {
            None = 0,
            Bottom = 1,
            Left = 2,
            Right = 4,
            Top = 8,
            All = Bottom | Left | Right | Top
        }
        
        public RegionHighlighter(List<VisualElement> memoryCells)
        {
            this.memoryCells = memoryCells;
            regions = new Dictionary<Bindable, Region>();
        }

        public Region AddRegion(Bindable bindable, ushort offset, Color color)
        {
            Region region = new Region
            {
                Addresses = bindable.addresses,
                IsPointer = bindable.IsPointer,
                Bytes = bindable.IsPointer ? bindable.Bytes : bindable.addresses.Length,
                Color = color,
                Offset = offset
            };
            regions[bindable] = region;
            return region;
        }

        public Region AddRegion(Bindable bindable)
        {
            return AddRegion(bindable, 0, GetColor(bindable));
        }

        public Region AddRegion(Bindable bindable, ushort offset)
        {
            return AddRegion(bindable, offset, GetColor(bindable));
        }
        
        public void RemoveRegion(Bindable bindable)
        {
            regions.Remove(bindable);
        }

        private Color GetColor(Bindable bindable)
        {
            return Color.HSVToRGB((uint)bindable.ToString().GetHashCode() / 4294967296f, 0.7f, 1);
        }
        
        public void Update(ushort firstAddress)
        {
            for (ushort i = 0; i < 256; i++)
            {
                SetBorder(memoryCells[i].style, BorderSide.All, Color.clear);
            }
            
            foreach (Region region in regions.Values)
            {
                foreach (ushort address in region.Addresses)
                {
                    int count = 1;
                    if (region.IsPointer)
                    {
                        count = region.Bytes;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int cellIndex = (ushort)(address + region.Offset + i) - firstAddress;
                        if (cellIndex is < 0 or > 255)
                            continue;

                        // TODO: Make borders prettier (don't put borders between adjacent cells)
                        SetBorder(memoryCells[cellIndex].style, BorderSide.All, region.Color);
                    }
                }
            }
        }

        private void SetBorder(IStyle style, BorderSide sides, Color color)
        {
            if ((sides & BorderSide.Bottom) != 0) style.borderBottomWidth = BorderWidth;
            if ((sides & BorderSide.Left) != 0) style.borderLeftWidth = BorderWidth;
            if ((sides & BorderSide.Right) != 0) style.borderRightWidth = BorderWidth;
            if ((sides & BorderSide.Top) != 0) style.borderTopWidth = BorderWidth;
            if ((sides & BorderSide.Bottom) != 0) style.borderBottomColor = color;
            if ((sides & BorderSide.Left) != 0) style.borderLeftColor = color;
            if ((sides & BorderSide.Right) != 0) style.borderRightColor = color;
            if ((sides & BorderSide.Top) != 0) style.borderTopColor = color;
        }
    }
}