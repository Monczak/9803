using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.Managers;
using UnityEngine;
using TMPro;

namespace NineEightOhThree.UI.CodeEditor
{
    public class TestCodeEditorController : MonoBehaviour
    {
        private TMP_InputField inputField;

        private string currentCode;
        private float timeToAssembly;

        public float assemblerCooldown = 1;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
            
            inputField.caretWidth = 6;
            inputField.onValueChanged.AddListener(OnInputChanged);

            timeToAssembly = 0;
        }

        private void OnInputChanged(string input)
        {
            currentCode = input;
            timeToAssembly = assemblerCooldown;
        }

        void Update()
        {
            if (timeToAssembly > 0)
            {
                timeToAssembly -= Time.deltaTime;
                if (timeToAssembly <= 0)
                {
                    AssembleCode();
                }
            }
        }
        
        
        private void AssembleCode()
        {
            AssemblerInterface.ScheduleAssembly(currentCode, null);
        }
    }
}

