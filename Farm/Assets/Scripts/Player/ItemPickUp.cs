using UnityEngine;


public class ItemPickUp : MonoBehaviour // sits on the player
{


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<Item>())
        {
            var item = collision.GetComponent<Item>(); // Get ItemDetails object out of Item we triggered with.

            // InventoryManager has all items in the game, so we just from SO list.
            // Get specific itemDetails object using itemCode from Item.
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // if an item can be picked up
            if (itemDetails.canBePickedUp == true)
            {
                // Add an item to the inventory
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject); // location player because this script is attached to the player
            }
        }
    }
}
