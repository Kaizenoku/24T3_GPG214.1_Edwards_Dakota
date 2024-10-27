using UnityEngine;

namespace DakotaLib
{
    public class StreamingSpriteLoader : StreamingAssetLoader
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private int m_PixelsPerUnit;

        private void Awake()
        {
            // If no sprite renderer provided...
            if (m_SpriteRenderer == null)
            {
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (m_SpriteRenderer != null)
            {
                LoadAsset();
            }
        }

        private void LoadAsset()
        {
            Sprite sprite = StreamingAssetUtilities.GetSpriteFromFile(m_AssetFilePath, PixelsPerUnit: m_PixelsPerUnit);

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