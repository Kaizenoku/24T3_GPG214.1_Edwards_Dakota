using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace DakotaLib
{
    public static class FileSystemUtilities
    {
        private static string m_DataPath = Application.dataPath;
        private static string m_PersistentDataPath = Application.persistentDataPath;
        private static string m_StreamingAssetsPath = Application.streamingAssetsPath;
        private static string m_TemporaryCachePath = Application.temporaryCachePath;

        #region Get assets from file (using ByteData)
        // Returns the text contents from a file
        public static string GetTextFromFile(string FilePath)
        {
            // If file doesn't exist...
            if (!File.Exists(FilePath))
            {
                Debug.LogError($"File does not exist!\n[{FilePath}]");
                return null;
            }

            string contents = string.Empty;

            // Try to read text from file
            try
            {
                contents = File.ReadAllText(FilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"File invalid!\n[{FilePath}]\n{ex.Message}");
                return null;
            }

            // If contents are null...
            if (contents == null)
            {
                Debug.LogError($"File invalid!\n[{FilePath}]");
                return null;
            }

            // If contents are empty...
            if (contents == string.Empty)
            {
                Debug.LogWarning($"File is empty!\n[{FilePath}]");
            }

            return contents;
        }

        // Returns a Texture2D from a file
        public static Texture2D GetTexture2DFromFile(string FilePath)
        {
            // If file doesn't exist...
            if (!File.Exists(FilePath))
            {
                Debug.LogError($"File does not exist!\n[{FilePath}]");
                return null;
            }

            // Get the bytes of the file
            byte[] fileBytes = File.ReadAllBytes(FilePath);

            // Create a blank texture
            Texture2D texture = new Texture2D(0, 0);

            // Turn byte data into image
            bool imageLoaded = texture.LoadImage(fileBytes);

            // If image didn't load successfully...
            if (!imageLoaded)
            {
                Debug.LogError($"File is not a valid image!\n[{FilePath}]");
                return null;
            }

            return texture;
        }

        // Returns a Sprite from a file
        public static Sprite GetSpriteFromFile(string FilePath, int PixelsPerUnit = 100)
        {
            // If file doesn't exist...
            if (!File.Exists(FilePath))
            {
                Debug.LogError($"File does not exist!\n[{FilePath}]");
                return null;
            }

            // Get the bytes of the file
            byte[] fileBytes = File.ReadAllBytes(FilePath);

            // Create a blank texture
            Texture2D texture = new Texture2D(0, 0);

            // Turn byte data into image
            bool imageLoaded = texture.LoadImage(fileBytes);

            // If texture is null...
            if (!imageLoaded)
            {
                Debug.LogError($"File is not a valid image!\n[{FilePath}]");
                return null;
            }

            // Create a new sprite using the texture width / height, and a centered pivot point
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);

            // If sprite is null...
            if (sprite == null)
            {
                Debug.LogError($"File could not be converted to a sprite!\n[{FilePath}]");
                return null;
            }

            return sprite;
        }

        // DEPRECATED! Use GetAudioClipFromFileAsync instead
        // Returns an AudioClip from a file (Only works with .wav files)
        public static AudioClip GetAudioClipFromFile(string FilePath, int Channels = 1, int Frequency = 44100, int BitDepth = 16)
        {
            // If file doesn't exist...
            if (!File.Exists(FilePath))
            {
                Debug.LogError($"File does not exist!\n[{FilePath}]");
                return null;
            }

            // Get the bytes of the file
            byte[] fileBytes = File.ReadAllBytes(FilePath);

            // Check the BitDepth is divisible by 8
            if (BitDepth % 8 != 0)
            {
                Debug.LogWarning($"Bit depth [{BitDepth}] is not divisible by 8! Using bit depth of 16.");
            }

            int divider = BitDepth / 8;

            // Convert every 'divider' bytes (8 bits) into one 'divider * 8'-bit integer
            float[] fileData = new float[fileBytes.Length / divider];
            for (int i = 0; i < fileData.Length; i++)
            {
                // Converting two bytes to a 16-bit integer (maybe need different bit converter methods for different bit depths?)
                short bitValue = System.BitConverter.ToInt16(fileBytes, i * divider);

                // Normalise the value
                fileData[i] = bitValue / 32768.0f;
            }

            string fileName = Path.GetFileName(FilePath);

            // Create our audio clip
            AudioClip audioClip = AudioClip.Create(fileName, fileData.Length, Channels, Frequency, false);
            bool clipLoaded = audioClip.SetData(fileData, 0);

            // If clip didn't load successfully...
            if (!clipLoaded)
            {
                Debug.LogError($"File is not a valid audio file!\n[{FilePath}]");
                return null;
            }

            return audioClip;
        }

        // Returns an AssetBundle from a file
        public static AssetBundle GetAssetBundleFromFile(string FilePath)
        {
            // If file doesn't exist...
            if (!File.Exists(FilePath))
            {
                Debug.LogError($"File does not exist!\n[{FilePath}]");
                return null;
            }

            // Load asset bundle
            AssetBundle assetBundle = AssetBundle.LoadFromFile(FilePath);

            // If asset bundle didn't load successfully...
            if (assetBundle == null)
            {
                Debug.LogError($"File is not a valid asset bundle!\n[{FilePath}]");
                return null;
            }

            return assetBundle;
        }
        #endregion

        #region Get assets from file or web asynchronously (using WebRequests)
        // Asynchronously returns the text contents from a file on the computer or uploaded to the web
        public static IEnumerator GetTextFromFileAsync(System.Action<string> Callback, string FileAddress)
        {
            // If file doesn't exist...
            if (!File.Exists(FileAddress))
            {
                Debug.LogError($"File does not exist!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // Request and download the texture
            UnityWebRequest request = UnityWebRequest.Get(FileAddress);
            AsyncOperation download = request.SendWebRequest();

            // Wait until download is finished
            yield return download;

            // If encoutering an error after downloading...
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"File couldn't be downloaded!\n[{FileAddress}]");
                request.Dispose();
                Callback?.Invoke(null);
                yield break;
            }

            // Set texture from download request then dispose request
            string textContents = request.downloadHandler.text;
            request.Dispose();

            // If contents are null...
            if (textContents == null)
            {
                Debug.LogError($"File invalid!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // If contents are empty...
            if (textContents == string.Empty)
            {
                Debug.LogWarning($"File is empty!\n[{FileAddress}]");
            }

            Callback?.Invoke(textContents);
        }

        // Asynchronously returns a Texture2D from a file on the computer or uploaded to the web
        public static IEnumerator GetTexture2DFromFileAsync(System.Action<Texture2D> Callback, string FileAddress)
        {
            // If file doesn't exist...
            if (!File.Exists(FileAddress))
            {
                Debug.LogError($"File does not exist!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // Request and download the texture
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(FileAddress);
            AsyncOperation download = request.SendWebRequest();

            // Wait until download is finished
            yield return download;

            // If encoutering an error after downloading...
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"File couldn't be downloaded!\n[{FileAddress}]");
                request.Dispose();
                Callback?.Invoke(null);
                yield break;
            }

            // Set texture from download request then dispose request
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            request.Dispose();

            if (texture == null)
            {
                Debug.LogError($"File is not a valid image!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            Callback?.Invoke(texture);
        }

        // Asynchronously returns a Sprite from a file on the computer or uploaded to the web
        public static IEnumerator GetSpriteFromFileAsync(System.Action<Sprite> Callback, string FileAddress, int PixelsPerUnit = 100)
        {
            // If file doesn't exist...
            if (!File.Exists(FileAddress))
            {
                Debug.LogError($"File does not exist!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // Request and download the texture
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(FileAddress);
            AsyncOperation download = request.SendWebRequest();

            // Wait until download is finished
            yield return download;

            // If encoutering an error after downloading...
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"File couldn't be downloaded!\n[{FileAddress}]");
                request.Dispose();
                Callback?.Invoke(null);
                yield break;
            }

            // Set texture from download request then dispose request
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            request.Dispose();

            if (texture == null)
            {
                Debug.LogError($"File is not a valid image!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // Create a new sprite using downloaded texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);

            // If sprite is null...
            if (sprite == null)
            {
                Debug.LogError($"File could not be converted to a sprite!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            Callback?.Invoke(sprite);
        }

        // Asynchronously returns an AudioClip from a file on the computer or uploaded to the web
        public static IEnumerator GetAudioClipFromFileAsync(System.Action<AudioClip> Callback, string FileAddress)
        {
            // If file doesn't exist...
            if (!File.Exists(FileAddress))
            {
                Debug.LogError($"File does not exist!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // A list of Unity's listed audio types and the associated file extensions
            Dictionary<string, AudioType> audioTypes = new Dictionary<string, AudioType>
            {
                { "acc", AudioType.ACC }, // Not supported
                { "aiff", AudioType.AIFF },
                { "it", AudioType.IT },
                { "mod", AudioType.MOD },
                { "mp3", AudioType.MPEG },
                { "ogg", AudioType.OGGVORBIS },
                { "s3m", AudioType.S3M },
                { "vag", AudioType.VAG },
                { "wav", AudioType.WAV },
                { "xm", AudioType.XM },
                { "xma", AudioType.XMA }
            };

            string fileExtension = Path.GetExtension(FileAddress).Substring(1);
            AudioType audioType = audioTypes.TryGetValue(fileExtension, out AudioType type) ? type : AudioType.UNKNOWN;

            // Request and download the audio clip
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(FileAddress, audioType);
            AsyncOperation download = request.SendWebRequest();

            // Wait until download is finished
            yield return download;

            // If encoutering an error after downloading...
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"File couldn't be downloaded!\n[{FileAddress}]");
                request.Dispose();
                Callback?.Invoke(null);
                yield break;
            }

            // Set audio clip from download request then dispose request
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            request.Dispose();

            if (audioClip == null)
            {
                Debug.LogError($"File is not a valid audio file!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            Callback?.Invoke(audioClip);
        }

        // Asynchronously returns an AssetBundle from a file on the computer or uploaded to the web
        public static IEnumerator GetAssetBundleFromFileAsync(System.Action<AssetBundle> Callback, string FileAddress)
        {
            // If file doesn't exist...
            if (!File.Exists(FileAddress))
            {
                Debug.LogError($"File does not exist!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            // Request and download the asset bundle
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(FileAddress);
            AsyncOperation download = request.SendWebRequest();

            // Wait until download is finished
            yield return download;

            // If encoutering an error after downloading...
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"File couldn't be downloaded!\n[{FileAddress}]");
                request.Dispose();
                Callback?.Invoke(null);
                yield break;
            }

            // Set asset bundle from download request then dispose request
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            request.Dispose();

            if (assetBundle == null)
            {
                Debug.LogError($"File is not a valid asset bundle!\n[{FileAddress}]");
                Callback?.Invoke(null);
                yield break;
            }

            Callback?.Invoke(assetBundle);
        }
        #endregion

        // Creates or overwrites a text file with data
        public static void WriteTextFileContents(string FilePath, string Contents)
        {
            // If contents are null...
            if (Contents == null)
            {
                Debug.LogError($"Contents are null!");
                return;
            }

            // If contents are empty...
            if (Contents == string.Empty)
            {
                Debug.LogWarning($"Contents are empty!");
            }

            // If file already exists...
            if (File.Exists(FilePath))
            {
                Debug.Log($"Overwriting existing file!\n[{FilePath}]");
            }
            else
            {
                Debug.Log($"Creating new file!\n[{FilePath}]");
            }

            File.WriteAllText(FilePath, Contents);
        }

        // Returns the filename with the specified extension
        public static string VerifyFileExtension(string FileName, string FileExtension, bool ForceAppendExtension = false)
        {
            // If file name is blank...
            if (string.IsNullOrEmpty(FileName))
            {
                Debug.LogError($"Blank file name!");
                return null;
            }

            // TODO: Check for other invalid filenames (@, /, etc.). Possibly OS dependent.

            // If file extension is blank or only contains periods (".")...
            if (string.IsNullOrEmpty(FileExtension) || FileExtension.Replace(".", "") == string.Empty)
            {
                Debug.LogError($"Blank extension!");
                return null;
            }

            string output = string.Empty;

            FileExtension = FileExtension.ToLower();

            // If file extension starts with a '.', remove it
            if (FileExtension[0] == '.')
            {
                FileExtension = FileExtension.Substring(1);
            }

            // Split the filename into name / extension
            string[] fileNameArray = FileName.Split('.', StringSplitOptions.RemoveEmptyEntries);

            // If there is more than 1 "." in the file name
            if (fileNameArray.Length > 2)
            {
                // Not error because you can have files with multiple periods (".")
                Debug.LogWarning($"More than one '.' contained in filename!\n[{FileName}]");
            }

            // If you want to add extension onto filename regardless of number of periods (".")...
            if (ForceAppendExtension)
            {
                output = string.Join(".", fileNameArray) + "." + FileExtension;
            }
            else
            {
                // If the file name doesn't already contain extensions...
                if (fileNameArray.Length == 1)
                {
                    output = fileNameArray[0] + "." + FileExtension;
                }
                else
                {
                    // Replace the last element in the array with new file extension
                    fileNameArray[fileNameArray.Length - 1] = FileExtension;
                    output = string.Join(".", fileNameArray);
                }
            }

            return output;
        }
    }
}