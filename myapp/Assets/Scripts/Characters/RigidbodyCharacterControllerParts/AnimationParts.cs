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
        public void SetAnimator(bool noMove, CharacterActionType actionType)
        {
            switch (actionType)
            {
                case CharacterActionType.Ground:
                    animator.SetBool("move", !noMove);
                    break;

                default:
                    animator.SetBool("move", false);
                    break;
            }
        }
    }
}