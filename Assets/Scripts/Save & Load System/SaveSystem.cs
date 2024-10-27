using DakotaLib;
using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Gamekit2D.SceneTransitionDestination;
using static Gamekit2D.TransitionPoint;

namespace Project1
{
    public class SaveSystem : MonoBehaviour
    {

        [SerializeField] private string m_SaveFileName = "Project1 SaveData";
        [SerializeField] private string m_SaveFolderPath = Application.streamingAssetsPath;
        private string m_SaveFileExtension = ".json";
        private SaveData m_SaveData = new SaveData();
        private bool loading = false;
        private SceneController m_SceneControllerInstance;

        [SerializeField] private PlayerCharacter m_Player;

        private void Start()
        {
            // Setting default values
            m_Player = m_Player == null ? FindObjectOfType<PlayerCharacter>() : m_Player;
            m_SceneControllerInstance = SceneController.Instance;
        }

        public void Save()
        {
            if (loading) { return; }

            if (m_SaveFileName == "")
            {
                Debug.LogError("No save file name!");
                return;
            }

            if (!Directory.Exists(m_SaveFolderPath))
            {
                Debug.LogWarning(string.Format("[{0}] folder doesn't exist, creating new directory.", m_SaveFolderPath));
                Directory.CreateDirectory(m_SaveFolderPath);
            }
            
            SaveGameToJSON(Path.Combine(m_SaveFileName,m_SaveFileExtension), m_SaveFolderPath);
        }

        public void Load()
        {
            if (loading) { return; }

            if (!Directory.Exists(m_SaveFolderPath))
            {
                Debug.LogError(string.Format("Couldn't load data, no folder at [{0}]", m_SaveFolderPath));
                return;
            }

            string combinedPath = Path.Combine(m_SaveFolderPath, m_SaveFileName);

            if (!File.Exists(combinedPath))
            {
                Debug.LogError(string.Format("Couldn't load data, no file [{0}] in [{1}]. Remember to include a file extension, eg. '.json'", m_SaveFileName, m_SaveFolderPath));
                return;
            }

            LoadGameFromJSON(m_SaveFileName, m_SaveFolderPath);
            //StartCoroutine(LoadGameFromJSON(m_SaveFileName, m_SaveFolderPath));
        }

        // Saves the game to JSON
        // TODO, NEED TO ADD CHECKS FOR MISSING DATA...
        private void SaveGameToJSON(string saveFileName, string saveFolderPath)
        {
            //Debug.Log("Saving data...");
            PersistentDataManager.SaveAllData();

            HashSet<IDataPersister> dpHashSet = PersistentDataManager.Instance.DataPersisters;

            IDataPersisterSaveData[] dpSaveData = new IDataPersisterSaveData[dpHashSet.Count];

            int i = 0;
            foreach (var dp in dpHashSet)
            {
                dpSaveData[i] = new IDataPersisterSaveData(
                    dp.GetType().Name,
                    dp.GetDataSettings(),
                    dp.SaveDataAsString()
                );

                i++;
            }

            m_SaveData.persisterSaveData = dpSaveData;

            SceneController sc = SceneController.Instance;

            m_SaveData.sceneName = sc.lastTransitionSceneName;
            m_SaveData.sceneResetInputValuesOnTransition = sc.lastTransitionResetInputValuesOnTransition;
            m_SaveData.sceneTransitionDestinationTag = sc.lastTransitionDestinationTag;
            m_SaveData.sceneTransitionType = sc.lastTransitionType;

            if (m_Player.LastCheckpoint != null)
            {
                Checkpoint lastCheckpoint = m_Player.LastCheckpoint;
                m_SaveData.spawnFacingLeft = lastCheckpoint.respawnFacingLeft;
                m_SaveData.spawnPosition = lastCheckpoint.transform.position;
            }

            // Used to check if all data was successfully saved
            //DebugSaveData(saveData);

            string saveDataJSON = JsonUtility.ToJson(m_SaveData);

            //Debug.Log("Contents: " + saveDataJSON);
            //Debug.Log("Save File Name: " + saveFileName);
            //Debug.Log("Save Folder Path: " + saveFolderPath);
            
            FileSystemUtilities.WriteTextFileContents(saveDataJSON, saveFolderPath, saveFileName);
            
            Debug.Log("Saving successful!");
        }

        // Loads the game from JSON
        // TODO, NEED TO ADD CHECKS FOR MISSING DATA
        private void LoadGameFromJSON(string saveFileName, string saveFolderPath)
        {
            Debug.Log("Loading data...");

            loading = true;

            string saveDataJSON = FileSystemUtilities.ReadTextFileContents(saveFolderPath, saveFileName);
            m_SaveData = JsonUtility.FromJson<SaveData>(saveDataJSON);

            StartCoroutine(m_SceneControllerInstance.TransitionSaveFile(m_SaveData, m_Player));

            Debug.Log("Loading successful!");
            loading = false;
        }
    }
}
