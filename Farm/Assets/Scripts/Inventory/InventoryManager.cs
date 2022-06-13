using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{

    [SerializeField] private SO_ItemList itemList = null;
    private Dictionary<int, ItemDetails> itemDetailsDictionary;



    private void Start()
    {
        // Create AND fill item details dictionary
        CreateItemDetailsDictionary();
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
}
