using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D
{
    public class HubDoor : MonoBehaviour, IDataPersister
    {
        public string[] requiredInventoryItemKeys;
        public Sprite[] unlockStateSprites;

        public DirectorTrigger keyDirectorTrigger;
        public InventoryController characterInventory;
        public UnityEvent onUnlocked;
        public DataSettings dataSettings;

        SpriteRenderer m_SpriteRenderer;

        [ContextMenu("Update State")]
        void CheckInventory()
        {
            var stateIndex = -1;
            foreach (var i in requiredInventoryItemKeys)
            {
                if (characterInventory.HasItem(i))
                {
                    stateIndex++;
                }
            }
            if (stateIndex >= 0)
            {
                keyDirectorTrigger.OverrideAlreadyTriggered (true);
                m_SpriteRenderer.sprite = unlockStateSprites[stateIndex];
                if (stateIndex == requiredInventoryItemKeys.Length - 1) onUnlocked.Invoke();
            }
        }

        void OnEnable()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            characterInventory.OnInventoryLoaded += CheckInventory;
        }

        void Update ()
        {
            CheckInventory ();
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void LoadData(Data data)
        {
            var d = data as Data<Sprite>;
            m_SpriteRenderer.sprite = d.value;
        }

        public Data SaveData()
        {
            return new Data<Sprite>(m_SpriteRenderer.sprite);
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public string[] SaveDataAsString()
        {
            Data<Sprite> data = (Data<Sprite>)SaveData();

            Sprite sprite = data.value;

            // TURN SPRITE INTO SPRITE REFERENCE
            string spriteReference = string.Empty;

            string[] output = { spriteReference };
            return output;
        }

        public void LoadDataFromString(string[] stringData)
        {
            string spriteReference = stringData[0];

            // GET SPRITE FROM SPRITE REFERENCE
            Sprite sprite = m_SpriteRenderer.sprite;

            Data<Sprite> data = new Data<Sprite>(sprite);
            LoadData(data);
        }
    }
}