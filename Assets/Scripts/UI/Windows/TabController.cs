using System;
using UnityEngine;
using UnityEngine.UI;

namespace NineEightOhThree.UI.Windows
{
    public class TabController : MonoBehaviour
    {
        private Button button;
        private Image icon;
        private Image background;

        private Color activeColor, inactiveColor;
        private Color iconColor, backgroundColor;

        public delegate void TabClickHandler();

        public void SetupTab(TabClickHandler handler, Color activeColor, Color inactiveColor)
        {
            button = GetComponentInChildren<Button>();
            icon = button.GetComponent<Image>();
            background = GetComponent<Image>();
            
            button.onClick.AddListener(() => handler());

            this.activeColor = activeColor;
            this.inactiveColor = inactiveColor;

            iconColor = icon.color;
            backgroundColor = background.color;
        }

        public void SetActive(bool active)
        {
            icon.color = iconColor * (active ? activeColor : inactiveColor);
            background.color = backgroundColor * (active ? activeColor : inactiveColor);
        }
    }
}