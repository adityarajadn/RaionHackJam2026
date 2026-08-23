using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public const float TILE_SIZE = 50f;

    private InventoryItem[,] gridArray;
    private RectTransform rectTransform;

    [Header("Visual Grid")]
    public bool showVisualGrid = true;
    public Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    public Color outlineColor = new Color(0f, 0f, 0f, 1f);

    [Header("UI")]
    public UnityEngine.UI.Text totalValueText;

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
        // Sesuaikan ukuran RectTransform otomatis berdasarkan ukuran grid dan TILE_SIZE
        rectTransform.sizeDelta = new Vector2(gridWidth * TILE_SIZE, gridHeight * TILE_SIZE);
    }

    private void CreateVisualGrid()
    {
        if (!showVisualGrid) return;

        // Buat container untuk visual grid agar tidak berantakan
        GameObject visualContainer = new GameObject("VisualGridContainer");
        RectTransform containerRect = visualContainer.AddComponent<RectTransform>();
        containerRect.SetParent(rectTransform, false);
        containerRect.localPosition = Vector3.zero;
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.sizeDelta = new Vector2(gridWidth * TILE_SIZE, gridHeight * TILE_SIZE);
        containerRect.SetAsFirstSibling(); // Pastikan visual grid ada di belakang item

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
                slotRect.localPosition = new Vector2(x * TILE_SIZE, -y * TILE_SIZE);

                UnityEngine.UI.Image img = slot.AddComponent<UnityEngine.UI.Image>();
                img.color = gridColor;

                UnityEngine.UI.Outline outline = slot.AddComponent<UnityEngine.UI.Outline>();
                outline.effectColor = outlineColor;
                outline.effectDistance = new Vector2(1, -1);
            }
        }
    }

    private void RebuildGridFromChildren()
    {
        // Kosongkan grid (berjaga-jaga)
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridArray[x, y] = null;
            }
        }

        // Cari semua InventoryItem yang ada sebagai child
        InventoryItem[] items = GetComponentsInChildren<InventoryItem>();
        foreach (InventoryItem item in items)
        {
            // Ambil posisi dari item (sudah diset sebelumnya di Editor)
            int x = item.onGridPositionX;
            int y = item.onGridPositionY;
            
            // Masukkan kembali ke dalam array 2D
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

    // Mengambil koordinat Grid (X, Y) berdasarkan posisi mouse di layar
    public Vector2Int GetGridPosition(Vector2 screenMousePosition)
    {
        InitializeIfNeeded();

        // Mengubah posisi layar mouse menjadi posisi lokal relatif terhadap Grid
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenMousePosition, null, out Vector2 localPos);
        
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Dapatkan pojok kiri atas dalam ruang lokal Grid
        Vector3 localTopLeft = rectTransform.InverseTransformPoint(corners[1]);
        
        float offsetX = localPos.x - localTopLeft.x;
        float offsetY = localTopLeft.y - localPos.y; // Y menurun
        
        int x = Mathf.FloorToInt(offsetX / TILE_SIZE);
        int y = Mathf.FloorToInt(offsetY / TILE_SIZE);
        
        return new Vector2Int(x, y);
    }

    // Cek apakah item bisa ditempatkan di koordinat (X, Y) tanpa menabrak atau keluar batas
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
                    return false; // overlapping
                }
            }
        }
        return true;
    }

    // Memasukkan item ke dalam array dan snap secara visual
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

        // Visual Snap yang akurat menggunakan math pivot
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.SetParent(rectTransform, false); // false agar local pos/scale tidak kacau
        
        Vector2 gridTopLeft = new Vector2(-rectTransform.sizeDelta.x * rectTransform.pivot.x, rectTransform.sizeDelta.y * (1f - rectTransform.pivot.y));
        Vector2 targetTopLeft = gridTopLeft + new Vector2(x * TILE_SIZE, -y * TILE_SIZE);
        
        Vector2 pivotOffset = new Vector2(itemRect.sizeDelta.x * itemRect.pivot.x, -itemRect.sizeDelta.y * (1f - itemRect.pivot.y));
        itemRect.localPosition = targetTopLeft + pivotOffset;

        UpdateTotalValue();

        return true;
    }

    // Mengangkat item dari grid
    public void RemoveItem(InventoryItem item)
    {
        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                gridArray[item.onGridPositionX + i, item.onGridPositionY + j] = null;
            }
        }
        
        UpdateTotalValue();
    }

    // Mengambil referensi item di koordinat spesifik
    public InventoryItem GetItemAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight) return null;
        return gridArray[x, y];
    }

    public void UpdateTotalValue()
    {
        if (totalValueText == null) return;

        int total = 0;
        System.Collections.Generic.HashSet<InventoryItem> countedItems = new System.Collections.Generic.HashSet<InventoryItem>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                InventoryItem item = gridArray[x, y];
                if (item != null && item.itemData != null && !countedItems.Contains(item))
                {
                    total += item.itemData.value;
                    countedItems.Add(item);
                }
            }
        }

        totalValueText.text = "Total Value: " + total.ToString();
    }
}