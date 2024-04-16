using UnityEngine;

namespace Inferno_Cascade
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;

        private int locomotion = Animator.StringToHash("Locomotion");
        private int attack = Animator.StringToHash("Attack");
        private int jump = Animator.StringToHash("Jump");

        #region UnityEvents
        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
        }
        #endregion
    }
}
