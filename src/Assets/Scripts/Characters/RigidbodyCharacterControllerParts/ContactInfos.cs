using System.Collections.Generic;
using UnityEngine;

using Game.Utils;
using Game.Stages;
using Game.Commons;

namespace Game.Characters.RigidbodyCharacterControllerParts
{
    /// <summary>
    /// 接触判定結果情報
    /// </summary>
    class ContactInfo
    {
        public bool valid;
        public Vector3 normal;
    }

    /// <summary>
    /// 接触判定結果情報をまとめたクラス
    /// </summary>
    public class ContactInfos
    {
        private Dictionary<Collider, ContactInfo> contacts = new Dictionary<Collider, ContactInfo>();

        /// <summary>
        /// 作成
        /// </summary>
        public void insert(Collider collider, bool result, Vector3 normal)
        {
            var info = new ContactInfo
            {
                valid = result,
                normal = normal
            };

            contacts[collider] = info;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void update(Collider collider, bool result, Vector3 normal)
        {
            if (contacts.TryGetValue(collider, out var info))
            {
                info.valid = result;
                info.normal = normal;
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        public void delete(Collider collider)
        {
            contacts.Remove(collider);
        }

        /// <summary>
        /// 消滅するデータが残るので消す処理
        /// </summary>
        public void CleanupDestroyedData()
        {
            if (contacts.Count == 0) return;

            var removeList = new List<Collider>();

            foreach (var col in contacts.Keys)
            {
                if (col == null)
                    removeList.Add(col);
            }

            foreach (var col in removeList)
                contacts.Remove(col);
        }

        // getter

        public bool Valid
        {
            get
            {
                foreach (var info in contacts.Values)
                {
                    if (info.valid) return true;
                }
                return false;
            }
        }
        public Vector3 ContactNormal
        {
            get
            {
                foreach (var info in contacts.Values)
                {
                    if (info.valid) return info.normal;
                }
                return Vector3.up;
            }
        }
        public int Count { get => contacts.Count; }
    }
}