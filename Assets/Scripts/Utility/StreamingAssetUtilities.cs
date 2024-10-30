using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DakotaLib
{
    public static class StreamingAssetUtilities
    {
        private static string m_StreamingAssetsPath = Application.streamingAssetsPath;
        public enum AudioBitDepth { Sixteen, TwentyFour, ThirtyTwo };

        /// <summary>
        /// Returns an AudioClip loaded from the file at the specified Streaming Assets path.
        /// If no valid file, returns null.
        /// </summary>
        /// <param name="RelativeFilePath">
        /// Requires a filepath (including file name) relative to the StreamingAssets folder.
        /// Do NOT include "Assets/StreamingAssets/".
        /// </param>
        /// <param name="Channels">
        /// The number of audio channels (1 for mono, 2 for stereo, etc.).
        /// Defaults to mono.
        /// </param>
        /// <param name="Frequency">
        /// The audio frequency (44100 for CD quality, 48000 for DVD quality, etc.).
        /// Defaults to CD quality.
        /// </param>
        /// <param name="BitDepth">
        /// The BitDepth of Audio File
        /// Defaults to 16 bits.
        /// </param>
        /// <param name="StreamAudio">
        /// Whether or not to stream the audio in or load all at once.
        /// Defaults to loading in all at once.
        /// </param>
        public static AudioClip GetAudioClipFromStreamingAssets(string RelativeFilePath, int Channels = 1, int Frequency = 44100, AudioBitDepth BitDepth = AudioBitDepth.Sixteen, bool StreamAudio = false)
        {
            string fullFilePath = Path.Combine(m_StreamingAssetsPath, RelativeFilePath);

            // If file doesn't exist...
            if (!File.Exists(fullFilePath))
            {
                Debug.LogError(string.Format("File at [{0}] does not exist.", fullFilePath));
                return null;
            }

            // Get the bytes of the file
            byte[] fileBytes = File.ReadAllBytes(fullFilePath);

            int divider = 2;

            switch (BitDepth)
            {
                case AudioBitDepth.Sixteen:
                    divider = 2;
                    break;
                case AudioBitDepth.TwentyFour:
                    divider = 3;
                    break;
                case AudioBitDepth.ThirtyTwo:
                    divider = 4;
                    break;
            }

            // Convert every 'divider' bytes (8 bits) into one 'divider * 8'-bit integer
            float[] fileData = new float[fileBytes.Length / divider];
            for (int i = 0; i < fileData.Length; i++)
            {
                // Converting two bytes to a 16-bit integer
                short bitValue = System.BitConverter.ToInt16(fileBytes, i * divider);

                // Normalise the value
                fileData[i] = bitValue / 32768.0f;
            }

            string fileName = Path.GetFileName(fullFilePath);

            // Create our audio clip
            AudioClip audioClip = AudioClip.Create(fileName, fileData.Length, Channels, Frequency, StreamAudio);
            bool clipLoaded = audioClip.SetData(fileData, 0);

            // If clip didn't load successfully...
            if (!clipLoaded)
            {
                Debug.LogError(string.Format("File at [{0}] is not a valid audio file.", fullFilePath));
                return null;
            }

            return audioClip;
        }

    }
}