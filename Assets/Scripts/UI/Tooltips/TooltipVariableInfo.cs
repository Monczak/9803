using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.Tooltips
{
    public class TooltipVariableInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text varNameText;

        [SerializeField] private GameObject tooltipLinePrefab;

        private List<TooltipLine> lines;

        public void MakeFromData(TooltipData data)
        {
            lines = new List<TooltipLine>();
            
            foreach (var _ in data.Lines)
            {
                TooltipLine tooltipLine = Instantiate(tooltipLinePrefab, transform).GetComponent<TooltipLine>();
                lines.Add(tooltipLine);
            }
            
            UpdateData(data);
        }

        public void UpdateData(TooltipData data)
        {
            varNameText.text = data.Header;
            for (int i = 0; i < data.Lines.Length; i++)
            {
                var (line, address) = data.Lines[i];
                lines[i].SetLine(line, address);
            }
        }
    }
}