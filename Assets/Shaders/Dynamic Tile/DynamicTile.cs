using System;
using NineEightOhThree.VirtualCPU;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace NineEightOhThree.Shaders
{
    public class DynamicTile : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
       
        private MaterialPropertyBlock block;
 
        public ushort graphicsPointer;

        private Texture2D texture;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int GraphicsData = Shader.PropertyToID("_GraphicsData");

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            block = new MaterialPropertyBlock();

            texture = new Texture2D(8, 8, TextureFormat.RGB24, false);
        }

        private void FixedUpdate()
        {
            UpdateTile();
        }

        private void UpdateTile()
        {
            var data = CPU.Instance.Memory.ReadBlock(graphicsPointer, 8);
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int mask = 128 >> col;
                    bool on = (data[row] & mask) != 0;
                    texture.SetPixel(col, row, on ? Color.white : Color.black);
                }
            }

            spriteRenderer.GetPropertyBlock(block);
            block.SetTexture(MainTex, texture);
            spriteRenderer.SetPropertyBlock(block);
        }
    }
}