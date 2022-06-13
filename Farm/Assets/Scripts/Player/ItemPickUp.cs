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


            // Debug it's name for now
            Debug.Log(itemDetails.itemDescription);
        }
    }
}
