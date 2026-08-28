using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    // InventoryModel acts as the central data handler, delegating to the grid
    public bool IsValidPosition(InventoryGrid grid, int x, int y, int width, int height)
    {
        return grid.IsValidPosition(x, y, width, height);
    }

    public void PlaceItem(InventoryItem item, InventoryGrid grid, int x, int y)
    {
        grid.PlaceItem(item, x, y);
    }

    public void RemoveItem(InventoryItem item, InventoryGrid grid)
    {
        grid.RemoveItem(item);
    }
}
