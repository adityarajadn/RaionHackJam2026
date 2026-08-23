using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int width = 1;
    public int height = 1;
    public int value = 0; // Nilai item
    public Sprite itemIcon;
}