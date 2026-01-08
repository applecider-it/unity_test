using UnityEngine;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// アニメーション処理
    /// </summary>
    public class AnimationParts
    {
        private Animator animator;

        // コンストラクタ
        public AnimationParts(Animator argAnimator)
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