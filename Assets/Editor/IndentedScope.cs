using System;
using UnityEngine;

namespace NineEightOhThree.Editor
{
    public class IndentedScope : IDisposable
    {
        public IndentedScope(int indentPixels = 10)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentPixels);
            GUILayout.BeginVertical();
        }
        
        public void Dispose()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}