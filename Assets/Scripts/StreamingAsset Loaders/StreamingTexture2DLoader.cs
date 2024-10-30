using System.Collections;
using UnityEngine;

namespace DakotaLib
{
    public class StreamingTexture2DLoader : StreamingAssetLoader<Texture2D>
    {
        [SerializeField] private Renderer m_Renderer;

        public override void LoadAsset()
        {
            Texture2D texture = FileSystemUtilities.GetTexture2DFromFile(m_AssetFilePath);

            // If texture is null...
            if (texture == null)
            {
                return;
            }

            // Set main texture of renderer
            m_Renderer.material.mainTexture = texture;
        }

        public override IEnumerator LoadAssetAsync()
        {
            coroutineRunning = true;

            yield return FileSystemUtilities.GetTexture2DFromFileAsync(OnAssetLoaded, m_AssetFilePath);

            coroutineRunning = false;
        }

        protected override void OnAssetLoaded(Texture2D Texture)
        {
            // If texture is null...
            if (Texture == null)
            {
                return;
            }

            // Set main texture of renderer
            m_Renderer.material.mainTexture = Texture;
        }

        protected override bool RequirementsMet()
        {
            bool output = true;

            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<Renderer>();
                output = m_Renderer != null;
            }

            return output;
        }
    }
}