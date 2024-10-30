using System.Collections;
using UnityEngine;

namespace DakotaLib
{
    public abstract class StreamingAssetLoaderWrapper { }
    
    [DefaultExecutionOrder(-3)]
    public abstract class StreamingAssetLoader<T> : MonoBehaviour
    {
    #if UNITY_EDITOR
        // Object property is only used in the editor
        [SerializeField] protected Object m_StreamingAsset;
    #endif

        [SerializeField] protected string m_AssetFilePath;
        [SerializeField] protected bool m_LoadOnAwake = true;
        [SerializeField] protected bool m_LoadAsynchronously = false;

        [HideInInspector] public bool coroutineRunning;

        public abstract void LoadAsset();
        public abstract IEnumerator LoadAssetAsync();
        protected abstract void OnAssetLoaded(T Asset);
        protected abstract bool RequirementsMet();

        protected void Awake()
        {
            if (!m_LoadOnAwake) { return ; }

            if (!RequirementsMet())
            {
                Debug.LogError("Not all required components could be found!");
                return;
            }

            if (!m_LoadAsynchronously)
            {
                LoadAsset();
            }
            else
            {
                if (!coroutineRunning)
                {
                    StartCoroutine(LoadAssetAsync());
                }
            }
        }
    }
}
