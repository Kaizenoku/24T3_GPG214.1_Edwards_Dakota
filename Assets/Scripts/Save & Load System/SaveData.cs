using Gamekit2D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// TODO COMMENT FILE
namespace Project1
{
    // Using a class because variables aren't instantiated all at the same time.
    [Serializable] public class SaveData
    {
        public string sceneName;
        public bool sceneResetInputValuesOnTransition;
        public SceneTransitionDestination.DestinationTag sceneTransitionDestinationTag;
        public TransitionPoint.TransitionType sceneTransitionType;
        public bool spawnFacingLeft;
        public Vector3 spawnPosition;

        public IDataPersisterSaveData[] persisterSaveData;

        public SaveData() { }
    }

    // Using a struct because variables are instantiated all at the same time.
    [Serializable] public struct IDataPersisterSaveData
    {
        public string className;
        public string dataSettingsTag;
        public DataSettings.PersistenceType dataSettingsPersistenceType;
        public string[] dataValues;

        public IDataPersisterSaveData(string className, DataSettings dataSettings, string[] data)
        {
            this.className = className;
            this.dataSettingsTag = dataSettings.dataTag;
            this.dataSettingsPersistenceType = dataSettings.persistenceType;
            this.dataValues = data;
        }
    }
}
