using UnityEngine;

namespace Game.Systems
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnGameStart()
        {
            Debug.Log("ゲーム開始時に1回だけ呼ばれる");

        }
    }
}