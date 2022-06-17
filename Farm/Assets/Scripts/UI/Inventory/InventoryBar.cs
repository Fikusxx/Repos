using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{

    // References
    private RectTransform rect;
    [SerializeField] private Sprite blankSprite;
    [SerializeField] private InventorySlot[] inventoryBarSlots;
    public GameObject draggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameobject;

    // Data
    private bool isBarPositionBottom = true;
    public bool IsBarPositionBottom { get => isBarPositionBottom; }


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        ClearInventorySlots();
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }


    private void Update()
    {
        // Switch inv bar for position depending on player pos
        SwitchInventoryBarPosition();
    }

    private void SwitchInventoryBarPosition()
    {
        var playerViewportPos = Player.Instance.GetPlayerViewportPosition(); // gets pos from 0 to 1 on X/Y axis

        if (playerViewportPos.y > 0.3f && isBarPositionBottom == false) // if bar is on top and player is positioned high enough
        {
            // changes anchors and pivot
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);

            rect.anchoredPosition = new Vector2(0f, 4f); // changes actual position on the screen

            isBarPositionBottom = true;
        }
        else if (playerViewportPos.y <= 0.3f && isBarPositionBottom == true)
        {
            // changes anchors and pivot
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);

            rect.anchoredPosition = new Vector2(0f, -4f); // changes actual position on the screen

            isBarPositionBottom = false;
        }
    }

    private void ClearInventorySlots()
    {
        if (inventoryBarSlots.Length > 0)
        {
            for (int i = 0; i < inventoryBarSlots.Length; i++)
            {
                inventoryBarSlots[i].invetorySlotImage.sprite = blankSprite;
                inventoryBarSlots[i].textTMP.text = "";
                inventoryBarSlots[i].itemDetails = null;
                inventoryBarSlots[i].itemQuantity = 0;
                SetHightlightedInventorySlots(i);
            }
        }
    }


    public void ClearHightlightOnInventorySlots()
    {
        if (inventoryBarSlots.Length > 0)
        {
            for (int i = 0; i < inventoryBarSlots.Length; i++)
            {
                if (inventoryBarSlots[i].isSelected)
                {
                    inventoryBarSlots[i].isSelected = false;
                    inventoryBarSlots[i].invetorySlotHightlight.color = new Color(0, 0, 0, 0);

                    // Update inventory to show items are not selected
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    public void SetHightlightedInventorySlots()
    {
        if (inventoryBarSlots.Length > 0)
        {
            for (int i = 0; i < inventoryBarSlots.Length; i++)
            {
                SetHightlightedInventorySlots(i);
            }
        }
    }

    private void SetHightlightedInventorySlots(int itemIndex)
    {
        if (inventoryBarSlots.Length > 0 && inventoryBarSlots[itemIndex].itemDetails != null)
        {
            if (inventoryBarSlots[itemIndex].isSelected)
            {
                inventoryBarSlots[itemIndex].invetorySlotHightlight.color = new Color(1, 1, 1, 1);

                // Update inventory to show items are not selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventoryBarSlots[itemIndex].itemDetails.itemCode);
            }
        }
    }

    private void InventoryUpdated(InventoryLocation location, List<InventoryItem> inventoryItems)
    {
        if (location == InventoryLocation.player) // check if it's player inventory, not his chest
        {
            ClearInventorySlots();

            if (inventoryBarSlots.Length > 0 && inventoryItems.Count > 0)
            {

                // loop thru inventory slots and update with corresponding inventory list item
                for (int i = 0; i < inventoryBarSlots.Length; i++)
                {
                    if (i < inventoryItems.Count)
                    {
                        int itemCode = inventoryItems[i].itemCode;

                        // ItemDetails itemDetails = InventoryManager.Instance.itemList.itemDetails.Find(x => x.itemCode == itemCode);
                        var itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            inventoryBarSlots[i].invetorySlotImage.sprite = itemDetails.itemSprite;
                            inventoryBarSlots[i].textTMP.text = inventoryItems[i].itemQuantity.ToString();
                            inventoryBarSlots[i].itemDetails = itemDetails;
                            inventoryBarSlots[i].itemQuantity = inventoryItems[i].itemQuantity;
                            SetHightlightedInventorySlots(i);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
