using Project1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Gamekit2D
{
    /// <summary>
    /// This class is used to transition between scenes. This includes triggering all the things that need to happen on transition such as data persistence.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<SceneController>();

                if (instance != null)
                    return instance;

                Create ();

                return instance;
            }
        }

        public static bool Transitioning
        {
            get { return Instance.m_Transitioning; }
        }

        protected static SceneController instance;

        public static SceneController Create ()
        {
            GameObject sceneControllerGameObject = new GameObject("SceneController");
            instance = sceneControllerGameObject.AddComponent<SceneController>();

            return instance;
        }

        public SceneTransitionDestination initialSceneTransitionDestination;

        // Variables added by Dakota
        public string lastTransitionSceneName;
        public bool lastTransitionResetInputValuesOnTransition;
        public SceneTransitionDestination.DestinationTag lastTransitionDestinationTag;
        public TransitionPoint.TransitionType lastTransitionType;

        protected Scene m_CurrentZoneScene;
        protected SceneTransitionDestination.DestinationTag m_ZoneRestartDestinationTag;
        protected PlayerInput m_PlayerInput;
        protected bool m_Transitioning;

        void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            m_PlayerInput = FindObjectOfType<PlayerInput>();

            lastTransitionSceneName = lastTransitionSceneName == string.Empty ? SceneManager.GetActiveScene().name : lastTransitionSceneName;
            lastTransitionResetInputValuesOnTransition = true;

            lastTransitionType = TransitionPoint.TransitionType.SameScene;

            if (initialSceneTransitionDestination != null)
            {
                lastTransitionDestinationTag = initialSceneTransitionDestination.destinationTag;
                SetEnteringGameObjectLocation(initialSceneTransitionDestination);
                ScreenFader.SetAlpha(1f);
                StartCoroutine(ScreenFader.FadeSceneIn());
                initialSceneTransitionDestination.OnReachDestination.Invoke();
            }
            else
            {
                m_CurrentZoneScene = SceneManager.GetActiveScene();
                m_ZoneRestartDestinationTag = SceneTransitionDestination.DestinationTag.A;
                lastTransitionDestinationTag = m_ZoneRestartDestinationTag;
            }
        }

        public static void RestartZone(bool resetHealth = true)
        {
            if(resetHealth && PlayerCharacter.PlayerInstance != null)
            {
                PlayerCharacter.PlayerInstance.damageable.SetHealth(PlayerCharacter.PlayerInstance.damageable.startingHealth);
            }

            Instance.StartCoroutine(Instance.Transition(Instance.m_CurrentZoneScene.name, true, Instance.m_ZoneRestartDestinationTag, TransitionPoint.TransitionType.DifferentZone));
        }

        public static void RestartZoneWithDelay(float delay, bool resetHealth = true)
        {
            Instance.StartCoroutine(CallWithDelay(delay, RestartZone, resetHealth));
        }

        public static void TransitionToScene(TransitionPoint transitionPoint)
        {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.resetInputValuesOnTransition, transitionPoint.transitionDestinationTag, transitionPoint.transitionType));
        }

        public static SceneTransitionDestination GetDestinationFromTag(SceneTransitionDestination.DestinationTag destinationTag)
        {
            return Instance.GetDestination(destinationTag);
        }

        public IEnumerator TransitionSaveFile(SaveData SaveData, PlayerCharacter Player)
        {
            m_Transitioning = true;
            
            if (m_PlayerInput == null)
                m_PlayerInput = FindObjectOfType<PlayerInput>();
            m_PlayerInput.ReleaseControl(SaveData.sceneResetInputValuesOnTransition);

            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));

            lastTransitionSceneName = SaveData.sceneName;
            lastTransitionResetInputValuesOnTransition = SaveData.sceneResetInputValuesOnTransition;
            lastTransitionDestinationTag = SaveData.sceneTransitionDestinationTag;
            lastTransitionType = SaveData.sceneTransitionType;

            PersistentDataManager.ClearPersisters();

            SceneManager.LoadScene(SaveData.sceneName);
            //yield return SceneManager.LoadSceneAsync(SaveData.sceneName);

            //yield return new WaitForSecondsRealtime(1f);

            #region Debug SaveData
            //Debug.Log(string.Format(
            //    "Data Loaded\n" +
            //    "Scene Name: [{0}]\n" +
            //    "Scene ResetInputValues: [{1}]\n" +
            //    "Scene Destination Tag: [{2}]\n" +
            //    "Scene Transition Type: [{3}]\n" +
            //    "Spawn Facing: [{4}]\n" +
            //    "Spawn Location: [{5}]\n",
            //    SaveData.sceneName,
            //    SaveData.sceneResetInputValuesOnTransition,
            //    SaveData.sceneTransitionDestinationTag,
            //    SaveData.sceneTransitionType,
            //    SaveData.spawnFacingLeft ? "left" : "right",
            //    SaveData.spawnPosition.ToString()
            //));

            string debugOutput = "Saved Data Persisters!\n";

            for (int i = 0; i < SaveData.persisterSaveData.Length; i++)
            {
                debugOutput += $"Persister Name: [{SaveData.persisterSaveData[i].className}]\n";
                debugOutput += $"Persister Tag: [{SaveData.persisterSaveData[i].dataSettingsTag}]\n";
                debugOutput += $"Persister Type: [{SaveData.persisterSaveData[i].dataSettingsPersistenceType}]\n";
                debugOutput += $"Persister Values: [{string.Join(", ", SaveData.persisterSaveData[i].dataValues)}]\n\n";
            }

            Debug.Log(debugOutput);
            #endregion

            if (m_PlayerInput == null)
                m_PlayerInput = FindObjectOfType<PlayerInput>();
            m_PlayerInput.ReleaseControl(SaveData.sceneResetInputValuesOnTransition);

            List<IDataPersister> dataPersistersInScene = new List<IDataPersister>(
                FindObjectsOfType<MonoBehaviour>().OfType<IDataPersister>()
            );

            #region Debug Fresh Scene Persisters
            debugOutput = "Fresh Data Persisters!\n";

            foreach (var  dp in dataPersistersInScene)
            {
                debugOutput += $"Persister Name: [{dp.GetType().Name}]\n";
                debugOutput += $"Persister Tag: [{dp.GetDataSettings().dataTag}]\n";
                debugOutput += $"Persister Type: [{dp.GetDataSettings().persistenceType}]\n";
                debugOutput += $"Persister Values: [{string.Join(", ", dp.SaveDataAsString())}]\n\n";
            }

            Debug.Log(debugOutput);
            #endregion

            foreach (IDataPersisterSaveData dpFromSave in SaveData.persisterSaveData)
            {
                foreach (IDataPersister dp in dataPersistersInScene)
                {
                    if (dp.GetDataSettings().dataTag == dpFromSave.dataSettingsTag)
                    {
                        dp.SetDataSettings(dpFromSave.dataSettingsTag, dpFromSave.dataSettingsPersistenceType);
                        dp.LoadDataFromString(dpFromSave.dataValues);

                        PersistentDataManager.RegisterPersister(dp);

                        //dataPersistersInScene.Remove(dpClass);
                        break;
                    }
                }
            }

            PersistentDataManager.LoadAllData();

            #region Debug Loaded Scene Persisters
            debugOutput = "Loaded Data Persisters!\n";

            foreach (var persister in dataPersistersInScene)
            {
                debugOutput += $"Persister Name: [{persister.GetType().Name}]\n";
                debugOutput += $"Persister Tag: [{persister.GetDataSettings().dataTag}]\n";
                debugOutput += $"Persister Type: [{persister.GetDataSettings().persistenceType}]\n";
                debugOutput += $"Persister Values: [{string.Join(", ", persister.SaveDataAsString())}]\n\n";
            }

            Debug.Log(debugOutput);
            #endregion

            SceneTransitionDestination entrance = GetDestination(SaveData.sceneTransitionDestinationTag);
            //SetEnteringGameObjectLocation(entrance);
            //SetupNewScene(SaveData.sceneTransitionType, entrance);

            //if (entrance != null)
            //    entrance.OnReachDestination.Invoke();

            Player.UpdateFacing(SaveData.spawnFacingLeft);
            GameObjectTeleporter.Teleport(Player.gameObject, SaveData.spawnPosition);

            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            m_PlayerInput.GainControl();

            m_Transitioning = false;
        }

        protected IEnumerator Transition(string newSceneName, bool resetInputValues, SceneTransitionDestination.DestinationTag destinationTag, TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone)
        {
            m_Transitioning = true;
            
            // Dakota added this...
            lastTransitionSceneName = newSceneName;
            lastTransitionResetInputValuesOnTransition = resetInputValues;
            lastTransitionDestinationTag = destinationTag;
            lastTransitionType = transitionType;
            
            PersistentDataManager.SaveAllData();

            if (m_PlayerInput == null)
                m_PlayerInput = FindObjectOfType<PlayerInput>();
            m_PlayerInput.ReleaseControl(resetInputValues);
            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));
            PersistentDataManager.ClearPersisters();
            yield return SceneManager.LoadSceneAsync(newSceneName);
            m_PlayerInput = FindObjectOfType<PlayerInput>();
            m_PlayerInput.ReleaseControl(resetInputValues);
            PersistentDataManager.LoadAllData();
            SceneTransitionDestination entrance = GetDestination(destinationTag);
            SetEnteringGameObjectLocation(entrance);
            SetupNewScene(transitionType, entrance);
            if(entrance != null)
                entrance.OnReachDestination.Invoke();
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            m_PlayerInput.GainControl();

            m_Transitioning = false;
        }

        protected SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
        {
            SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].destinationTag == destinationTag)
                    return entrances[i];
            }
            Debug.LogWarning("No entrance was found with the " + destinationTag + " tag.");
            return null;
        }

        protected void SetEnteringGameObjectLocation(SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Entering Transform's location has not been set.");
                return;
            }
            Transform entranceLocation = entrance.transform;
            Transform enteringTransform = entrance.transitioningGameObject.transform;
            enteringTransform.position = entranceLocation.position;
            enteringTransform.rotation = entranceLocation.rotation;
        }

        protected void SetupNewScene(TransitionPoint.TransitionType transitionType, SceneTransitionDestination entrance)
        {
            if (entrance == null)
            {
                Debug.LogWarning("Restart information has not been set.");
                return;
            }
        
            if (transitionType == TransitionPoint.TransitionType.DifferentZone)
                SetZoneStart(entrance);
        }

        protected void SetZoneStart(SceneTransitionDestination entrance)
        {
            m_CurrentZoneScene = entrance.gameObject.scene;
            m_ZoneRestartDestinationTag = entrance.destinationTag;
        }

        static IEnumerator CallWithDelay<T>(float delay, Action<T> call, T parameter)
        {
            yield return new WaitForSeconds(delay);
            call(parameter);
        }
    }
}