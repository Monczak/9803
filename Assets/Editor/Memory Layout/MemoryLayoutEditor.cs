using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout
{
    public class MemoryLayoutEditor : EditorWindow
    {
        private MemoryLayoutEditorController controller;

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
        }
            
        private void OnEnable()
        {
            controller = new MemoryLayoutEditorController();
            
            controller.GetAllBindablesInScene();
            SetupBindableList();
        }

        private void SetupBindableList()
        {
            VisualElement bindableListPane = rootVisualElement.Query<VisualElement>("BindableListPane").First();
            bindableListPane.Add(new Label("hallo"));
            int itemHeight = 55;

            /*ListView listView = new ListView(controller.Bindables, itemHeight, BindableList_MakeItem,
                BindableList_BindItem);*/
        }

        private void BindableList_BindItem(VisualElement arg1, int arg2)
        {
            throw new System.NotImplementedException();
        }

        private VisualElement BindableList_MakeItem()
        {
            throw new System.NotImplementedException();
        }
    }
}
