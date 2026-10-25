using UnityEngine;

abstract class Inventory : MonoBehaviour
{
    private int _width = 20;
    private int _height = 20;
    private int _cellSize = 1;
    [SerializeField] private Font _font;
    public Grid<InventoryNode> _inventoryGrid;

    public virtual void GridSetup()
    {
        _inventoryGrid = new Grid<InventoryNode>(_width, _height, _cellSize, _font, Vector3.zero, (Grid<InventoryNode> g, int x, int y) => CreateInventoryNode(g, x, y));
    }
    //AddItem chia ra co vi tri va khong co vi tri
    public virtual void AddItem(Grid<InventoryNode> _grid, int XCoordinate, int YCoordinate, Item storedItem)
    {
        InventoryNode _wantsToStore = _grid.GetGridObject(XCoordinate, YCoordinate);
        if(_wantsToStore.StoredItem == null)
        {
            _grid.SetGridObject(XCoordinate, YCoordinate, new InventoryNode(new Vector2Int(XCoordinate, YCoordinate), storedItem, _grid));
            _grid.TriggerGridObjectChanged(XCoordinate, YCoordinate);
        }
        else
        {
            Debug.Log("Another Item is already in place");
        }
    }
    public virtual void AddItem(Grid<InventoryNode> _grid, Item storedItem)
    {
        foreach(InventoryNode node in _grid.GridArray)
        {
            if (node.StoredItem == null)
            {
                _grid.SetGridObject(node.ItemCoordinates.x, node.ItemCoordinates.y, new InventoryNode(new Vector2Int(node.ItemCoordinates.x, node.ItemCoordinates.y), storedItem, _grid));
                _grid.TriggerGridObjectChanged(node.ItemCoordinates.x, node.ItemCoordinates.y);
            }
            else continue;
        }
    }
    public virtual InventoryNode CreateInventoryNode(Grid<InventoryNode> _grid, int XCoordinate, int YCoordinate)
    {
        InventoryNode newNode = new InventoryNode(new Vector2Int(XCoordinate, YCoordinate), _inventoryGrid);
        return newNode;
    }
}
