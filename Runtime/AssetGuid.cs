using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine
{

    [Serializable]
    public struct AssetGuid
    {
        [SerializeField]
        private string guid;

        public AssetGuid(string guid)
        {
            this.guid = guid;
#if UNITY_EDITOR
            this.target = null;
#endif
        }


        public string Guid => guid;

#if UNITY_EDITOR
        [NonSerialized]
        private UnityEngine.Object target;
        public AssetGuid(UnityEngine.Object target)
        {
            guid = null;
            this.target = target;
            if (target)
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target, out guid, out long id))
                {

                }
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

                Debug.LogError("Can't at runtime access AssetGuid.Target");
                return null;
            }
        }

#endif

    }


}
