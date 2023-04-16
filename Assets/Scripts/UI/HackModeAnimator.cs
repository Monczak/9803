using NineEightOhThree.Managers;
using UnityEngine;

namespace NineEightOhThree.UI
{
    [RequireComponent(typeof(Animator))]
    public class HackModeAnimator : MonoBehaviour
    {
        private Animator animator;

        private UIControls controls;
        
        private static readonly int Disable = Animator.StringToHash("Disable");
        private static readonly int Enable = Animator.StringToHash("Enable");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            
            GameManager.Instance.OnHackModeToggled += OnHackModeToggled;
        }

        private void OnHackModeToggled(object sender, bool hackModeEnabled)
        {
            animator.SetTrigger(hackModeEnabled ? Enable : Disable);
        }
    }
}

