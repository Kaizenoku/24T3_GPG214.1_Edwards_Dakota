using System.Collections;
using UnityEngine;

namespace DakotaLib
{
    public class StreamingSpriteLoader : StreamingAssetLoader<Sprite>
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private int m_PixelsPerUnit;

        public override void LoadAsset()
        {
            Sprite sprite = FileSystemUtilities.GetSpriteFromFile(m_AssetFilePath, PixelsPerUnit: m_PixelsPerUnit);

            // If sprite is null...
            if (sprite == null)
            {
                return;
            }

            // Set sprite of sprite renderer
            m_SpriteRenderer.sprite = sprite;
        }

        public override IEnumerator LoadAssetAsync()
        {
            coroutineRunning = true;

            yield return FileSystemUtilities.GetSpriteFromFileAsync(OnAssetLoaded, m_AssetFilePath);

            coroutineRunning = false;
        }

        protected override void OnAssetLoaded(Sprite Sprite)
        {
            // If sprite is null...
            if (Sprite == null)
            {
                return;
            }

            // Set sprite of sprite renderer
            m_SpriteRenderer.sprite = Sprite;
        }

        protected override bool RequirementsMet()
        {
            bool output = true;

            if (m_SpriteRenderer == null)
            {
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
                output = m_SpriteRenderer != null;
            }

            return output;
        }
    }
}