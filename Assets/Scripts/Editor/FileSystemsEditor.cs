using System.IO;
using UnityEditor;
using UnityEngine;

namespace DakotaLib
{
    public static class FileSystemsEditor
    {
        private static string dataPath = Application.dataPath;
        private static string persistentDataPath = Application.persistentDataPath;
        private static string streamingAssetsPath = Application.streamingAssetsPath;
        private static string temporaryCachePath = Application.temporaryCachePath;

        // Opens a folder in the specified directory
        private static void OpenFolder(string directoryPath)
        {
            // If directory path doesn't exist...
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            // Open the folder in your operating system's file explorer
            Application.OpenURL(directoryPath);
        }

        [MenuItem("File/Open data folder")]
        private static void OpenDataFolder()
        {
            OpenFolder(dataPath);
        }

        [MenuItem("File/Open persistent data folder")]
        private static void OpenPersistentDataFolder()
        {
            OpenFolder(persistentDataPath);
        }

        [MenuItem("File/Open streaming assets folder")]
        private static void OpenStreamingAssetsFolder()
        {
            OpenFolder(streamingAssetsPath);
        }

        [MenuItem("File/Open temporary cache folder")]
        private static void OpenTemporaryCacheFolder()
        {
            OpenFolder(temporaryCachePath);
        }
    }
}