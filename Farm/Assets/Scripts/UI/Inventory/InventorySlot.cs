using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera; // cached ref to main camera
    private Transform parentItem; // Empty GO at the scene where all default items are
    private GameObject draggedItem; // ref to object to drag thru methods

    private Canvas parentCanvas;

    public Image invetorySlotHightlight; // holds an image (a border) around the slot with Alpha 0-1
    public Image invetorySlotImage; // image that represents an item in that slot
    public TextMeshProUGUI textTMP; // text, that is quantity or number to drag/use item on a keyboard

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [HideInInspector] public bool isSelected = false;

    [SerializeField] private InventoryBar inventoryBar = null; // ref to invBar that holds "real" draggedItemPrefab
    [SerializeField] private GameObject itemPrefab = null; // core item prefab that gets filled with data 
    [SerializeField] private int slotNumber = 0;
    [SerializeField] private GameObject inventoryTextboxPrefab;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SceneLoaded;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            Player.Instance.DisablePlayerInputAndResetMovement();

            draggedItem = Instantiate(inventoryBar.draggedItem/*, inventoryBar.transform*/);


            // Get the image for the dragged item
            draggedItem.GetComponentInChildren<Image>().sprite = invetorySlotImage.sprite;

            // Set the item to be selected
            SetSeletectedItem();
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
            // get the slot number where the drag ended
            int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>().slotNumber;

            // Swap inventory items in inventory list
            InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

            // Destory inv text box
            DestroyInventoryTextBox();

            // Clear selected item
            ClearSelectedItem();

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


    private void ClearSelectedItem()
    {
        // Clear currently highlighted item
        inventoryBar.ClearHightlightOnInventorySlots();

        isSelected = false;

        // set no item selected in the inventory
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        // Clear player carrying item
        Player.Instance.ClearCarriedItem();
    }


    /// <summary>
    /// Set this inv slot item to be selected
    /// </summary>
    private void SetSeletectedItem()
    {
        // Clear previously highlighted item
        inventoryBar.ClearHightlightOnInventorySlots();
        isSelected = true;

        // Set highlighted inv slots
        inventoryBar.SetHightlightedInventorySlots();

        // Set item selected in inventory
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if (itemDetails.canBeCarried == true)
        {
            // Show player carrying item
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }


    /// <summary>
    /// Drops the item (if selected) at the current mouse pos. Called by the DropItem event
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

            // Create item from prefab at mouse pos
            GameObject itemGameObject = Instantiate(itemPrefab, worldPos, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            // Remove item from player inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

            // if no more of item then clear selected
            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }


    /// <summary>
    /// Highlight invSlot if we left click it and UNhightlight if it was highlighted already
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSeletectedItem();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Populate text box with item details
        if (itemQuantity != 0)
        {

            // Instantiate text box at the position of inventorySlot, parent it to canvas, set ref to invBar.GO
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextboxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            InventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<InventoryTextBox>();

            // Set item type description
            string itemTypeDesc = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            // Populate text box
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDesc, "", itemDetails.itemLongDescription, "", "");

            // Set text box position according to inventory bar position
            if (inventoryBar.IsBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f); // sets its pivot to bottom middle
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 30f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 30f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    private void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }

    private void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}


