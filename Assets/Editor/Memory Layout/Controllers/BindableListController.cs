﻿using System;
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

        public event EventHandler<VisualElement> OnDragEnterOverlap, OnDragExitOverlap;

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

        private VisualElement InitializeDragAndDrop(VisualTreeAsset elemTemplate, DragAndDropManipulator.VisualElementConstructor constructor, DragAndDropManipulator.VisualElementBinder binder)
        {
            DragAndDropManipulator manipulator = new(root, elemTemplate, constructor, binder);
            manipulator.OnDrop += (sender, data) => HandleDrop((VisualElement)sender, data);

            manipulator.OnEnterOverlap += (sender, element) => OnDragEnterOverlap?.Invoke(sender, element);
            manipulator.OnExitOverlap += (sender, element) => OnDragExitOverlap?.Invoke(sender, element);
            
            return manipulator.target;
        }


        private void FillBindableList()
        {
            bindableList.makeItem = () =>
            {
                VisualElement Construct(VisualTreeAsset template)
                {
                    var item = template.Instantiate();
                    var logic = new BindableListItemController();
                    item.userData = logic;
                    logic.Setup(item);
                    return item;
                }

                void Bind(VisualElement item, object data)
                {
                    BindableListItemController controller = data as BindableListItemController;
                    controller?.Setup(item);
                    controller?.SetData(bindables[controller.Index], controller.Index);
                }

                return InitializeDragAndDrop(listItemTemplate, Construct, Bind);
            };

            bindableList.bindItem = (item, index) =>
            {
                (item.userData as BindableListItemController)?.SetData(bindables[index], index);
            };
            
            bindableList.itemsSource = bindables;
            bindableList.RefreshItems();
        }

        private void HandleDrop(VisualElement sender, (bool success, VisualElement slot, Vector2 startPos) data)
        {
            sender.transform.position = data.startPos;
        }
    }
}