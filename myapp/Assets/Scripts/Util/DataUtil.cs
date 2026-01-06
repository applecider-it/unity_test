using UnityEngine;

using Game.System;

namespace Game.Util
{
    /// <summary>
    /// データ管理用ユーティリティ
    /// </summary>
    public class DataUtil
    {
        private static CommonData cd = null;

        /// <summary>
        /// 共通データを返す
        /// </summary>
        public static CommonData getCommonData()
        {
            if (cd == null)
            {
                GameObject obj = GameObject.Find("Script");
                cd = obj.GetComponent<CommonData>();
            }
            return cd;
        }
    }
}