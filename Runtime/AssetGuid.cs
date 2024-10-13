using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity
{

    [Serializable]
    public struct AssetGuid
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
    {
        [SerializeField]
        private string guid;

#if UNITY_EDITOR
        [SerializeField]
        private UnityEngine.Object target;
#endif
        public string Guid => guid;

        public AssetGuid(string guid)
        {
            this.guid = guid;
#if UNITY_EDITOR
            this.target = null;
#endif
        }

        public AssetGuid(UnityEngine.Object target)
        {
            guid = null;
#if UNITY_EDITOR
            this.target = target;
            RefreshTarget();
#endif
        }
        public UnityEngine.Object Asset
        {
            get => Target;
        }
        public UnityEngine.Object Target
        {
            get
            {
#if UNITY_EDITOR
                if (target)
                    return target;
                if (!string.IsNullOrEmpty(guid))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        target = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    }
                }
                return target;
                
#endif

                Debug.LogError("Can't at runtime access AssetGuid.Target");
                return null;
            }
        }

#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
            RefreshTarget();
        }

        public void OnAfterDeserialize()
        {

        }

        void RefreshTarget()
        {
            var target = Target;
            if (target)
            {
                guid = null;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target, out guid, out long id))
                {

                }
            }
            //target 引用丢失不处理
        }
#endif

        public static implicit operator UnityEngine.Object(AssetGuid assetGuid)
        {
            return assetGuid.Asset;
        }




        public static implicit operator AssetGuid(UnityEngine.Object obj)
        {
            return new AssetGuid(obj);
        }
    }


}
