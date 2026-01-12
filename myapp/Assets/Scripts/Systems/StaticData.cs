using UnityEngine;

using Game.Objects;

namespace Game.Systems
{
    public class StaticData
    {
        // シーン接続管理
        private static SceneConnectorInfoClass sceneConnectorInfo = null;

        // getter setter

        public static SceneConnectorInfoClass SceneConnectorInfo { get => sceneConnectorInfo; set => sceneConnectorInfo = value; }
    }
}