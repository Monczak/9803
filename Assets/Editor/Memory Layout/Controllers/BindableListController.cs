using System.Collections.Generic;
using NineEightOhThree.Editor.Utils.UI;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout.Controllers
{
    public class BindableListController
    {
        private VisualTreeAsset listItemTemplate;
        private ListView bindableList;

        private List<Bindable> bindables;
        private List<VisualElement> listItems;

        private VisualElement root;

        public BindableListController(VisualElement root)
        {
            this.root = root;
        }

        public void InitializeBindableList(VisualTreeAsset bindableListItemTemplate, List<Bindable> bindables)
        {
            listItemTemplate = bindableListItemTemplate;
            this.bindables = bindables;
            bindableList = root.Q<ListView>("BindableList");
            FillBindableList();
        }

        private void InitializeDragAndDrop(VisualElement elem)
        {
            DragAndDropManipulator manipulator = new(root,root.Q<VisualElement>("MemoryEditorContainer"), elem);
            manipulator.OnDrop += (sender, data) => HandleDrop((VisualElement)sender, data);
        }


        private void FillBindableList()
        {
            bindableList.makeItem = () =>
            {
                var item = listItemTemplate.Instantiate();
                var logic = new BindableListItemController();
                item.userData = logic;
                logic.Setup(item);

                InitializeDragAndDrop(item.contentContainer);
                
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

        private void HandleDrop(VisualElement sender, (bool success, VisualElement slot, Vector2 startPos) data)
        {
            if (data.success)
            {
                Debug.Log($"Success, slot name = {data.slot.name}");
            }
            else
            {
                Debug.Log($"Failure, startPos = {data.startPos}");
            }
            
            sender.transform.position = data.startPos;
        }
    }
}