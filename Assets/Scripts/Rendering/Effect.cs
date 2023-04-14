using System;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class Effect
    {
        public Material material;
        public bool enabled;
        public float strength;

        private Material matCopy;

        public Material MatCopy => matCopy ? matCopy : matCopy = new Material(material);
    }
}