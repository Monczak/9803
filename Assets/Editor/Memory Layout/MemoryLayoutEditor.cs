using System;
using System.Collections.Generic;
using NineEightOhThree.Editor.MemoryLayout.Controllers;
using NineEightOhThree.Editor.Utils;
using NineEightOhThree.Editor.Utils.UI;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout
{
    public class MemoryLayoutEditor : EditorWindow
    {
        private MemoryLayoutEditorController controller;

        private BindableListController bindableListController;
        private MemoryEditorController memoryEditorController;
        
        private DelayedExecutor scheduler;

        private VisualTreeAsset bindableListItemTemplate;

        [MenuItem("Tools/Memory Layout Editor")]
        public static void ShowWindow()
        {
            MemoryLayoutEditor wnd = GetWindow<MemoryLayoutEditor>();
            wnd.titleContent = new GUIContent("Memory Layout Editor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
        
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Memory Layout/MemoryLayoutEditor.uxml");
            VisualElement ui = visualTree.CloneTree();
            root.Add(ui);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Memory Layout/MemoryLayoutEditor.uss");
            root.styleSheets.Add(styleSheet);

            bindableListItemTemplate =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Editor/Memory Layout/Templates/BindableListItem.uxml");
        }
            
        private void OnEnable()
        {
            controller = new MemoryLayoutEditorController();
            scheduler = new DelayedExecutor();
            
            controller.GetAllBindablesInScene();
            
            scheduler.Schedule(SetupBindableList);
            scheduler.Schedule(SetupMemoryEditor);
            
            scheduler.Schedule(SetupCallbacks);
        }

        private void SetupCallbacks()
        {
            rootVisualElement.Q<Button>("BindableListRefreshButton").clicked += RefreshButton_Clicked;
        }

        private void RefreshButton_Clicked()
        {
            SetupBindableList();
        }

        private void Update()
        {
            memoryEditorController?.Update();
            
            scheduler.ExecuteAll();
        }

        private void SetupBindableList()
        {
            bindableListController = new BindableListController(rootVisualElement);
            bindableListController.InitializeBindableList(bindableListItemTemplate,
                controller.Bindables);
            
            bindableListController.OnDragEnterOverlap += OnDragEnterOverlap;
            bindableListController.OnDragExitOverlap += OnDragExitOverlap;
        }

        private void OnDragExitOverlap(object sender, VisualElement e)
        {
            memoryEditorController.RemoveRegion(GetBindable((VisualElement)sender));
        }

        private void OnDragEnterOverlap(object sender, VisualElement e)
        {
            Bindable bindable = GetBindable((VisualElement)sender);
            ushort address = (ushort)(((MemoryEditorController.CellData)e.userData).Index + memoryEditorController.FirstAddress);
            ushort offset = (ushort)(address - bindable.addresses[0]);
 
            memoryEditorController.AddRegion(bindable, offset);
        }
        
        private Bindable GetBindable(VisualElement e)
        {
            return ((BindableListItemController)e.userData).Bindable;
        }

        private void SetupMemoryEditor()
        {
            memoryEditorController = new MemoryEditorController();
            memoryEditorController.Initialize(rootVisualElement);
        }
    }
}
