using UnityEngine;

namespace DakotaLib
{
    // Requires a renderer to load the image into
    [RequireComponent(typeof(Renderer))]
    public class StreamingTexture2DLoader : StreamingAssetLoader
    {
        [SerializeField] private Renderer m_Renderer;
        
        private void Awake()
        {
            // If no renderer provided...
            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<Renderer>();
            }

            if (m_Renderer != null)
            {
                LoadAsset();
            }
        }

        private void LoadAsset()
        {
            Texture2D texture = StreamingAssetUtilities.Get2DTextureFromFile(m_AssetFilePath);

            // If texture is null...
            if (texture == null)
            {
                return;
            }

            // Set main texture of renderer
            m_Renderer.material.mainTexture = texture;
        }
    }
}