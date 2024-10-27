using UnityEngine;

namespace DakotaLib
{
    public class StreamingAudioClipLoader : StreamingAssetLoader
    {
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private int m_Channels = 1;
        [SerializeField] private int m_Frequency = 44100;
        [SerializeField] private StreamingAssetUtilities.AudioBitDepth m_BitDepth = StreamingAssetUtilities.AudioBitDepth.Sixteen;

        private void Awake()
        {
            // If no audio source provided...
            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
            }

            if (m_AudioSource != null)
            {
                LoadAsset();
            }
        }

        private void LoadAsset()
        {
            AudioClip audioClip = StreamingAssetUtilities.GetAudioClipFromFile(
                m_AssetFilePath,
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