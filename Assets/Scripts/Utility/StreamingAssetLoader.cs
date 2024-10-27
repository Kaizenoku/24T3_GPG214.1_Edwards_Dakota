using UnityEngine;

namespace DakotaLib
{
    // Abstract class used to add Drag and Drop capabilities to add StreamingAsset filepaths
    public abstract class StreamingAssetLoader : MonoBehaviour
    {
    #if UNITY_EDITOR
        // Object property is only used in the editor
        [SerializeField] protected Object m_StreamingAsset;
    #endif
        [SerializeField] protected string m_AssetFilePath;
    }
}
