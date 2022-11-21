using System.Collections.Generic;
using NineEightOhThree.Editor.Utils.UI;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEditor.U2D;
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
        
        private Dictionary<VisualElement, DragAndDropManipulator> listItemManipulators;

        public void InitializeBindableList(VisualElement root, VisualTreeAsset bindableListItemTemplate, List<Bindable> bindables)
        {
            listItemTemplate = bindableListItemTemplate;
            this.bindables = bindables;
            bindableList = root.Q<ListView>("BindableList");
            FillBindableList();

            listItems = bindableList.Query<VisualElement>("BindableListItem").ToList();
            
            listItemManipulators = new Dictionary<VisualElement, DragAndDropManipulator>();
            foreach (var item in listItems)
            {
                Debug.Log(item.name);
                DragAndDropManipulator manipulator = new(item);
                manipulator.OnDropSuccess += (sender, slot) => HandleDropSuccess((VisualElement)sender, slot);
                manipulator.OnDropFailure += (sender, startPos) => HandleDropFailure((VisualElement)sender, startPos);
                listItemManipulators.Add(item, manipulator);
            }
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

        private void HandleDropSuccess(VisualElement sender, VisualElement slot)
        {
            Debug.Log($"Success, slot name = {slot.name}");
        }

        private void HandleDropFailure(VisualElement sender, Vector2 startPos)
        {
            Debug.Log($"Failure, startPos = {startPos}");
            sender.transform.position = startPos;
        }
    }
}