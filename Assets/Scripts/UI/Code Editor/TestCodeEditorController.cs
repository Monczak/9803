using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.Managers;
using NineEightOhThree.Utilities;
using NineEightOhThree.VirtualCPU;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

namespace NineEightOhThree.UI.CodeEditor
{
    [RequireComponent(typeof(TMPTextFormatter))]
    public class TestCodeEditorController : MonoBehaviour
    {
        private TMP_InputField inputField;
        private TMPTextFormatter formatter;

        private string currentCode;
        private float timeToAssembly;

        public float assemblerCooldown = 1;

        private bool formatText;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
            formatter = GetComponent<TMPTextFormatter>();
            
            inputField.caretWidth = 6;
            inputField.onValueChanged.AddListener(OnInputChanged);

            timeToAssembly = 0;
            
            formatText = false;
        }

        private void OnInputChanged(string input)
        {
            currentCode = input;
            timeToAssembly = assemblerCooldown;
            formatText = true;
        }

        private void Update()
        {
            if (timeToAssembly > 0)
            {
                timeToAssembly -= Time.deltaTime;
                if (timeToAssembly <= 0)
                {
                    AssembleCode();
                }
            }

            if (formatText)
            {
                FormatText();
                formatText = false;
            }
        }

        private void FormatText()
        {
            formatter.Begin(inputField.textComponent);

            for (int i = 0; i < inputField.text.Length; i++)
            {
                formatter.Color(i, 1,
                    new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256),
                        255));
            }

            if (inputField.text.Length > 2)
                formatter.Underline(0, 2);

            if (inputField.text.Length > 10)
                formatter.Underline(10, inputField.text.Length - 10);
            
            formatter.Flush();
        }
        
        
        private void AssembleCode()
        {
            AssemblerInterface.ScheduleAssembly(currentCode, CPU.Instance.WriteCode);
        }
    }
}

