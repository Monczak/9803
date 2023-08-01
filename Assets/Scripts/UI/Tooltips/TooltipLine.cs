using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.Tooltips
{
    public class TooltipLine : MonoBehaviour
    {
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private TMP_Text addressText;

        public void SetLine(string value, ushort address)
        {
            valueText.text = value;
            addressText.text = $"({address:X4})";
        }
    }
}