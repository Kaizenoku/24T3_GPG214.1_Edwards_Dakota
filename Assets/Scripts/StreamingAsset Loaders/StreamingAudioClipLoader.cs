using UnityEngine;

namespace DakotaLib
{
    // Requires an audio source to load the clip into
    public class StreamingAudioClipLoader : StreamingAssetLoader
    {
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private int m_Channels = 1;
        [SerializeField] private int m_Frequency = 44100;
        [SerializeField] private StreamingAssetUtilities.AudioBitDepth m_BitDepth = StreamingAssetUtilities.AudioBitDepth.Sixteen;

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
            AudioClip audioClip = StreamingAssetUtilities.GetAudioClipFromFile(
                AssetFilePath,
                Channels: m_Channels,
                Frequency: m_Frequency,
                BitDepth: m_BitDepth
            );

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