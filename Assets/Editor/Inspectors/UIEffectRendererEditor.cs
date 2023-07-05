using NineEightOhThree.Rendering.Effects;
using UnityEditor;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(UIEffectRenderer))]
    public class UIEffectRendererEditor : UnityEditor.Editor
    {
        private UIEffectRenderer theRenderer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        
    }
}