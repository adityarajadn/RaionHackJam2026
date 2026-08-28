using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    public void DropItemToWorld(InventoryItem selectedItem, Vector2 mousePosition)
    {
        if (selectedItem.linkedWorldItem != null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(mainCamera.transform.position.z)));
                worldPos.z = 0f; 
                GameObject rootObj = selectedItem.linkedWorldItem.GetRootObject();
                rootObj.transform.position = worldPos;
                selectedItem.linkedWorldItem.ShowWorldItem();
                
                Rigidbody2D rb = rootObj.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = rootObj.AddComponent<Rigidbody2D>();
                }
            }
        }
        Destroy(selectedItem.gameObject);
    }
}
