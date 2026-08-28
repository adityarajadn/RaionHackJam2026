using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public const float TILE_SIZE = 50f;
    public float spacing = 0f; 
    private InventoryItem[,] gridArray;
    private RectTransform rectTransform;
    [Header("Visual Grid")]
    public bool showVisualGrid = true;
    [Tooltip("Sprite untuk tiap kotak (ukuran ideal 50x50 atau sebanding TILE_SIZE)")]
    public Sprite slotSprite;
    public Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    public bool useOutline = true;
    public Color outlineColor = new Color(0f, 0f, 0f, 1f);
    [Header("UI")]
    public UnityEngine.UI.Text totalValueText;
    public UnityEngine.UI.Slider weightSlider;
    public UnityEngine.UI.Text weightText;
    private void Awake()
    {
        InitializeIfNeeded();
        AdjustRectTransform();
        CreateVisualGrid();
        RebuildGridFromChildren();
    }
    private void InitializeIfNeeded()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (gridArray == null) gridArray = new InventoryItem[gridWidth, gridHeight];
    }
    private void AdjustRectTransform()
    {
        float width = (gridWidth * TILE_SIZE) + (Mathf.Max(0, gridWidth - 1) * spacing);
        float height = (gridHeight * TILE_SIZE) + (Mathf.Max(0, gridHeight - 1) * spacing);
        rectTransform.sizeDelta = new Vector2(width, height);
    }
    private void CreateVisualGrid()
    {
        Transform existingBg = rectTransform.Find("InventoryBackground");
        if (existingBg != null) Destroy(existingBg.gameObject);
        Transform existingContainer = rectTransform.Find("VisualGridContainer");
        if (existingContainer != null) Destroy(existingContainer.gameObject);
        GameObject visualContainer = new GameObject("VisualGridContainer");
        RectTransform containerRect = visualContainer.AddComponent<RectTransform>();
        containerRect.SetParent(rectTransform, false);
        containerRect.localPosition = Vector3.zero;
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        float totalWidth = (gridWidth * TILE_SIZE) + (Mathf.Max(0, gridWidth - 1) * spacing);
        float totalHeight = (gridHeight * TILE_SIZE) + (Mathf.Max(0, gridHeight - 1) * spacing);
        containerRect.sizeDelta = new Vector2(totalWidth, totalHeight);
        containerRect.SetAsFirstSibling(); 
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject slot = new GameObject($"Slot_{x}_{y}");
                RectTransform slotRect = slot.AddComponent<RectTransform>();
                slotRect.SetParent(containerRect, false);
                slotRect.anchorMin = new Vector2(0, 1);
                slotRect.anchorMax = new Vector2(0, 1);
                slotRect.pivot = new Vector2(0, 1);
                slotRect.sizeDelta = new Vector2(TILE_SIZE, TILE_SIZE);
                slotRect.localPosition = new Vector2(x * (TILE_SIZE + spacing), -y * (TILE_SIZE + spacing));
                UnityEngine.UI.Image img = slot.AddComponent<UnityEngine.UI.Image>();
                if (slotSprite != null)
                {
                    img.sprite = slotSprite;
                    img.type = UnityEngine.UI.Image.Type.Sliced; 
                }
                img.color = gridColor;
                if (useOutline)
                {
                    UnityEngine.UI.Outline outline = slot.AddComponent<UnityEngine.UI.Outline>();
                    outline.effectColor = outlineColor;
                    outline.effectDistance = new Vector2(1, -1);
                }
            }
        }
    }
    private void RebuildGridFromChildren()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridArray[x, y] = null;
            }
        }
        InventoryItem[] items = GetComponentsInChildren<InventoryItem>();
        foreach (InventoryItem item in items)
        {
            int x = item.onGridPositionX;
            int y = item.onGridPositionY;
            if (!IsValidPosition(x, y, item.Width, item.Height))
            {
                continue; 
            }
            for (int i = 0; i < item.Width; i++)
            {
                for (int j = 0; j < item.Height; j++)
                {
                    if (x + i < gridWidth && y + j < gridHeight)
                    {
                        gridArray[x + i, y + j] = item;
                    }
                }
            }
        }
        UpdateTotalValue();
    }
    public Vector2Int GetGridPosition(Vector2 screenMousePosition)
    {
        InitializeIfNeeded();
        Camera uiCamera = null;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace))
        {
            uiCamera = canvas.worldCamera;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenMousePosition, uiCamera, out Vector2 localPos);
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 localTopLeft = rectTransform.InverseTransformPoint(corners[1]);
        float offsetX = localPos.x - localTopLeft.x;
        float offsetY = localTopLeft.y - localPos.y; 
        float cellStep = TILE_SIZE + spacing;
        int x = Mathf.FloorToInt(offsetX / cellStep);
        if (offsetX % cellStep > TILE_SIZE && x < gridWidth - 1) x = Mathf.FloorToInt((offsetX + spacing) / cellStep);
        int y = Mathf.FloorToInt(offsetY / cellStep);
        if (offsetY % cellStep > TILE_SIZE && y < gridHeight - 1) y = Mathf.FloorToInt((offsetY + spacing) / cellStep);
        return new Vector2Int(x, y);
    }
    public bool IsValidPosition(int x, int y, int width, int height)
    {
        InitializeIfNeeded();
        if (x < 0 || y < 0 || x + width > gridWidth || y + height > gridHeight) return false;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gridArray[x + i, y + j] != null)
                {
                    return false; 
                }
            }
        }
        return true;
    }
    public bool PlaceItem(InventoryItem item, int x, int y)
    {
        InitializeIfNeeded();
        if (!IsValidPosition(x, y, item.Width, item.Height)) return false;
        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                gridArray[x + i, y + j] = item;
            }
        }
        item.onGridPositionX = x;
        item.onGridPositionY = y;
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.SetParent(rectTransform, false); 
        itemRect.localScale = Vector3.one;
        Vector2 gridTopLeft = new Vector2(-rectTransform.sizeDelta.x * rectTransform.pivot.x, rectTransform.sizeDelta.y * (1f - rectTransform.pivot.y));
        Vector2 targetTopLeft = gridTopLeft + new Vector2(x * (TILE_SIZE + spacing), -y * (TILE_SIZE + spacing));
        Vector2 pivotOffset = new Vector2(itemRect.sizeDelta.x * itemRect.pivot.x, -itemRect.sizeDelta.y * (1f - itemRect.pivot.y));
        itemRect.localPosition = targetTopLeft + pivotOffset;
        UpdateTotalValue();
        return true;
    }
    public void RemoveItem(InventoryItem item)
    {
        if (item == null) return;
        int startX = item.onGridPositionX;
        int startY = item.onGridPositionY;
        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                int x = startX + i;
                int y = startY + j;
                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    if (gridArray[x, y] == item)
                    {
                        gridArray[x, y] = null;
                    }
                }
            }
        }
        UpdateTotalValue();
    }
    public InventoryItem GetItemAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight) return null;
        return gridArray[x, y];
    }
    public void UpdateTotalValue()
    {
        int total = 0;
        float totalW = 0f;
        System.Collections.Generic.HashSet<InventoryItem> countedItems = new System.Collections.Generic.HashSet<InventoryItem>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                InventoryItem item = gridArray[x, y];
                if (item != null && item.itemData != null && !countedItems.Contains(item))
                {
                    total += item.itemData.value;
                    totalW += item.itemData.weight;
                    countedItems.Add(item);
                }
            }
        }
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.TotalScore = total;
            GameplayManager.Instance.TotalWeight = totalW;
        }
        if (totalValueText != null)
        {
            totalValueText.text = "Total Value: " + total.ToString();
        }
        if (weightSlider != null && GameplayManager.Instance != null)
        {
            weightSlider.maxValue = GameplayManager.Instance.MaxWeight;
            weightSlider.value = totalW;
        }
        if (weightText != null)
        {
            weightText.text = "Weight: " + totalW.ToString("F1") + " kg";
        }
    }
}