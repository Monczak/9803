using NineEightOhThree.Utilities;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Properties
{
    [CustomPropertyDrawer(typeof(CurveAttribute))]
    public class SamCurveDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CurveAttribute curveAttribute = (CurveAttribute)attribute;
            if (property.propertyType == SerializedPropertyType.AnimationCurve)
            {
                if (curveAttribute.Enabled)
                    EditorGUI.CurveField(position, property, Color.green,
                        new Rect(curveAttribute.PosX, curveAttribute.PosY, curveAttribute.RangeX,
                            curveAttribute.RangeY));
            }
        }
    }
}