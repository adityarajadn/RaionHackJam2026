using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public InventoryGrid selectedGrid;
    
    private InventoryItem selectedItem;
    private RectTransform selectedItemRect;

    // Highlighter untuk menunjukkan apakah posisi valid
    private RectTransform highlightRect;
    private Image highlightImage;

    private void Awake()
    {
        // Buat objek highlighter secara dinamis
        GameObject highlighter = new GameObject("Highlighter");
        highlightRect = highlighter.AddComponent<RectTransform>();
        highlightImage = highlighter.AddComponent<Image>();
        highlightImage.raycastTarget = false;
        
        highlightRect.pivot = new Vector2(0, 1); // Anchor Top-Left
        highlightRect.anchorMin = new Vector2(0, 1);
        highlightRect.anchorMax = new Vector2(0, 1);
        
        highlighter.transform.SetParent(transform, false);
        highlighter.SetActive(false);
    }

    private void Update()
    {
        if (selectedGrid == null) return;
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (selectedItem == null)
        {
            HandlePickup(mousePosition);
        }
        else
        {
            HandleDragging(mousePosition);
        }
    }

    private void HandlePickup(Vector2 mousePosition)
    {
        Vector2Int gridPos = selectedGrid.GetGridPosition(mousePosition);
        InventoryItem itemUnderMouse = selectedGrid.GetItemAt(gridPos.x, gridPos.y);

        if (itemUnderMouse != null)
        {
            // Tampilkan highlight hijau di atas item yang di-hover
            ShowHighlight(itemUnderMouse.onGridPositionX, itemUnderMouse.onGridPositionY, itemUnderMouse.Width, itemUnderMouse.Height, true);
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectedItem = itemUnderMouse;
                selectedItemRect = selectedItem.GetComponent<RectTransform>();
                
                selectedGrid.RemoveItem(selectedItem);
                
                selectedItemRect.SetParent(transform, true); // Pindah parent ke Canvas
                selectedItemRect.SetAsLastSibling(); // Agar berada di paling atas
            }
        }
        else
        {
            highlightRect.gameObject.SetActive(false);
        }
    }

    private void HandleDragging(Vector2 mousePosition)
    {
        // Posisikan item mengikuti kursor mouse (offset ke tengah)
        RectTransform canvasRect = transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, null, out Vector2 localMousePos);
        selectedItemRect.localPosition = localMousePos;

        // Rotasi dengan menekan tombol R
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            selectedItem.Rotate();
        }

        // Kalkulasi posisi grid untuk penempatan
        Vector2Int gridPos = selectedGrid.GetGridPosition(mousePosition);
        
        // Kita hitung titik awal X dan Y (pojok kiri atas) sehingga mouse berada di tengah item
        int startX = gridPos.x - (selectedItem.Width / 2);
        int startY = gridPos.y - (selectedItem.Height / 2);

        // Jika mouse ada di luar bounds sedikit, kita cap (batasi)
        bool isValid = selectedGrid.IsValidPosition(startX, startY, selectedItem.Width, selectedItem.Height);

        // Tampilkan highlight
        ShowHighlight(startX, startY, selectedItem.Width, selectedItem.Height, isValid);

        // Jatuhkan item
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isValid)
            {
                selectedGrid.PlaceItem(selectedItem, startX, startY);
                selectedItem = null;
                highlightRect.gameObject.SetActive(false);
            }
        }
    }

    private void ShowHighlight(int x, int y, int width, int height, bool isValid)
    {
        highlightRect.gameObject.SetActive(true);
        highlightRect.SetParent(selectedGrid.transform, false);
        
        highlightRect.sizeDelta = new Vector2(width * InventoryGrid.TILE_SIZE, height * InventoryGrid.TILE_SIZE);
        
        // Hijau jika bisa ditaruh, Merah jika tabrakan
        highlightImage.color = isValid ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
        
        // Snap posisi
        Vector3[] gridCorners = new Vector3[4];
        selectedGrid.GetComponent<RectTransform>().GetWorldCorners(gridCorners);
        Vector3 localGridTopLeft = selectedGrid.GetComponent<RectTransform>().InverseTransformPoint(gridCorners[1]);

        highlightRect.localPosition = new Vector2(localGridTopLeft.x + (x * InventoryGrid.TILE_SIZE), localGridTopLeft.y - (y * InventoryGrid.TILE_SIZE));
        highlightRect.SetAsLastSibling(); // Agar di atas grid tapi di bawah item saat di-drop
    }
}