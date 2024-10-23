using UnityEngine;

namespace DakotaUtility
{
    // Requires an audio source to load the clip into
    [RequireComponent(typeof(AudioSource))]
    public class StreamingAudioClipLoader : StreamingAssetLoader
    {
        [SerializeField] private AudioSource m_AudioSource;

        protected override void Awake()
        {
            // If no audio source provided...
            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
            }

            base.Awake();
        }

        protected override void LoadAsset(string AssetFilePath)
        {
            AudioClip audioClip = StreamingAssetUtilities.GetAudioClipFromFile(AssetFilePath);

            // If audio clip is null...
            if (audioClip == null)
            {
                return;
            }

            // Set audio clip of audio source
            m_AudioSource.clip = audioClip;
        }
    }
}