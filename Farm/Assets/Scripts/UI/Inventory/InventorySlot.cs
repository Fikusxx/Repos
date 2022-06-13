using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera; // cached ref to main camera
    private Transform parentItem; // Empty GO at the scene where all default items are
    private GameObject draggedItem; // ref to object to drag thru methods

    public Image invetorySlotHightlight; // holds an image (a border) around the slot with Alpha 0-1
    public Image invetorySlotImage; // image that represents an item in that slot
    public TextMeshProUGUI textTMP; // text, that is quantity or number to drag/use item on a keyboard

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    [SerializeField] private InventoryBar inventoryBar = null; // ref to invBar that holds "real" draggedItemPrefab
    [SerializeField] private GameObject itemPrefab = null; // core item prefab that gets filled with data 


    private void Start()
    {
        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            Player.Instance.DisablePlayerInputAndResetMovement();

            draggedItem = Instantiate(inventoryBar.draggedItem/*, inventoryBar.transform*/);


            // Get the image for the dragged item
            draggedItem.GetComponentInChildren<Image>().sprite = invetorySlotImage.sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Destroy game object as dragged item
        if (draggedItem != null)
        {
            Destroy(draggedItem);
        }

        // if drag ends over inventory bar, get item drag is over and swap them
        if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() != null)
        {

        }
        // else attempt to drop the item if ut can be dropped
        else
        {
            if (itemDetails.canBeDropped == true)
            {
                DropSelectedItemAtMousePosition();
            }
        }

        // Enable player input back
        Player.Instance.EnablePlayerInput();
    }

    /// <summary>
    /// Drops the item (if selected) at the current mouse pos. Called by the DropItem event
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

            // Create item from prefab at mouse pos
            GameObject itemGameObject = Instantiate(itemPrefab, worldPos, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            // Remove item from player inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);
        }
    }
}


