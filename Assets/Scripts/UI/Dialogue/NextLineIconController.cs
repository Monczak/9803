using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.Math;
using TMPro;
using UnityEngine;

public class NextLineIconController : MonoBehaviour
{
    public string characters;
    public float charSelect;
    public bool visible;

    private TMP_Text text;
    private Animator animator;
    
    private static readonly int AnimShow = Animator.StringToHash("Show");

    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        text = GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.SetBool(AnimShow, true);
    }

    public void Hide()
    {
        animator.SetBool(AnimShow, false);
    }
    
    public void HideImmediate()
    {
        text.text = " ";
        if (gameObject.activeInHierarchy) animator.Play("Hidden");
    }

    // Update is called once per frame
    private void Update()
    {
        if (visible)
        {
            int charIndex = (int)(charSelect * characters.Length).Constrain(0, characters.Length - 1);
            text.text = characters[charIndex].ToString();
        }
        else
        {
            text.text = " ";
        }
    }
}
