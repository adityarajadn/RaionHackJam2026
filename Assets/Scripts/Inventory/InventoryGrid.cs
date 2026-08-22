using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public const float TILE_SIZE = 50f;

    private InventoryItem[,] gridArray;
    private RectTransform rectTransform;

    private void Awake()
    {
        InitializeIfNeeded();
        RebuildGridFromChildren();
    }

    private void InitializeIfNeeded()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (gridArray == null) gridArray = new InventoryItem[gridWidth, gridHeight];
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

        // Visual Snap yang akurat tanpa peduli Anchor/Pivot
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.SetParent(rectTransform, true);
        
        Vector3[] gridCorners = new Vector3[4];
        rectTransform.GetWorldCorners(gridCorners);
        Vector3 localGridTopLeft = rectTransform.InverseTransformPoint(gridCorners[1]);
        
        Vector3[] itemCorners = new Vector3[4];
        itemRect.GetWorldCorners(itemCorners);
        Vector3 localItemTopLeft = rectTransform.InverseTransformPoint(itemCorners[1]);
        
        Vector2 targetTopLeft = new Vector2(localGridTopLeft.x + (x * TILE_SIZE), localGridTopLeft.y - (y * TILE_SIZE));
        Vector2 pivotOffset = (Vector2)itemRect.localPosition - (Vector2)localItemTopLeft;
        
        itemRect.localPosition = targetTopLeft + pivotOffset;

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
    }

    // Mengambil referensi item di koordinat spesifik
    public InventoryItem GetItemAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight) return null;
        return gridArray[x, y];
    }
}