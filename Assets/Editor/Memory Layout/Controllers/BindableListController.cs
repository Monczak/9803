using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout.Controllers
{
    public class BindableListController
    {
        private VisualTreeAsset listItemTemplate;
        private ListView bindableList;

        private List<Bindable> bindables;

        public void InitializeBindableList(VisualElement root, VisualTreeAsset bindableListItemTemplate, List<Bindable> bindables)
        {
            listItemTemplate = bindableListItemTemplate;
            this.bindables = bindables;
            bindableList = root.Q<ListView>("BindableList");
            FillBindableList();
        }

        private void FillBindableList()
        {
            bindableList.makeItem = () =>
            {
                var item = listItemTemplate.Instantiate();
                var logic = new BindableListItemController();
                item.userData = logic;
                logic.Setup(item);
                return item;
            };

            bindableList.bindItem = (item, index) =>
            {
                (item.userData as BindableListItemController)?.SetData(bindables[index]);
            };

            bindableList.fixedItemHeight = 55;
            bindableList.itemsSource = bindables;
            bindableList.RefreshItems();
        }
    }
}