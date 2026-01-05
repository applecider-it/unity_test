using UnityEngine;

using Game.System;

namespace Game.Util
{
    /// <summary>
    /// データ管理用ユーティリティ
    /// </summary>

    public class DataUtil
    {
        /// <summary>
        /// 共通データを返す
        /// </summary>
        public static CommonData getCommonData()
        {
            GameObject obj = GameObject.Find("Script");
            CommonData cd = obj.GetComponent<CommonData>();
            return cd;
        }
    }
}