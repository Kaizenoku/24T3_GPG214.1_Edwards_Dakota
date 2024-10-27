using DakotaLib;
using Gamekit2D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLoader : StreamingAssetLoader
{
    private string m_PlayerAssetName = "Ellen";
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

    private void Awake()
    {
        m_PlayerAssetBundle = StreamingAssetUtilities.GetAssetBundleFromFile(m_AssetFilePath);
        if (m_PlayerAssetBundle == null) return;

        m_PlayerPrefab = m_PlayerAssetBundle.LoadAsset<GameObject>(m_PlayerAssetName);
        if (m_PlayerPrefab == null) return;

        Vector3 playerPos = new Vector3(m_PlayerXPosition, m_PlayerYPosition);
        Quaternion playerRot = Quaternion.identity;

        m_PlayerInstance = Instantiate(m_PlayerPrefab, playerPos, playerRot);

        UpdateLight2D();
        UpdateDamager();
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
}
