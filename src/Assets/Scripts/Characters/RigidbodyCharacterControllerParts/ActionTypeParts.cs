using UnityEngine;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// アクションタイプ処理
    /// </summary>
    public class ActionTypeParts
    {
        /// <summary>
        /// アクションタイプ取得
        /// </summary>
        public CharacterActionType GetActionType(bool isGrounded, bool inWaterBuoyancy, bool isHang)
        {
            CharacterActionType type = CharacterActionType.Undefined;

            if (inWaterBuoyancy)
            {
                // 水中にいるとき

                type = CharacterActionType.Water;
            }
            else
            {
                // 水中にいないとき

                if (isGrounded)
                {
                    // 地面にいるとき

                    type = CharacterActionType.Ground;
                }
                else
                {
                    // 地面にいないとき

                    if (isHang)
                    {
                        // つかまっているとき

                        type = CharacterActionType.Hang;
                    }
                    else
                    {
                        // つかまっていないとき

                        type = CharacterActionType.Air;
                    }
                }
            }

            return type;
        }
    }
}