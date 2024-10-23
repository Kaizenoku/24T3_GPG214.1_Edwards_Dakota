using UnityEngine;

namespace DakotaUtility
{
    // Requires a rendered to load the image into
    [RequireComponent(typeof(Renderer))]
    public class StreamingTextureLoader : StreamingAssetLoader
    {
        protected override void LoadAsset(string AssetFilePath)
        {
            StreamingAssetUtilities.GetSpriteFromFile(AssetFilePath);
        }
    }
}

