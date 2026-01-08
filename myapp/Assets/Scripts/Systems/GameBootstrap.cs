using UnityEngine;

using Game.ScriptableObjects;

namespace Game.Systems
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnGameStart()
        {
            Debug.Log("ゲーム開始時に1回だけ呼ばれる");

            var config = Resources.Load<GameConfig>("GameConfig");
            Debug.Log(config.maxHp);
        }
    }
}