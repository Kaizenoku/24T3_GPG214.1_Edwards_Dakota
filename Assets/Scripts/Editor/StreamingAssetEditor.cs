using UnityEditor;

namespace DakotaUtility
{
    // Only apply to "StreamingAssetLoader" or derived classes
    [CustomEditor(typeof(StreamingAssetLoader), true)]
    public class StreamingAssetEditor : Editor
    {
        SerializedProperty assetFilePath;
        SerializedProperty streamingAsset;

        private const string m_StreamingAssetPrefix = "Assets/StreamingAssets";


        void OnEnable()
        {
            assetFilePath = serializedObject.FindProperty("m_AssetFilePath");
            streamingAsset = serializedObject.FindProperty("m_StreamingAsset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(streamingAsset);
            EditorGUILayout.PropertyField(assetFilePath);

            if (streamingAsset.objectReferenceValue == null)
            {
                return;
            }

            // Get the asset path relative to the project (eg. Assets/StreamingAssets/exampleFile.txt)
            string relativeAssetPath = AssetDatabase.GetAssetPath(streamingAsset.objectReferenceValue.GetInstanceID());

            // If asset is not in Streaming Assets folder...
            if (!relativeAssetPath.StartsWith(m_StreamingAssetPrefix))
            {
                return;
            }
        
            // Remove the "Assets/StreamingAssets/" part of the file path, and set in StreamingAssetLoader properties
            relativeAssetPath = relativeAssetPath.Substring(m_StreamingAssetPrefix.Length);
            assetFilePath.stringValue = relativeAssetPath;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
