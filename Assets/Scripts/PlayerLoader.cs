using DakotaLib;
using Gamekit2D;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(-5)]
public class PlayerLoader: MonoBehaviour
{
    [SerializeField] private string m_AssetBundleName = "player";
    [SerializeField] private string m_RelativeAssetBundlePath = "AssetBundles";
    [SerializeField] private string m_PlayerAssetName = "Ellen";
    private string m_FullFilePath;

    private AssetBundle m_PlayerAssetBundle;
    private GameObject m_PlayerPrefab;
    private GameObject m_PlayerInstance;
    [SerializeField] private float m_PlayerXPosition = 0.0f;
    [SerializeField] private float m_PlayerYPosition = 0.0f;

    private Light2D m_Light2D;
    [SerializeField] private float m_Light2DIntensity = -1.0f;
    [SerializeField] private float m_Light2DFalloffStrength = -1.0f;
    [SerializeField] private float m_Light2DOuterRadius = -1.0f;
    
    private Damager m_Damager;
    [SerializeField] private List<string> m_DamagerHittableLayers = new List<string>();

    private PlayerInput m_PlayerInput;
    private PlayerCharacter m_PlayerCharacter;
    private Animator m_Animator;

    protected void Awake()
    {
        m_FullFilePath = Path.Combine(Application.streamingAssetsPath, m_RelativeAssetBundlePath, m_AssetBundleName);
        
        LoadPlayer();
    }
    
    private void LoadPlayer()
    {
        m_PlayerAssetBundle = FileSystemUtilities.GetAssetBundleFromFile(m_FullFilePath);
        if (m_PlayerAssetBundle == null) return;

        m_PlayerPrefab = m_PlayerAssetBundle.LoadAsset<GameObject>(m_PlayerAssetName);
        if (m_PlayerPrefab == null) return;

        Vector3 playerPos = new Vector3(m_PlayerXPosition, m_PlayerYPosition);
        Quaternion playerRot = Quaternion.identity;

        m_PlayerInstance = Instantiate(m_PlayerPrefab, playerPos, playerRot);
        m_PlayerInstance.name = m_PlayerAssetName;

        m_PlayerAssetBundle.Unload(false);

        m_PlayerInput = m_PlayerInstance.GetComponent<PlayerInput>();
        m_PlayerCharacter = m_PlayerInstance.GetComponent<PlayerCharacter>();
        m_Animator = m_PlayerInstance.GetComponent<Animator>();

        UpdateLight2D();
        UpdateDamager();
        UpdateHealthUI();
        UpdateDirectorTriggers();
        UpdateTransitionPoints();
        UpdateTransitionDestinations();
        UpdateCharacterStateSetters();
    }

    private void UpdateLight2D()
    {
        m_Light2D = m_PlayerInstance.GetComponentInChildren<Light2D>();
        if (m_Light2D != null)
        {
            if (m_Light2DIntensity != -1.0f)
            {
                m_Light2D.intensity = m_Light2DIntensity;
            }

            if (m_Light2DFalloffStrength != -1.0f)
            {
                m_Light2D.falloffIntensity = m_Light2DFalloffStrength;
            }

            if (m_Light2DOuterRadius != -1.0f)
            {
                m_Light2D.pointLightOuterRadius = m_Light2DOuterRadius;
            }
        }
    }

    private void UpdateDamager()
    {
        m_Damager = m_PlayerInstance.GetComponent<Damager>();
        if (m_Damager != null)
        {
            if (m_DamagerHittableLayers.Count > 0)
            {
                foreach (string layer in m_DamagerHittableLayers)
                {
                    int layerIndex = LayerMask.NameToLayer(layer);

                    if (layerIndex != -1)
                    {
                        m_Damager.hittableLayers |= (1 << layerIndex);
                    }
                    else
                    {
                        Debug.LogError(string.Format("Layer [{0}] does not exist!", layer));
                    }
                }
            }
        }
    }
    
    private void UpdateHealthUI()
    {
        HealthUI healthUI = FindObjectOfType<HealthUI>();
        if (healthUI == null) { return; }

        Damageable playerDamageable = m_PlayerInstance.GetComponent<Damageable>();
        if (playerDamageable == null) { return; }

        healthUI.representedDamageable = playerDamageable;
        playerDamageable.OnHealthSet.AddListener(healthUI.ChangeHitPointUI);
    }

    private void UpdateDirectorTriggers()
    {
        if (m_PlayerInput == null) { return; }

        DirectorTrigger[] cutsceneTriggers = FindObjectsOfType<DirectorTrigger>();
        if (cutsceneTriggers.Length == 0) { return; }

        foreach (DirectorTrigger cutscene in cutsceneTriggers)
        {
            if (cutscene.triggeringGameObject == null)
            {
                cutscene.triggeringGameObject = m_PlayerInstance;
            }

            cutscene.OnDirectorPlay.AddListener(ReleasePlayerInputControls);
            cutscene.OnDirectorFinish.AddListener(m_PlayerInput.GainControl);
        }
    }

    private void UpdateCharacterStateSetters()
    {
        if (m_PlayerCharacter == null) { return; }
        if (m_Animator == null) { return; }

        CharacterStateSetter[] characterStateSetters = FindObjectsOfType<CharacterStateSetter>();
        if (characterStateSetters.Length == 0) { return; }

        foreach (CharacterStateSetter setter in characterStateSetters)
        {
            if (setter.playerCharacter == null)
            {
                setter.playerCharacter = m_PlayerCharacter;
            }

            if (setter.animator == null)
            {
                setter.animator = m_Animator;
            }
        }
    }

    private void UpdateTransitionPoints()
    {
        TransitionPoint[] transitionPoints = FindObjectsOfType<TransitionPoint>();
        if (transitionPoints.Length == 0) { return; }

        foreach (TransitionPoint point in transitionPoints)
        {
            if (point.transitioningGameObject == null)
            {
                point.transitioningGameObject = m_PlayerInstance;
            }
        }
    }

    private void UpdateTransitionDestinations()
    {
        SceneTransitionDestination[] sceneTransitionDestinations = FindObjectsOfType<SceneTransitionDestination>();
        if (sceneTransitionDestinations.Length == 0) { return; }

        foreach (SceneTransitionDestination destination in sceneTransitionDestinations)
        {
            if (destination.transitioningGameObject == null)
            {
                destination.transitioningGameObject = m_PlayerInstance;
            }
        }
    }

    private void ReleasePlayerInputControls()
    {
        m_PlayerInput.ReleaseControl();
    }
}
