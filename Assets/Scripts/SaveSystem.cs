using DakotaUtility;
using Gamekit2D;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project1
{
    // TODO, COMMENT! Eg. what does this actually save?
    // TODO, add a debug log of previous values when loaded and new values?

    // Might have to remove these "requires components" if we move data elsewhere
    [RequireComponent(typeof(PlayerCharacter))]
    public class SaveSystem : MonoBehaviour
    {

        [SerializeField] private string saveFileName;
        [SerializeField] private string saveFolderPath;
        private string defaultFileName = "Project1 SaveData";
        private string defaultFolderPath = Application.streamingAssetsPath;
        private string fileType = ".json";
        private SaveData saveData = new SaveData();

        [SerializeField] private PlayerCharacter character;

        private void Start()
        {
            // Setting default values
            character = character == null ? gameObject.GetComponent<PlayerCharacter>() : character;

            saveFileName = string.IsNullOrEmpty(saveFileName) ? defaultFileName : saveFileName;
            saveFolderPath = string.IsNullOrEmpty(saveFolderPath) ? defaultFolderPath : saveFolderPath;

            //TODO add default "scene" stuff
        }

        // Currently only used for testing... add to DEBUG LOGGER thingy (see 2DGameKit references)
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                SaveGameToJSON(saveFileName, saveFolderPath);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadGameFromJSON(saveFileName, saveFolderPath);
            }
        }

        // Saves the game to JSON
        // TODO, NEED TO ADD CHECKS FOR MISSING DATA...
        public void SaveGameToJSON(string saveFileName, string saveFolderPath)
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

            saveData.persisterSaveData = dpSaveData;

            SceneController sc = SceneController.Instance;

            saveData.sceneName = sc.lastTransitionSceneName;
            saveData.sceneResetInputValuesOnTransition = sc.lastTransitionResetInputValuesOnTransition;
            saveData.sceneTransitionDestinationTag = sc.lastTransitionDestinationTag;
            saveData.sceneTransitionType = sc.lastTransitionType;

            // Used to check if the dpData was successfully saved
            //foreach (var dp in saveData.persisterSaveData)
            //{
            //    DebugDataPersisterData(dp);
            //}

            if (character.LastCheckpoint != null)
            {
                Checkpoint lastCheckpoint = character.LastCheckpoint;
                saveData.spawnFacingLeft = lastCheckpoint.respawnFacingLeft;
                saveData.spawnPosition = lastCheckpoint.transform.position;
            }

            // Used to check if all data was successfully saved
            //DebugSaveData(saveData);

            saveFileName = FileSystemOperations.VerifyFileExtension(saveFileName, fileType);
            string saveDataJSON = JsonUtility.ToJson(saveData);

            //Debug.Log("Contents: " + saveDataJSON);
            //Debug.Log("Save File Name: " + saveFileName);
            //Debug.Log("Save Folder Path: " + saveFolderPath);
            
            FileSystemOperations.WriteTextFileContents(saveDataJSON, saveFolderPath, saveFileName);
            
            //Debug.Log("Saving successful!");
        }

        // Loads the game from JSON
        // TODO, NEED TO ADD CHECKS FOR MISSING DATA
        // Maybe make this an IEnumerator
        public void LoadGameFromJSON(string saveFileName, string saveFolderPath)
        {
            //Debug.Log("Loading data...");

            saveFileName = FileSystemOperations.VerifyFileExtension(saveFileName, fileType);
            string saveDataJSON = FileSystemOperations.ReadTextFileContents(saveFolderPath, saveFileName);

            saveData = JsonUtility.FromJson<SaveData>(saveDataJSON);

            // Used to check if save data contents loaded from file correctly
            //DebugSaveData(saveData);

            SceneController.TransitionToScene(saveData.sceneName, saveData.sceneResetInputValuesOnTransition, saveData.sceneTransitionDestinationTag, saveData.sceneTransitionType);

            // Have to wait for scene transition to finish before teleporting?
            character.UpdateFacing(saveData.spawnFacingLeft);
            GameObjectTeleporter.Teleport(character.gameObject, saveData.spawnPosition);

            PersistentDataManager.ClearPersisters();

            List<IDataPersister> dataPersistersInScene = new List<IDataPersister>(
                FindObjectsOfType<MonoBehaviour>().OfType<IDataPersister>()
            );

            foreach (IDataPersisterSaveData dpSaveData in saveData.persisterSaveData)
            {
                foreach (var dpClass in dataPersistersInScene)
                {
                    if (dpClass.GetDataSettings().dataTag == dpSaveData.dataSettingsTag)
                    {
                        dpClass.SetDataSettings(dpSaveData.dataSettingsTag, dpSaveData.dataSettingsPersistenceType);

                        dpClass.LoadDataFromString(dpSaveData.dataValues);

                        PersistentDataManager.RegisterPersister(dpClass);

                        dataPersistersInScene.Remove(dpClass);
                        break;
                    }
                }
            }
        }

        // UPDATE
        public void DebugDataPersisterData(IDataPersisterSaveData dpData)
        {
            Debug.Log(string.Format(
                "Class Name: [{0}]\n" +
                "Data Tag: [{1}]\n" +
                "Data Persistence: [{2}]\n" +
                "Data Values: [{3}]",
                dpData.className,
                dpData.dataSettingsTag,
                dpData.dataSettingsPersistenceType,
                string.Join(", ", dpData.dataValues)
            ));
        }

        // UPDATE
        public void DebugSaveData(SaveData saveData)
        {
            string[] dpNames = new string[saveData.persisterSaveData.Length];

            for (int i = 0; i < dpNames.Length; i++)
            {
                dpNames[i] = saveData.persisterSaveData[i].className;
            }

            Debug.Log(string.Format(
                "Scene Name: [{0}]\n" +
                "Spawn Facing: [{1}]\n" +
                "Spawn Location: [{2}]\n" +
                "Data Persistors: [{3}]",
                saveData.sceneName,
                saveData.spawnFacingLeft ? "left" : "right",
                saveData.spawnPosition.ToString(),
                string.Join(", ", dpNames)
            ));
        }
    }
}
