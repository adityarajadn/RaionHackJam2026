using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    public InteractableWorldItem linkedWorldItem; // Referensi ke objek di dunia game
    public int onGridPositionX;
    public int onGridPositionY;
    public bool isRotated = false;

    // Lebar dan tinggi dinamis tergantung status rotasi
    public int Width => isRotated ? itemData.height : itemData.width;
    public int Height => isRotated ? itemData.width : itemData.height;

    public const float TILE_SIZE = 50f;

    private RectTransform rectTransform;
    private Image itemImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (itemImage == null && transform.childCount > 0)
        {
            itemImage = transform.GetChild(0).GetComponent<Image>();
        }
    }

    public void Initialize(ItemData data)
    {
        this.itemData = data;
        
        // Cek apakah itemImage sudah ada (dari prefab). Jika belum, buat baru.
        if (itemImage == null)
        {
            GameObject imageObj = new GameObject("ItemImage");
            imageObj.transform.SetParent(transform, false);
            itemImage = imageObj.AddComponent<Image>();
            
            // Setup default pivot hanya jika ini adalah objek baru
            RectTransform childRect = itemImage.GetComponent<RectTransform>();
            childRect.anchorMin = new Vector2(0.5f, 0.5f);
            childRect.anchorMax = new Vector2(0.5f, 0.5f);
            childRect.pivot = new Vector2(0.5f, 0.5f);
            childRect.sizeDelta = new Vector2(data.width * TILE_SIZE, data.height * TILE_SIZE);
            childRect.localPosition = Vector2.zero;
        }
        else
        {
            // Jika sudah ada (dari prefab), sesuaikan saja ukurannya
            RectTransform childRect = itemImage.GetComponent<RectTransform>();
            childRect.sizeDelta = new Vector2(data.width * TILE_SIZE, data.height * TILE_SIZE);
            childRect.anchoredPosition = Vector2.zero;
        }
        
        itemImage.sprite = data.itemIcon;
        itemImage.preserveAspect = true;
        // Warna item sedikit semi transparan jika mau, tapi ini defaultnya solid
        itemImage.raycastTarget = false; // Karena parent (InventoryItem) yang jadi target kalau ada

        // Atur ukuran utama
        RefreshVisuals();
    }

    public void Rotate()
    {
        isRotated = !isRotated;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        // Sesuaikan ukuran parent (bounding box) agar sesuai grid
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(Width * TILE_SIZE, Height * TILE_SIZE);
        }
        
        // Rotasi anak gambar secara visual
        if (itemImage != null)
        {
            itemImage.rectTransform.localEulerAngles = new Vector3(0, 0, isRotated ? -90f : 0f);
        }
    }
}