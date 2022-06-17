using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{

    public List<ISavable> iSavableObjectList;

    protected override void Awake()
    {
        base.Awake();
        iSavableObjectList = new List<ISavable>();
    }

    public void StoreCurrentSceneData()
    {
        // loop thru all ISavable objects and trigger STORE scene data for each
        foreach (ISavable iSavableObject in iSavableObjectList)
        {
            iSavableObject.ISavableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentStoreData()
    {
        // loop thru all ISavable objects and trigger RESTORE scene data for each
        foreach (ISavable iSavableObject in iSavableObjectList)
        {
            iSavableObject.ISavableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

}
