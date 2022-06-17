using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{

    [SerializeField] private SO_ItemList itemList = null;
    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    private int[] selectedInventoryItems;

    public List<InventoryItem>[] inventoryLists; // inventory lists for different locations, like Player and Chest
    [HideInInspector] public int[] invetoryListCapacityInArray; // defines capacity of a specific inventoryList above


    protected override void Awake()
    {
        base.Awake();

        // Create Inventory lists
        CreateInventoryLists();

        // Create AND fill item details dictionary
        CreateItemDetailsDictionary();

        // Initialise selected inventory item array
        selectedInventoryItems = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryItems.Length; i++)
        {
            selectedInventoryItems[i] = -1;
        }
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count]; // sets how many locations player can hold items at = 2;

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        // Initialise Inventory list capacity array
        invetoryListCapacityInArray = new int[(int)InventoryLocation.count]; // sets how many locations player can hold items at = 2;

        // Initialise player inventory list capacity
        invetoryListCapacityInArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity; // sets how many items player can hold at certain loc
    }



    /// <summary>
    /// Fills the itemDetails dictionary from the SO item list we created before (there are all items in the game?)
    /// </summary>
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (var itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }



    /// <summary>
    /// Adds an item to the inventory list for the inventory location and then destroys the gameObjectToDelete
    /// </summary>
    public void AddItem(InventoryLocation location, Item item, GameObject gameObjectToDelete)
    {
        AddItem(location, item);
        Destroy(gameObjectToDelete);
    }



    /// <summary>
    /// Add an item to the inventory lists for the inventoryLocation
    /// </summary>
    public void AddItem(InventoryLocation location, Item item)
    {
        int itemCode = item.ItemCode;

        List<InventoryItem> inventoryItems = inventoryLists[(int)location];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(location, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryItems, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryItems, itemCode);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(location, inventoryLists[(int)location]);
    }


    /// <summary>
    /// Swap item at fromItem index with item at toItem index in inventoryLocation inventory list
    /// </summary>
    public void SwapInventoryItems(InventoryLocation location, int fromItem, int toItem)
    {
        // if fromItem index and toItem index are within the bounds of the list, not the same, and greater or equal to zero
        if (fromItem < inventoryLists[(int)location].Count && toItem < inventoryLists[(int)location].Count && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)location][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)location][toItem];

            inventoryLists[(int)location][toItem] = fromInventoryItem;
            inventoryLists[(int)location][fromItem] = toInventoryItem;

            // Send event that inventory has been updated
            EventHandler.CallInventoryUpdatedEvent(location, inventoryLists[(int)location]);
        }
    }


    /// <summary>
    /// Find if itemCode is already in the inventory. Returns the item position
    /// in the inventory, or -1 if the item isnt there
    /// </summary>
    public int FindItemInInventory(InventoryLocation location, int itemCode)
    {
        int itemIndex = inventoryLists[(int)location].FindIndex(x => x.itemCode == itemCode);
        return itemIndex;
    }



    /// <summary>
    /// Adds an item at a specific position (index)
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryItems, int itemCode, int itemPosition)
    {
        int quantity = inventoryItems[itemPosition].itemQuantity + 1; // increase quantity that already in that "place" + 1
        var inventoryItem = new InventoryItem(itemCode, quantity);

        inventoryItems[itemPosition] = inventoryItem;
    }


    /// <summary>
    /// Adds an item at next position available. Need to check for available free space?
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryItems, int itemCode)
    {
        var inventoryItem = new InventoryItem(itemCode, 1);
        inventoryItems.Add(inventoryItem);
    }


    /// <summary>
    /// Returns the itemDetails from SO itemList depending on the itemCode. Or null if the item code doesnt exist
    /// </summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Get the item type description for an item type - returns the item type desc as a string for a given itemType
    /// </summary>
    public string GetItemTypeDescription(ItemType type)
    {
        return type switch
        {
            ItemType.Breaking_tool => Settings.BreakingTool,
            ItemType.Chopping_tool => Settings.ChoppingTool,
            ItemType.Hoeing_tool => Settings.HoeingTool,
            ItemType.Reaping_tool => Settings.ReapingTool,
            ItemType.Watering_tool => Settings.WateringTool,
            ItemType.Collecting_tool => Settings.CollectingTool,
            _ => type.ToString()
        };
    }


    /// <summary>
    /// Remove an item from the inventory, and create a GO at the position it was dropped
    /// </summary>
    public void RemoveItem(InventoryLocation location, int itemCode)
    {
        List<InventoryItem> inventoryItems = inventoryLists[(int)location];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(location, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryItems, itemCode, itemPosition);
        }

        // Send event that inventory has been update
        EventHandler.CallInventoryUpdatedEvent(location, inventoryItems);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryItems, int itemCode, int itemPosition)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryItems[itemPosition].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryItems[itemPosition] = inventoryItem;
        }
        else
        {
            inventoryItems.RemoveAt(itemPosition);
        }
    }


    /// <summary>
    /// Set the selected inventory item for inventoryLocation to itemCode
    /// </summary>
    public void SetSelectedInventoryItem(InventoryLocation location, int itemCode)
    {
        selectedInventoryItems[(int)location] = itemCode;
    }

    /// <summary>
    /// Clear the selected inv item for inventoryLocation
    /// </summary>
    public void ClearSelectedInventoryItem(InventoryLocation location)
    {
        selectedInventoryItems[(int)location] = -1;
    }
}
