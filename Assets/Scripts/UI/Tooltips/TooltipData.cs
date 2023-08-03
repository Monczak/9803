using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree.UI.Tooltips
{
    public struct TooltipData
    {
        public string Header { get; }
        public (string line, ushort address)[] Lines { get; }

        public TooltipData(string header, (string line, ushort address)[] lines)
        {
            Header = header;
            Lines = lines;
        }
    }
}