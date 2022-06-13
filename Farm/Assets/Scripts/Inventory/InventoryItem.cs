using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InventoryItem
{
    public int itemCode; // make it get; private set;
    public int itemQuantity; // make it get; private set;

    public InventoryItem(int itemCode, int itemQuantity)
    {
        this.itemCode = itemCode;
        this.itemQuantity = itemQuantity;
    }
}
