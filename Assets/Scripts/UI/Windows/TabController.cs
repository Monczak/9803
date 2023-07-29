using System;
using UnityEngine;
using UnityEngine.UI;

namespace NineEightOhThree.UI.Windows
{
    public class TabController : MonoBehaviour
    {
        private Button button;

        public delegate void TabClickHandler();

        public void SetupButton(TabClickHandler handler)
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(() => handler());
        }
    }
}