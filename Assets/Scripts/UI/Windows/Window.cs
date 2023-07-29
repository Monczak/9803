using System;
using UnityEngine;

namespace NineEightOhThree.UI.Windows
{
    [CreateAssetMenu(menuName = "UI/Window")]
    public class Window : ScriptableObject
    {
        [field: SerializeField] public GameObject TabPrefab { get; private set; }
        [field: SerializeField] public GameObject ContentPrefab { get; private set; }
    }
}