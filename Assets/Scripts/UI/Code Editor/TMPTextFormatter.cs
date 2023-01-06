using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI
{
    public class TMPTextFormatter : MonoBehaviour
    {
        private TMP_Text text;

        public GameObject meshHolder;
        private CanvasRenderer canvasRenderer;
        
        private Mesh decoMesh;
        private TMP_MeshInfo decoMeshInfo;

        private TMP_CharacterInfo[] CharacterInfo => text.textInfo.characterInfo;
        private TMP_MeshInfo[] MeshInfo => text.textInfo.meshInfo;
        
        private int meshDecoCount;
        
        public Material meshMaterial;
        public float underlineHeight = 5;
        public float underlineScale = 5;

        private Texture MeshTexture => meshMaterial.mainTexture;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();

            if (decoMesh is null)
            {
                decoMesh = new Mesh();
                decoMeshInfo = new TMP_MeshInfo(decoMesh, 8);
            }

            if (meshHolder is null) throw new Exception("Mesh holder is unassigned");
            canvasRenderer = meshHolder.GetComponent<CanvasRenderer>();
            canvasRenderer.SetMaterial(meshMaterial, MeshTexture);
        }

        private void SetCharColor(int charIndex, Color32 color)
        {
            int meshIndex = CharacterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = CharacterInfo[charIndex].vertexIndex;
            Color32[] vertexColors = MeshInfo[meshIndex].colors32;

            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;

            CharacterInfo[charIndex].color = color;
        }

        private void AddUnderline(int startIndex, int length)
        {
            if (decoMeshInfo.vertices is null)
                decoMeshInfo = new TMP_MeshInfo(decoMesh, meshDecoCount + 1);
            else if (decoMeshInfo.vertices.Length < meshDecoCount * 4 + 1)
                decoMeshInfo.ResizeMeshInfo(Mathf.NextPowerOfTwo(meshDecoCount + 1));

            int first = startIndex;
            int last = startIndex + length - 1;
            
            float wordScale = CharacterInfo[first].scale;

            Vector3 bottomLeft = new(CharacterInfo[first].bottomLeft.x,
                CharacterInfo[first].baseLine - underlineHeight * wordScale, 0);
            Vector3 topLeft = new(bottomLeft.x, CharacterInfo[first].baseLine, 0);
            Vector3 topRight = new(CharacterInfo[last].topRight.x, topLeft.y, 0);
            Vector3 bottomRight = new(topRight.x, bottomLeft.y, 0);

            int vertexIndex = meshDecoCount * 4;
            Vector3[] vertices = decoMeshInfo.vertices;
            vertices[vertexIndex + 0] = bottomLeft;
            vertices[vertexIndex + 1] = topLeft;
            vertices[vertexIndex + 2] = topRight;
            vertices[vertexIndex + 3] = bottomRight;

            Vector2[] uvs = decoMeshInfo.uvs0;
            float underlineLength = Mathf.Abs(topRight.x - bottomLeft.x) / wordScale * underlineScale;
            float underlineTiling = underlineLength / (MeshTexture == null ? underlineLength : MeshTexture.width);
            uvs[vertexIndex + 0] = new Vector2(0, 0);
            uvs[vertexIndex + 1] = new Vector2(0, 1);
            uvs[vertexIndex + 2] = new Vector2(underlineTiling, 0);
            uvs[vertexIndex + 3] = new Vector2(underlineTiling, 1);

            meshDecoCount++;
        }

        public TMPTextFormatter Begin(TMP_Text theText = null)
        {
            if (theText is not null)
                text = theText;

            meshDecoCount = 0;
            
            decoMeshInfo.Clear(true);
            text.ForceMeshUpdate();
            return this;
        }

        public TMPTextFormatter Color(int startIndex, int length, Color32 color)
        {
            for (int i = startIndex; i < startIndex + length; i++)
                SetCharColor(i, color);
            return this;
        }

        public TMPTextFormatter Underline(int startIndex, int length)
        {
            List<(int start, int end)> lines = new();
            float currentBaseLine = CharacterInfo[startIndex].baseLine;
            int currentLineStart = startIndex;
            
            for (int i = startIndex; i < startIndex + length; i++)
            {
                if (!Mathf.Approximately(currentBaseLine, CharacterInfo[i].baseLine))
                {
                    currentBaseLine = CharacterInfo[i].baseLine;
                    lines.Add((currentLineStart, i - 1));
                    currentLineStart = i;
                }
            }
            lines.Add((currentLineStart, startIndex + length - 1));

            foreach ((int start, int end) in lines)
            {
                AddUnderline(start, end - start + 1);
            }
            
            return this;
        }

        public void Apply()
        {
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            decoMesh.vertices = decoMeshInfo.vertices;
            decoMesh.uv = decoMeshInfo.uvs0;
            decoMesh.RecalculateBounds();
            
            canvasRenderer.SetMesh(decoMesh);
            
            // Leave a shibboleth that marks the text as formatted (for checking later, as ForceMeshUpdate() resets characterInfo)
            // Working around TextMesh Pro's stupidity
            if (text.text.Length > 0)
            {
                text.textInfo.characterInfo[0].character = '\xFF';
            }
        }

        private void OnDestroy()
        {
            DestroyImmediate(decoMesh);
        }
    }
}

