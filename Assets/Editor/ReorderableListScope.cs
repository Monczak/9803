using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor
{
    public class ReorderableListScope<T> : IDisposable
    {
        private T toRemove;
        private int index = -1;
        private bool moveUp = false;
        private bool moveDown = false;

        private readonly List<T> list;

        public delegate void DrawElementDelegate(T element, int index);

        private readonly DrawElementDelegate drawElement;
        

        public ReorderableListScope(List<T> list, DrawElementDelegate drawElement)
        {
            this.list = list;
            this.drawElement = drawElement;
            
            DrawElements();
        }

        private void DrawElements()
        {
            for (int i = 0; i < list.Count; i++)
            {
                using var horizontalScore = new GUILayout.HorizontalScope();
                using (new GUILayout.VerticalScope())
                {
                    drawElement(list[i], i);
                }

                if (GUILayout.Button("/\\", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 25 }))
                {
                    index = i;
                    moveUp = true;
                }

                if (GUILayout.Button("\\/", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 25 }))
                {
                    index = i;
                    moveDown = true;
                }

                if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 20 }))
                    toRemove = list[i];
            }
        }

        public void Dispose()
        {
            if (toRemove is not null) list.Remove(toRemove);
            if (moveUp && index > 0)
            {
                (list[index], list[index - 1]) =
                    (list[index - 1], list[index]);
            }

            if (moveDown && index < list.Count - 1 && index != -1)
            {
                (list[index], list[index + 1]) =
                    (list[index + 1], list[index]);
            }
        }
    }
}