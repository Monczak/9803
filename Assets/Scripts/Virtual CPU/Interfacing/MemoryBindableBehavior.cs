using NineEightOhThree.VirtualCPU;
using NineEightOhThree.VirtualCPU.Interfacing;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class MemoryBindableBehavior : MonoBehaviour
{
    public Memory memory;

    public Dictionary<Bindable, string> bindableValues;
    public Dictionary<FieldInfo, ushort> addresses;

    public List<FieldInfo> testFields;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        foreach (FieldInfo i in testFields)
            Debug.Log($"Test {i.Name}");
    }
}
