using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.UI.Layout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NineEightOhThree.UI.Tooltips
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text objNameText;
        [SerializeField] private RectTransform container;
        [SerializeField] private GameObject tooltipVariableInfoPrefab;

        private List<TooltipVariableInfo> varInfos;

        private ContentSizeFitterRefresher refresher;

        public TooltipDataProvider Provider { get; private set; }

        private void Awake()
        {
            varInfos = new List<TooltipVariableInfo>();
            refresher = GetComponent<ContentSizeFitterRefresher>();
        }

        public void MakeFromData(TooltipData data)
        {
            foreach (var info in varInfos)
            {
                Destroy(info.gameObject);
            }
            
            varInfos = new List<TooltipVariableInfo>();
            
            TooltipVariableInfo varInfo = Instantiate(tooltipVariableInfoPrefab, container.transform)
                .GetComponent<TooltipVariableInfo>();
            varInfo.MakeFromData(data);
            
            varInfos.Add(varInfo);
            
            refresher.RefreshContentFitters();
        }

        public void Make(TooltipDataProvider provider)
        {
            Provider = provider;
            
            var data = Provider.GetAllData();
            foreach (TooltipData dataPiece in data)
            {
                MakeFromData(dataPiece);
            }
            
            UpdateData();
        }

        public void UpdateData()
        {
            objNameText.text = Provider.gameObject.name;
            
            var data = Provider.GetAllData();
            for (int i = 0; i < varInfos.Count; i++)
            {
                varInfos[i].UpdateData(data[i]);
            }
        }

        public void Despawn()
        {
            Destroy(gameObject);
        }
    }
}

