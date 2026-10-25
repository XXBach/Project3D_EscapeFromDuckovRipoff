using UnityEngine;

public class InventoryNode
{
    public Vector2Int ItemCoordinates { get; private set; }
    public Item StoredItem { get; private set; }
    public Grid<InventoryNode> Grid { get; private set; }

    public InventoryNode()
    {
        ItemCoordinates = new Vector2Int(-1,-1);
        StoredItem = null;
        Grid = null;
    }
    public InventoryNode(Vector2Int itemCoordinates, Item storedItem, Grid<InventoryNode> grid)
    {
        ItemCoordinates = itemCoordinates;
        StoredItem = storedItem;
        Grid = grid;
    }
    public InventoryNode(Vector2Int itemCoordinates, Item storedItem)
    {
        ItemCoordinates = itemCoordinates;
        StoredItem = storedItem;
        Grid = null;
    }
    public InventoryNode(Vector2Int itemCoordinates, Grid<InventoryNode> grid)
    {
        ItemCoordinates = itemCoordinates;
        StoredItem = null;
        Grid = grid;
    }
    public InventoryNode(Grid<InventoryNode> grid, Item storedItem)
    {
        ItemCoordinates = new Vector2Int(0, 0);
        StoredItem = storedItem;
        Grid = grid;
    }
    public InventoryNode(Vector2Int itemCoordinates)
    {
        ItemCoordinates = itemCoordinates;
        StoredItem = null;
        Grid = null;
    }
    public InventoryNode(Item storedItem)
    {
        ItemCoordinates = new Vector2Int(0, 0) ;
        StoredItem = storedItem;
        Grid = null;
    }
    public InventoryNode(Grid<InventoryNode> grid)
    {
        ItemCoordinates = new Vector2Int(0, 0);
        StoredItem = null;
        Grid = Grid;
    }
    public void SetItem(Item storedItem)
    {
        StoredItem = storedItem;
    }
    public void SetCoordinates(Vector2Int itemCoordinates)
    {
        ItemCoordinates = itemCoordinates;
    }
    public void RemoveItem()
    {
        StoredItem = null;
    }
    public void SetGrid(Grid<InventoryNode> grid)
    {
        Grid = grid;
    }
}
