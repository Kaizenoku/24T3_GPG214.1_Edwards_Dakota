using UnityEngine;

namespace DakotaLib
{
    // Requires a sprite renderer to load the image into
    public class StreamingSpriteLoader : StreamingAssetLoader
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private int m_PixelsPerUnit;

        protected override void Awake()
        {
            // If no sprite renderer provided...
            if (m_SpriteRenderer == null)
            {
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
            }

            base.Awake();
        }

        protected override void LoadAsset(string AssetFilePath)
        {
            Sprite sprite = StreamingAssetUtilities.GetSpriteFromFile(AssetFilePath, PixelsPerUnit: m_PixelsPerUnit);

            // If sprite is null...
            if (sprite == null)
            {
                return;
            }

            // Set sprite of sprite renderer
            m_SpriteRenderer.sprite = sprite;
        }
    }
}