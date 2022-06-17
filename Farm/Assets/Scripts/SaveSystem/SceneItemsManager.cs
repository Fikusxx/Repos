using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISavable
{

    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string iSavableUniqueID;
    public string ISavableUniqueID { get => iSavableUniqueID; set => iSavableUniqueID = value; }

    private GameObjectSave gameObjectSave;
    public GameObjectSave GameObjectSave { get => gameObjectSave; set => gameObjectSave = value; }


    protected override void Awake()
    {
        base.Awake();

        ISavableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISavableRegister();
        EventHandler.AfterSceneLoadedEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        ISavableDeregister();
        EventHandler.AfterSceneLoadedEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    /// <summary>
    ///  Destroy items currently in the scene
    /// </summary>
    private void DestroySceneItems()
    {
        // Get all items in the scene
        Item[] itemsInScene = FindObjectsOfType<Item>();

        // Destroy them all xd
        for (int i = 0; i < itemsInScene.Length; i++)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    public void InstantiateSceneItems(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    public void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.Init(item.ItemCode);
        }       
    }




    public void ISavableDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }

    public void ISavableRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }

    public void ISavableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItemDictionary != null && sceneSave.listSceneItemDictionary.TryGetValue("sceneItemList", out List<SceneItem> sceneItemList))
            {
                // scene list items found - destroy existing  items in the scene
                DestroySceneItems();

                // now instantiate the list of scene items
                InstantiateSceneItems(sceneItemList);
            }
        }
    }

    public void ISavableStoreScene(string sceneName)
    {
        // Remove old scene save for gameObject if exists
        GameObjectSave.sceneData.Remove(sceneName);

        // Get all items in the scene
        var sceneItemList = new List<SceneItem>();
        var itemsInScene = FindObjectsOfType<Item>();

        // Loop thru all scene items

        foreach (var item in itemsInScene)
        {
            var sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            // Add sccene item to list
            sceneItemList.Add(sceneItem);
        }

        // Create list scene items dictionary in scene save and add to it
        var sceneSave = new SceneSave();
        sceneSave.listSceneItemDictionary = new Dictionary<string, List<SceneItem>>();
        sceneSave.listSceneItemDictionary.Add("sceneItemList", sceneItemList);

        // Add scene save to gameobject
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
