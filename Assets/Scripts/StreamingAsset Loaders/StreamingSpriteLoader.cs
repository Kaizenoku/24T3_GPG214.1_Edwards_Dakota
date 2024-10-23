using UnityEngine;

namespace DakotaUtility
{
    // Requires a sprite renderer to load the image into
    [RequireComponent(typeof(SpriteRenderer))]
    public class StreamingSpriteLoader : StreamingAssetLoader
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;

        protected override void Awake()
        {
            // If no renderer provided...
            if (m_SpriteRenderer == null)
            {
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
            }

            base.Awake();
        }

        protected override void LoadAsset(string AssetFilePath)
        {
            Sprite sprite = StreamingAssetUtilities.GetSpriteFromFile(AssetFilePath);

            // If texture is null...
            if (sprite == null)
            {
                return;
            }

            // Set sprite of sprite renderer
            m_SpriteRenderer.sprite = sprite;
        }
    }
}