using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity
{

    [Serializable]
    public struct AssetPath
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
    {
        [SerializeField]
        private string guid;
        [SerializeField]
        private string assetPath;
#if UNITY_EDITOR
        [SerializeField]
        private UnityEngine.Object target;
#endif

        public AssetPath(string assetGuid, string assetPath)
        {
            this.guid = assetGuid;
            this.assetPath = assetPath;
#if UNITY_EDITOR
            this.target = null;
#endif
        }

        public AssetPath(UnityEngine.Object target)
        {
            guid = null;
            assetPath = null;
#if UNITY_EDITOR
            this.target = target;
            RefreshTarget();
#endif
        }

        public string Guid => guid;

        public string Path
        {
            get
            {
                string value = this.assetPath;
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(guid))
                {
                    value = AssetDatabase.GUIDToAssetPath(guid);
                }
#endif
                return value;
            }
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
                else if (!string.IsNullOrEmpty(assetPath))
                {
                    target = AssetDatabase.LoadMainAssetAtPath(assetPath);
                }
                return target;
#endif
                Debug.LogError("Can't at runtime access AssetPath.Target");
                return null;

            }
        }

        public static implicit operator UnityEngine.Object(AssetPath assetPath)
        {
            return assetPath.Asset;
        }



        public static implicit operator AssetPath(UnityEngine.Object obj)
        {
            return new AssetPath(obj);
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
                assetPath = null;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target, out guid, out long id))
                {
                    assetPath = AssetDatabase.GUIDToAssetPath(guid);
                }
            }
            //target 引用丢失不处理

            //else if (!string.IsNullOrEmpty(guid))
            //{
            //    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            //    if (assetPath != this.assetPath)
            //    {
            //        this.assetPath = assetPath;
            //    }
            //}
        }
#endif
    }


}
