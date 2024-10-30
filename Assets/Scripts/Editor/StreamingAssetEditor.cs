using UnityEditor;

namespace DakotaLib
{
    // Only apply to "StreamingAssetLoader" or derived classes
    [CustomEditor(typeof(StreamingAssetLoaderWrapper), true)]
    public class StreamingAssetEditor : Editor
    {
        SerializedProperty streamingAsset;
        SerializedProperty streamingAssetFilePath;

        private const string m_StreamingAssetPrefix = "Assets/StreamingAssets/";


        void OnEnable()
        {
            streamingAssetFilePath = serializedObject.FindProperty("m_StreamingAssetFilePath");
            streamingAsset = serializedObject.FindProperty("m_StreamingAsset");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (streamingAsset.objectReferenceValue == null)
            {
                return;
            }

            // Get the asset path relative to the project (eg. Assets/StreamingAssets/exampleFile.txt)
            string assetPath = AssetDatabase.GetAssetPath(streamingAsset.objectReferenceValue.GetInstanceID());

            // If asset is not in Streaming Assets folder...
            if (!assetPath.StartsWith(m_StreamingAssetPrefix))
            {
                return;
            }
        
            // Set in StreamingAssetLoader properties
            streamingAssetFilePath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
