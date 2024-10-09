using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class WeaponPickup : MonoBehaviour, IDataPersister
    {
        public SpriteRenderer spriteRenderer;
        public ParticleSystem blueMotes;
        public InteractOnTrigger2D interactOnTrigger2D;
        public DataSettings dataSettings;

        void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        public DataSettings GetDataSettings ()
        {
            return dataSettings;
        }

        public void SetDataSettings (string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData ()
        {
            return new Data<Sprite, bool, bool> (spriteRenderer.sprite, blueMotes.isPlaying, interactOnTrigger2D.enabled);
        }

        public void LoadData (Data data)
        {
            Data<Sprite, bool, bool> weaponPickupData = (Data<Sprite, bool, bool>)data;
            spriteRenderer.sprite = weaponPickupData.value0;
            if(!weaponPickupData.value1)
                blueMotes.Stop();
            if (!weaponPickupData.value2)
                interactOnTrigger2D.enabled = false;
        }

        public string[] SaveDataAsString()
        {
            Data<Sprite, bool, bool> data = (Data<Sprite, bool, bool>)SaveData();

            Sprite sprite = data.value0;
            
            // TURN SPRITE INTO SPRITE REFERENCE
            string spriteReference = string.Empty;

            string[] output = { spriteReference, data.value1.ToString(), data.value2.ToString() };
            return output;
        }

        public void LoadDataFromString(string[] stringData)
        {
            string spriteReference = stringData[0];

            // GET SPRITE FROM SPRITE REFERENCE
            Sprite sprite = spriteRenderer.sprite;

            Data<Sprite, bool, bool> data = new Data<Sprite, bool, bool>(sprite, bool.Parse(stringData[1]), bool.Parse(stringData[2]));
            LoadData(data);
        }
    }
}