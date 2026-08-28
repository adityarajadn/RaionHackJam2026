using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(InventoryModel))]
public class InventoryUI : MonoBehaviour
{
    public InventoryGrid selectedGrid;
    [Header("UI")]
    public GameObject inventoryUI; 
    public static bool IsInventoryOpen { get; private set; }
    [Header("Item Spawning")]
    public GameObject inventoryItemPrefab; 
    public RectTransform stagingArea; 

    private InventoryItem selectedItem;
    private RectTransform selectedItemRect;
    private int originalGridX = -1;
    private int originalGridY = -1;
    private bool originalWasRotated = false;
    private RectTransform highlightRect;
    private Image highlightImage;
    
    private ItemDropper itemDropper;
    private InventoryModel inventoryModel;

    private void Awake()
    {
        itemDropper = GetComponent<ItemDropper>();
        inventoryModel = GetComponent<InventoryModel>();

        GameObject highlighter = new GameObject("Highlighter");
        highlightRect = highlighter.AddComponent<RectTransform>();
        highlightImage = highlighter.AddComponent<Image>();
        highlightImage.raycastTarget = false;
        highlightRect.pivot = new Vector2(0, 1); 
        highlightRect.anchorMin = new Vector2(0, 1);
        highlightRect.anchorMax = new Vector2(0, 1);
        highlighter.transform.SetParent(transform, false);
        highlighter.SetActive(false);
    }

    private void Update()
    {
        if (GameplayManager.Instance != null && GameplayManager.Instance.IsGameOver)
        {
            if (IsInventoryOpen) HideInventory();
            return;
        }
        if (Keyboard.current.bKey.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (IsInventoryOpen) HideInventory();
            else ShowInventory();
        }
        if (!IsInventoryOpen || selectedGrid == null) return;
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (selectedItem == null)
            HandlePickup(mousePosition);
        else
            HandleDragging(mousePosition);
    }

    private void HandlePickup(Vector2 mousePosition)
    {
        Vector2Int gridPos = selectedGrid.GetGridPosition(mousePosition);
        InventoryItem itemUnderMouse = selectedGrid.GetItemAt(gridPos.x, gridPos.y);
        
        if (itemUnderMouse != null)
        {
            ShowHighlight(itemUnderMouse.onGridPositionX, itemUnderMouse.onGridPositionY, itemUnderMouse.Width, itemUnderMouse.Height, true);
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectedItem = itemUnderMouse;
                selectedItemRect = selectedItem.GetComponent<RectTransform>();
                originalGridX = selectedItem.onGridPositionX;
                originalGridY = selectedItem.onGridPositionY;
                originalWasRotated = selectedItem.isRotated;
                
                inventoryModel.RemoveItem(selectedItem, selectedGrid);
                selectedItemRect.SetParent(transform, true); 
                selectedItemRect.localScale = Vector3.one;
                selectedItemRect.SetAsLastSibling(); 
                
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("drag&drop");
            }
        }
        else
        {
            highlightRect.gameObject.SetActive(false);
            if (Mouse.current.leftButton.wasPressedThisFrame && stagingArea != null)
            {
                foreach (Transform child in stagingArea)
                {
                    RectTransform childRect = child as RectTransform;
                    Camera uiCamera = null;
                    Canvas canvas = childRect.GetComponentInParent<Canvas>();
                    if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)) uiCamera = canvas.worldCamera;
                    if (childRect != null && RectTransformUtility.RectangleContainsScreenPoint(childRect, mousePosition, uiCamera))
                    {
                        selectedItem = child.GetComponent<InventoryItem>();
                        selectedItemRect = childRect;
                        originalGridX = -1;
                        originalGridY = -1;
                        selectedItemRect.SetParent(transform, true);
                        selectedItemRect.localScale = Vector3.one;
                        selectedItemRect.SetAsLastSibling();
                        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("drag&drop");
                        break;
                    }
                }
            }
        }
    }

    private void HandleDragging(Vector2 mousePosition)
    {
        RectTransform canvasRect = transform as RectTransform;
        Camera uiCamera = null;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)) uiCamera = canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, uiCamera, out Vector2 localMousePos);
        selectedItemRect.localPosition = localMousePos;
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            selectedItem.Rotate();
        }
        
        Vector2Int gridPos = selectedGrid.GetGridPosition(mousePosition);
        int startX = gridPos.x - (selectedItem.Width / 2);
        int startY = gridPos.y - (selectedItem.Height / 2);
        bool isValid = inventoryModel.IsValidPosition(selectedGrid, startX, startY, selectedItem.Width, selectedItem.Height);
        
        ShowHighlight(startX, startY, selectedItem.Width, selectedItem.Height, isValid);
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isValid)
            {
                inventoryModel.PlaceItem(selectedItem, selectedGrid, startX, startY);
                selectedItem = null;
                highlightRect.gameObject.SetActive(false);
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("drag&drop");
            }
            else
            {
                RectTransform gridRect = selectedGrid.GetComponent<RectTransform>();
                Camera gridUiCamera = null;
                Canvas gridCanvas = gridRect.GetComponentInParent<Canvas>();
                if (gridCanvas != null && (gridCanvas.renderMode == RenderMode.ScreenSpaceCamera || gridCanvas.renderMode == RenderMode.WorldSpace)) gridUiCamera = gridCanvas.worldCamera;
                
                if (!RectTransformUtility.RectangleContainsScreenPoint(gridRect, mousePosition, gridUiCamera))
                {
                    itemDropper.DropItemToWorld(selectedItem, mousePosition);
                    selectedItem = null;
                    highlightRect.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ShowHighlight(int x, int y, int width, int height, bool isValid)
    {
        highlightRect.gameObject.SetActive(true);
        highlightRect.SetParent(selectedGrid.transform, false);
        float hw = width * InventoryGrid.TILE_SIZE + Mathf.Max(0, width - 1) * selectedGrid.spacing;
        float hh = height * InventoryGrid.TILE_SIZE + Mathf.Max(0, height - 1) * selectedGrid.spacing;
        highlightRect.sizeDelta = new Vector2(hw, hh);
        highlightImage.color = isValid ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
        RectTransform gridRect = selectedGrid.GetComponent<RectTransform>();
        Vector2 gridTopLeft = new Vector2(-gridRect.sizeDelta.x * gridRect.pivot.x, gridRect.sizeDelta.y * (1f - gridRect.pivot.y));
        Vector2 targetTopLeft = gridTopLeft + new Vector2(x * (InventoryGrid.TILE_SIZE + selectedGrid.spacing), -y * (InventoryGrid.TILE_SIZE + selectedGrid.spacing));
        Vector2 pivotOffset = new Vector2(highlightRect.sizeDelta.x * highlightRect.pivot.x, -highlightRect.sizeDelta.y * (1f - highlightRect.pivot.y));
        highlightRect.localPosition = targetTopLeft + pivotOffset;
        highlightRect.SetAsLastSibling(); 
    }

    public void ShowInventory()
    {
        IsInventoryOpen = true;
        if (inventoryUI != null) inventoryUI.SetActive(true);
        Time.timeScale = 0f; 
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("open&close inventory");
    }

    public void HideInventory()
    {
        IsInventoryOpen = false;
        if (inventoryUI != null) inventoryUI.SetActive(false);
        if (highlightRect != null) highlightRect.gameObject.SetActive(false);
        Time.timeScale = 1f; 
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("open&close inventory");
        
        if (stagingArea != null)
        {
            foreach (Transform child in stagingArea)
            {
                InventoryItem item = child.GetComponent<InventoryItem>();
                if (item != null && item.linkedWorldItem != null)
                {
                    item.linkedWorldItem.ShowWorldItem();
                }
                Destroy(child.gameObject);
            }
        }
        
        if (selectedItem != null)
        {
            if (selectedItem.linkedWorldItem != null)
            {
                selectedItem.linkedWorldItem.ShowWorldItem();
                Destroy(selectedItem.gameObject);
            }
            else
            {
                if (originalGridX != -1 && originalGridY != -1)
                {
                    if (selectedItem.isRotated != originalWasRotated) selectedItem.Rotate();
                    inventoryModel.PlaceItem(selectedItem, selectedGrid, originalGridX, originalGridY);
                }
                else
                {
                    Destroy(selectedItem.gameObject);
                }
            }
            selectedItem = null;
        }
    }

    public void ReceiveWorldItem(InteractableWorldItem worldItem)
    {
        ShowInventory(); 
        if (inventoryItemPrefab == null) return;
        
        if (stagingArea == null && selectedGrid != null)
        {
            GameObject stagingObj = new GameObject("StagingArea");
            stagingArea = stagingObj.AddComponent<RectTransform>();
            stagingArea.SetParent(selectedGrid.transform.parent, false);
            stagingArea.sizeDelta = new Vector2(200, 200);
            RectTransform gridRect = selectedGrid.GetComponent<RectTransform>();
            float offset = (gridRect.sizeDelta.x / 2f) + 150f;
            stagingArea.localPosition = gridRect.localPosition - new Vector3(offset, 0, 0);
        }
        
        GameObject newItemObj = Instantiate(inventoryItemPrefab, stagingArea);
        InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();
        newItem.Initialize(worldItem.itemData);
        newItem.linkedWorldItem = worldItem;
        RectTransform itemRect = newItemObj.GetComponent<RectTransform>();
        
        if (stagingArea.GetComponent<UnityEngine.UI.LayoutGroup>() == null)
        {
            itemRect.localPosition = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);
        }
    }
}
