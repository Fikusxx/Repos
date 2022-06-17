

public interface ISavable
{
    string ISavableUniqueID { get; set; }

    GameObjectSave GameObjectSave { get; set; }

    void ISavableRegister();

    void ISavableDeregister();

    void ISavableStoreScene(string sceneName);

    void ISavableRestoreScene(string sceneName);

}
