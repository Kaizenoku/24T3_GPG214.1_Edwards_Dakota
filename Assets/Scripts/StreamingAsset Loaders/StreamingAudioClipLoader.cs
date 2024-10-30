using System.Collections;
using UnityEngine;

namespace DakotaLib
{
    public class StreamingAudioClipLoader : StreamingAssetLoader<AudioClip>
    {
        [SerializeField] private AudioSource m_AudioSource;

        // Deprecated, now only using Asynchronous Loading
        [Header("Only required for non asynchronous loading (NOT RECOMMENDED)")]
        [SerializeField] private int m_Channels = 1;
        [SerializeField] private int m_Frequency = 44100;
        [SerializeField] private int m_BitDepth = 16;

        public override void LoadAsset()
        {
            Debug.LogWarning("Not recommended! Use LoadAssetAsync instead.");
            
            AudioClip audioClip = FileSystemUtilities.GetAudioClipFromFile(
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
        
        public override IEnumerator LoadAssetAsync()
        {
            assetLoading = true;

            yield return FileSystemUtilities.GetAudioClipFromFileAsync(OnAssetLoaded, m_AssetFilePath);

            assetLoading = false;
        }

        protected override void OnAssetLoaded(AudioClip AudioClip)
        {
            // If audio clip is null...
            if (AudioClip == null)
            {
                return;
            }

            // Set audio clip of audio source
            m_AudioSource.clip = AudioClip;
        }

        protected override bool RequirementsMet()
        {
            bool output = true;

            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
                output = m_AudioSource != null;
            }

            return output;
        }
    }
}