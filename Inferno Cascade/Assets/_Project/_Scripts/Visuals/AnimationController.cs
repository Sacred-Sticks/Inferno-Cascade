using Kickstarter.Observer;
using System.Collections;
using UnityEngine;

namespace Inferno_Cascade
{
    public class AnimationController : MonoBehaviour, IObserver<PlayerMovement.PlayerMovementNotification>
    {
        private Animator animator;

        private int locomotion = Animator.StringToHash("Locomotion");
        private int attack = Animator.StringToHash("Attack");
        private int jump = Animator.StringToHash("Jump");

        private int velocityX = Animator.StringToHash("VelocityX");
        private int velocityY = Animator.StringToHash("VelocityY");

        private const float transitionDuration = 0.25f;

        #region UnityEvents
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            GetComponent<PlayerMovement>()?.AddObserver(this);
        }
        #endregion

        public void Locomotion()
        {
            animator.Play(locomotion, 0, transitionDuration);
        }

        public void Attack()
        {
            StartCoroutine(Attacking());
        }

        public void Jump()
        {
            animator.Play(jump, 0, transitionDuration);
        }

        public void SetDirection(Vector2 direction)
        {
            animator.SetFloat(velocityX, direction.x);
            animator.SetFloat(velocityY, direction.y);
        }

        private IEnumerator Attacking()
        {
            animator.SetLayerWeight(1, 1);
            animator.Play(attack, 1, 0);
            float clipDuration = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
            yield return new WaitForSeconds(clipDuration);
            float strength = 1;
            for (int i = 0; i < 10; i++)
            {
                strength -= 0.1f;
                animator.SetLayerWeight(1, strength);
                yield return new WaitForSeconds(0.02f);
            }
        }

        #region Observations
        public void OnNotify(PlayerMovement.PlayerMovementNotification argument)
        {
            animator.SetFloat(velocityX, argument.Direction.x);
            animator.SetFloat(velocityY, argument.Direction.y);
        }
        #endregion
    }
}
