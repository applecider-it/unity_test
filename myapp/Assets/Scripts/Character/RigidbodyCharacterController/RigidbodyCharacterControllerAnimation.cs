using UnityEngine;

namespace Game.Character.RigidbodyCharacterControllerParts
{
    public class RigidbodyCharacterControllerAnimation
    {
        private Animator animator;

        // コンストラクタ
        public RigidbodyCharacterControllerAnimation(Animator argAnimator)
        {
            animator = argAnimator;
        }

        /// <summary>
        /// アニメーター指定
        /// </summary>
        public void SetAnimator(bool noMove, bool isGrounded)
        {
            if (noMove)
            {
                // 入力なしの時

                animator.SetBool("move", false);
            }
            else
            {
                // 入力ありの時

                animator.SetBool("move", isGrounded);
            }
        }
    }
}