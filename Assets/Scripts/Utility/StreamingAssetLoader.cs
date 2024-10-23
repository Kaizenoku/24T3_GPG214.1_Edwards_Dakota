using UnityEngine;

namespace DakotaUtility
{
    // Abstract class that other "Loader" classes inherit from to load assets on Awake from "StreamingAssets" folder
    public abstract class StreamingAssetLoader : MonoBehaviour
    {
    #if UNITY_EDITOR
        // Object property is only used in the editor
        [SerializeField] protected Object m_StreamingAsset;
    #endif
        [SerializeField] protected string m_AssetFilePath;

        protected virtual void Awake()
        {
            LoadAsset(m_AssetFilePath);
        }

        // Overwritten by derived classes
        protected abstract void LoadAsset(string AssetFilePath);
    }
}
