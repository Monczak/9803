using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.Tooltips
{
    public class TooltipVariableInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text varNameText;

        [SerializeField] private GameObject tooltipLinePrefab;

        public void MakeFromData(TooltipData data)
        {
            varNameText.text = data.Header;

            foreach (var (line, address) in data.Lines)
            {
                TooltipLine tooltipLine = Instantiate(tooltipLinePrefab, transform).GetComponent<TooltipLine>();
                tooltipLine.SetLine(line, address);
            }
        }
    }
}