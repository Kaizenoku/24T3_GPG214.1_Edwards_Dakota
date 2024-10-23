using UnityEngine;

namespace DakotaUtility
{
    // Requires a renderer to load the image into
    [RequireComponent(typeof(Renderer))]
    public class StreamingTexture2DLoader : StreamingAssetLoader
    {
        [SerializeField] private Renderer m_Renderer;
        
        protected override void Awake()
        {
            // If no renderer provided...
            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<Renderer>();
            }

            base.Awake();
        }

        protected override void LoadAsset(string AssetFilePath)
        {
            Texture2D texture = StreamingAssetUtilities.Get2DTextureFromFile(AssetFilePath);

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