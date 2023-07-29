using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.UI.Windows;
using UnityEngine;

namespace NineEightOhThree.UI.HackMode
{
    public class TabbedWindowManager : MonoBehaviour
    {
        public RectTransform tabContainer;
        public RectTransform contentContainer;
        
        [field: SerializeField] public List<Window> Windows { get; private set; }
        private List<(TabController tab, RectTransform content)> windowWidgets;

        [SerializeField] private int selectedTab;
        public int SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                UpdateContent();
            }
        }

        private void Awake()
        {
            windowWidgets = new List<(TabController tab, RectTransform content)>();

            int index = 0;
            
            foreach (Window window in Windows)
            {
                TabController tab = null;
                RectTransform content = null;
                
                if (window.TabPrefab is null)
                    Logger.LogError($"Window {window.name} has no tab prefab");
                else
                    tab = Instantiate(window.TabPrefab, tabContainer).AddComponent<TabController>();
                
                if (window.ContentPrefab is null)
                    Logger.LogError($"Window {window.name} has no content prefab");
                else
                    content = Instantiate(window.ContentPrefab, contentContainer).GetComponent<RectTransform>();

                if (tab)
                {
                    int currentIndex = index;
                    tab.SetupButton(() =>
                    {
                        SelectedTab = currentIndex;
                    });
                }
                
                if (content)
                {
                    content.gameObject.SetActive(false);
                }
                
                windowWidgets.Add((tab, content));

                index++;
            }
            
            UpdateContent();
        }

        private void UpdateContent()
        {
            foreach (var (tab, content) in windowWidgets)
            {
                content.gameObject.SetActive(false);
            }

            var (activeTab, activeContent) = windowWidgets[selectedTab];
            activeContent.gameObject.SetActive(true);
        }
    }
}
