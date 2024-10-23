using System.IO;
using UnityEngine;

namespace DakotaLib
{
    public static class FileSystemUtilities
    {
        public static string[] knownTextFileExtensions = new string[] { ".txt", ".json", ".xml"};
        private static string dataPath = Application.dataPath;
        private static string persistentDataPath = Application.persistentDataPath;
        private static string streamingAssetsPath = Application.streamingAssetsPath;
        private static string temporaryCachePath = Application.temporaryCachePath;

        // Checks if a folder or file at this path exists
        public static bool DoesPathExist(string path)
        {
            // If either the file or directory exists at the path...
            return File.Exists(path) || Directory.Exists(path);
        }

        // Checks if the string is not empty and the path exists
        // TODO check for special characters that WINDOWS doesn't like in filenames...
        public static bool IsPathValid(string path)
        {
            // If the path exists and it's not a blank path
            return DoesPathExist((string)path) && !string.IsNullOrEmpty(path);
        }

        // Returns the combined file name and extension
        public static string VerifyFileExtension(string fileName, string extension)
        {
            string output = fileName;

            // If file name is blank...
            if (string.IsNullOrEmpty(fileName))
            {
                //Debug.LogError(String.Format("Invalid file name! Returning [{0}].", output));
                return output;
            }

            string fileNameExtension = string.Empty;
            
            // If the file name has a "." at the end it...
            if (fileName.LastIndexOf(".") == fileName.Length)
            {
                // Remove the "."
                fileName = fileName.Substring(0, fileName.Length - 1);
            }
            // Else, if the file contains a "."...
            else if (fileName.LastIndexOf('.') != -1)
            {
                fileNameExtension = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
            }

            // If the extension isn't blank...
            if (!string.IsNullOrEmpty(extension))
            {
                // Convert extension to all lowercase
                extension = extension.ToLower();

                // Add a "." to the start of the extension if it doesn't have on
                extension = extension.Substring(0, 1) == "." ? extension : "." + extension;

                // If the existing extension is blank
                if (string.IsNullOrEmpty(fileNameExtension))
                {
                    output = fileName + extension;
                    //Debug.Log(string.Format("Returning [{0}].", output));
                    return output;
                }
                // If both extensions match
                else if (fileNameExtension == extension)
                {
                    //Debug.Log(string.Format("Both extensions match. Returning [{0}].", output));
                    return output;
                }
                else
                {
                    output = fileName.Substring(0, fileName.Length - fileNameExtension.Length) + extension;
                    //Debug.Log(string.Format("Replacing [{1}] with [{2}]. Returning [{0}].", output, fileNameExtension, extension));
                    return output;
                }
            }

            // If file name extension is empty...
            if (string.IsNullOrEmpty(fileNameExtension))
            {
                //Debug.LogError(string.Format("No extensions! Returning [{0}].", output));
                return output;
            }
            // The new extension is empty but the old one exists
            else
            {
                //Debug.LogWarning(String.Format("No extension to verify against. Returning [{0}].", output));
                return output;
            }
        }

        // Creates a folder at the specified directory if none already exists
        public static void CreateFolder(string directoryPath)
        {
            // If directory path is blank...
            if (string.IsNullOrEmpty(directoryPath))
            {
                //Debug.LogError("Invalid directory path!");
                return;
            }

            // If the folder does not exist...
            if (!DoesPathExist(directoryPath))
            {
                // Create the folder(s)
                //Debug.Log("Folder created at [" +  directoryPath + "]");
                Directory.CreateDirectory(directoryPath);
            }
            else
            {
                //Debug.Log("Folder already exists at [" + directoryPath +"]");
            }
        }

        // Returns a string with the combined filepath + filename
        public static string ReturnCombinedFilePath(string directoryPath, string fileName)
        {
            // If file path is blank or doesn't exist...
            if (!IsPathValid(directoryPath))
            {
                //Debug.LogError("Invalid directory!");
                return null;
            }

            // If file name is blank...
            if (string.IsNullOrEmpty(fileName))
            {
                //Debug.LogWarning("No fileName!");
            }
            // Else, if the file name doesn't contain a "."...
            else if (!fileName.Contains("."))
            {
                //Debug.LogWarning("[" + fileName + "] doesn't contain a file extension!");
            }

            string output = Path.Combine(directoryPath, fileName);
            //Debug.Log("Output: " + output);
            return output;
        }

        public static string ReturnCombinedStreamingAssetsFilePath(string fileName)
        {
            return ReturnCombinedFilePath(streamingAssetsPath, fileName);
        }

        public static string ReturnCombinedDataFilePath(string fileName)
        {
            return ReturnCombinedFilePath(dataPath, fileName);
        }

        public static string ReturnCombinedPersistentDataFilePath(string fileName)
        {
            return ReturnCombinedFilePath(persistentDataPath, fileName);
        }

        public static string ReturnCombinedTemporaryCacheFilePath(string fileName)
        {
            return ReturnCombinedFilePath(temporaryCachePath, fileName);
        }

        // Returns a string with the contents of a text file
        public static string ReadTextFileContents(string filePath)
        {
            // If file doesn't exist or if file path is blank...
            if (!IsPathValid(filePath))
            {
                //Debug.LogError("Invalid file path!");
                return null;
            }

            string extension = Path.GetExtension(filePath);
            string output = string.Empty;

            // Read the text contents in the file (changes for some file extensions)
            switch (extension)
            {
                //case ".json":
                //    // TODO
                //    break;

                //case ".xml":
                //    // TODO
                //    break;

                default:
                    output = File.ReadAllText(filePath);
                    break;
            }


            // If the text contents in the file is empty...
            if (string.IsNullOrEmpty(output))
            {
                //Debug.LogWarning("File is empty.");
            }
            else
            {
                //Debug.Log("Contents: " + output);
            }

            return output;
        }

        // Returns a string with the contents of a text file
        public static string ReadTextFileContents(string folderPath, string fileName)
        {
            string combinedPath = ReturnCombinedFilePath(folderPath, fileName);

            // If file doesn't exist or if file path is blank...
            if (!IsPathValid(combinedPath))
            {
                //Debug.LogError("Invalid file path!");
                return null;
            }

            string extension = Path.GetExtension(combinedPath);
            string output = string.Empty;

            // Read the text contents in the file (changes for some file extensions)
            switch (extension)
            {
                //case ".json":
                //    // TODO
                //    break;

                //case ".xml":
                //    // TODO
                //    break;

                default:
                    output = File.ReadAllText(combinedPath);
                    break;
            }


            // If the text contents in the file is empty...
            if (string.IsNullOrEmpty(output))
            {
                //Debug.LogWarning("File is empty.");
            }
            else
            {
                //Debug.Log("Contents: " + output);
            }

            return output;
        }

        // Creates or overwrites a text file with data
        public static void WriteTextFileContents(string contents, string directoryPath, string fileName)
        {
            if (!IsPathValid(directoryPath))
            {
                //Debug.LogError(string.Format("[{0}] is an invalid directoryPath!", directoryPath));
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                //Debug.LogError("Invalid fileName!");
                return;
            }

            if (string.IsNullOrEmpty(contents))
            {
                //Debug.LogWarning("No contents!");
            }

            bool knownFileExtension = false;
            string extension = Path.GetExtension(fileName);
            // For each known text file extension...
            foreach (string knownExtension in knownTextFileExtensions)
            {
                // If the extension of the fileName is the same as a known one...
                if (extension == knownExtension)
                {
                    knownFileExtension = true;
                    break;
                }
            }
        
            // If the file extension is not a known text file extension...
            if (!knownFileExtension)
            {
                //Debug.LogWarning("[" + fileName + "] is not a known text file extension [" + string.Join(", ", knownTextFileExtensions) + "].");
            }

            string combinedPath = Path.Combine(directoryPath, fileName);
        
            // If file already exists
            if (IsPathValid(combinedPath))
            {
                //Debug.Log("Overwriting [" + fileName + "] at [" + directoryPath + "]." );
            }
            else
            {
                //Debug.Log("Creating [" + fileName + "] at [" + directoryPath + "].");
            }

            // Write contents to file (changes for some file extensions)
            switch (extension)
            {
                //case ".xml":
                //    // TODO
                //    break;

                default:
                    File.WriteAllText(combinedPath, contents);
                    break;
            }

            //Debug.Log("[" + contents + "] written to [" + combinedPath + "]");
        }

        // Creates or overwrites a text file with data
        public static void WriteTextFileContents(string contents, string filePath)
        {
            if (string.IsNullOrEmpty(contents))
            {
                //Debug.LogWarning("No contents!");
            }

            bool knownFileExtension = false;
            string extension = Path.GetExtension(filePath);
            // For each known text file extension...
            foreach (string knownExtension in knownTextFileExtensions)
            {
                // If the extension of the fileName is the same as a known one...
                if (extension == knownExtension)
                {
                    knownFileExtension = true;
                    break;
                }
            }

            // If the file extension is not a known text file extension...
            if (!knownFileExtension)
            {
                //Debug.LogWarning("[" + filePath + "] is not a known text file extension [" + string.Join(", ", knownTextFileExtensions) + "].");
            }

            // If file already exists
            if (IsPathValid(filePath))
            {
                //Debug.Log("Overwriting [" + filePath + "].");
            }
            else
            {
                //Debug.Log("Creating [" + filePath + "].");
            }

            // Write contents to file (changes for some file extensions)
            switch (extension)
            {
                //case ".xml":
                //    // TODO
                //    break;

                default:
                    File.WriteAllText(filePath, contents);
                    break;
            }

            //Debug.Log("[" + contents + "] written to [" + filePath + "]");
        }
    }
}