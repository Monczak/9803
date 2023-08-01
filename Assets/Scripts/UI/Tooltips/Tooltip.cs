using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.Tooltips
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text objNameText;

        [SerializeField] private RectTransform container;
        
        [SerializeField] private GameObject tooltipVariableInfoPrefab;

        public void MakeFromData(TooltipData data)
        {
            TooltipVariableInfo varInfo = Instantiate(tooltipVariableInfoPrefab, container.transform)
                .GetComponent<TooltipVariableInfo>();
            varInfo.MakeFromData(data);
        }

        public void Make(TooltipData[] data)
        {
            foreach (TooltipData dataPiece in data)
            {
                MakeFromData(dataPiece);
            }
        }
    }
}

