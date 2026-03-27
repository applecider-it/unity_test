using UnityEngine;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 移動床処理
    /// </summary>
    public class MovingPlatformParts
    {
        /// <summary> 移動床の移動量 </summary>
        private Vector3 movingPlatformDeltaPos = Vector3.zero;
        /// <summary> 移動床の移動量有効カウント </summary>
        private int movingPlatformDeltaPosCnt = 0;

        /// <summary> 移動床の移動量 </summary>
        private Vector3 movingPlatformDelta = Vector3.zero;
        /// <summary> 移動床の移動量有効カウント </summary>

        /// <summary>
        /// 移動床プロセス
        /// </summary>
        public void MovingPlatformProccess()
        {
            if (movingPlatformDeltaPosCnt > 0) movingPlatformDeltaPosCnt--;

            movingPlatformDelta = (movingPlatformDeltaPosCnt > 0) ? movingPlatformDeltaPos : Vector3.zero;
        }

        // setter getter

        public Vector3 MovingPlatformDeltaPos
        {
            set
            {
                movingPlatformDeltaPos = value;
                movingPlatformDeltaPosCnt = 5;
            }
        }
        public Vector3 MovingPlatformDelta { get => movingPlatformDelta; }
    }
}