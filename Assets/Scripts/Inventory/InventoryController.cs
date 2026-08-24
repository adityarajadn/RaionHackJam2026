using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public InventoryGrid selectedGrid;
    
    [Header("UI")]
    public GameObject inventoryUI; // Panel utama inventory (misalnya background atau grid)
    private bool isInventoryOpen = true;

    [Header("Item Spawning")]
    public GameObject inventoryItemPrefab; // Prefab UI Item
    public RectTransform stagingArea; // Lokasi item yang baru dipungut (sebelah kiri)

    private InventoryItem selectedItem;
    private RectTransform selectedItemRect;
    
    // Menyimpan posisi asli item jika diambil dari grid
    private int originalGridX = -1;
    private int originalGridY = -1;
    private bool originalWasRotated = false;

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
        // Toggle inventory dengan tombol E
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (isInventoryOpen) HideInventory();
            else ShowInventory();
        }

        if (!isInventoryOpen || selectedGrid == null) return;
        
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
                
                // Simpan posisi aslinya agar bisa dikembalikan kalau batal ditaruh
                originalGridX = selectedItem.onGridPositionX;
                originalGridY = selectedItem.onGridPositionY;
                originalWasRotated = selectedItem.isRotated;
                
                selectedGrid.RemoveItem(selectedItem);
                
                selectedItemRect.SetParent(transform, true); // Pindah parent ke Canvas
                selectedItemRect.SetAsLastSibling(); // Agar berada di paling atas
            }
        }
        else
        {
            highlightRect.gameObject.SetActive(false);

            // Cek apakah klik pada item di staging area (sebelah kiri grid)
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
                        
                        // Item ini asalnya dari staging, bukan grid
                        originalGridX = -1;
                        originalGridY = -1;
                        
                        selectedItemRect.SetParent(transform, true);
                        selectedItemRect.SetAsLastSibling();
                        break;
                    }
                }
            }
        }
    }

    private void HandleDragging(Vector2 mousePosition)
    {
        // Posisikan item mengikuti kursor mouse (offset ke tengah)
        RectTransform canvasRect = transform as RectTransform;
        Camera uiCamera = null;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)) uiCamera = canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, uiCamera, out Vector2 localMousePos);
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
                
                // Kita tidak lagi menghancurkan linkedWorldItem di sini, 
                // melainkan membiarkannya tetap tersembunyi agar bisa di-drop ke dunia nanti.
                
                selectedItem = null;
                highlightRect.gameObject.SetActive(false);
            }
            else
            {
                // Jika klik di luar kotak grid, maka jatuhkan item ke dunia
                RectTransform gridRect = selectedGrid.GetComponent<RectTransform>();
                Camera gridUiCamera = null;
                Canvas gridCanvas = gridRect.GetComponentInParent<Canvas>();
                if (gridCanvas != null && (gridCanvas.renderMode == RenderMode.ScreenSpaceCamera || gridCanvas.renderMode == RenderMode.WorldSpace)) gridUiCamera = gridCanvas.worldCamera;

                if (!RectTransformUtility.RectangleContainsScreenPoint(gridRect, mousePosition, gridUiCamera))
                {
                    DropItemToWorld(mousePosition);
                }
            }
        }
    }

    private void DropItemToWorld(Vector2 mousePosition)
    {
        if (selectedItem.linkedWorldItem != null)
        {
            // Hitung posisi dunia berdasarkan klik mouse
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(mainCamera.transform.position.z)));
                worldPos.z = 0f; // Asumsi game 2D
                
                GameObject rootObj = selectedItem.linkedWorldItem.GetRootObject();
                rootObj.transform.position = worldPos;
                selectedItem.linkedWorldItem.ShowWorldItem();
            }
        }
        else
        {
            Debug.LogWarning("Item ini tidak memiliki referensi world item fisik, jadi langsung dihapus.");
        }

        Destroy(selectedItem.gameObject);
        selectedItem = null;
        highlightRect.gameObject.SetActive(false);
    }

    private void ShowHighlight(int x, int y, int width, int height, bool isValid)
    {
        highlightRect.gameObject.SetActive(true);
        highlightRect.SetParent(selectedGrid.transform, false);
        
        float hw = width * InventoryGrid.TILE_SIZE + Mathf.Max(0, width - 1) * selectedGrid.spacing;
        float hh = height * InventoryGrid.TILE_SIZE + Mathf.Max(0, height - 1) * selectedGrid.spacing;
        highlightRect.sizeDelta = new Vector2(hw, hh);
        
        // Hijau jika bisa ditaruh, Merah jika tabrakan
        highlightImage.color = isValid ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
        
        // Snap posisi
        RectTransform gridRect = selectedGrid.GetComponent<RectTransform>();
        Vector2 gridTopLeft = new Vector2(-gridRect.sizeDelta.x * gridRect.pivot.x, gridRect.sizeDelta.y * (1f - gridRect.pivot.y));
        
        // Gunakan spacing untuk menentukan posisi target
        Vector2 targetTopLeft = gridTopLeft + new Vector2(x * (InventoryGrid.TILE_SIZE + selectedGrid.spacing), -y * (InventoryGrid.TILE_SIZE + selectedGrid.spacing));
        
        Vector2 pivotOffset = new Vector2(highlightRect.sizeDelta.x * highlightRect.pivot.x, -highlightRect.sizeDelta.y * (1f - highlightRect.pivot.y));
        
        highlightRect.localPosition = targetTopLeft + pivotOffset;
        highlightRect.SetAsLastSibling(); // Agar di atas grid tapi di bawah item saat di-drop
    }

    public void ShowInventory()
    {
        isInventoryOpen = true;
        if (inventoryUI != null) inventoryUI.SetActive(true);
        Time.timeScale = 0f; // Freeze gameplay
    }

    public void HideInventory()
    {
        isInventoryOpen = false;
        if (inventoryUI != null) inventoryUI.SetActive(false);
        
        if (highlightRect != null) highlightRect.gameObject.SetActive(false);

        Time.timeScale = 1f; // Resume gameplay
        
        // Kembalikan item di staging area ke dunia
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
        
        // Jika sedang men-drag item, batalkan
        if (selectedItem != null)
        {
            if (selectedItem.linkedWorldItem != null)
            {
                selectedItem.linkedWorldItem.ShowWorldItem();
                Destroy(selectedItem.gameObject);
            }
            else
            {
                // Kembalikan ke posisi asal di grid
                if (originalGridX != -1 && originalGridY != -1)
                {
                    // Pastikan rotasinya juga kembali seperti semula
                    if (selectedItem.isRotated != originalWasRotated)
                    {
                        selectedItem.Rotate();
                    }
                    selectedGrid.PlaceItem(selectedItem, originalGridX, originalGridY);
                }
                else
                {
                    // Fallback jika tidak ada origin
                    Destroy(selectedItem.gameObject);
                }
            }
            selectedItem = null;
        }
    }

    public void ReceiveWorldItem(InteractableWorldItem worldItem)
    {
        ShowInventory(); // Buka inventory secara otomatis

        if (inventoryItemPrefab == null)
        {
            Debug.LogError("Inventory Item Prefab belum diset di InventoryController!");
            return;
        }

        // Bikin staging area otomatis kalau belum diset
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

        // Atur posisi acak sedikit agar kalau ada banyak item tidak menumpuk persis
        RectTransform itemRect = newItemObj.GetComponent<RectTransform>();
        if (stagingArea.GetComponent<UnityEngine.UI.LayoutGroup>() == null)
        {
            itemRect.localPosition = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);
        }
    }
}