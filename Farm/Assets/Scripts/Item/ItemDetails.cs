using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // makes it visible on the Inspector as a whole
public class ItemDetails // contains all properties for an item. I think there cna be many classes like this, all with its own properties
{

    // Core Data
    public int itemCode;
    public ItemType itemType;
    public bool isStartingItem;


    // Pickable / Dropable
    public bool canBeDropped;
    public bool canBePickedUp;


    // Description info
    [TextArea(1, 3)] public string itemDescription;
    [TextArea(5,5)]public string itemLongDescription;


    // Usage logic
    public bool canBeEaten;
    public bool canBeCarried;
    public float itemUseRadius;
    public short itemUseGridRadius;
    

    // References
    public Sprite itemSprite;


}
