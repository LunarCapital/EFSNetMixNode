using Godot;

public class TileMapToGridMapConverter
{
    public static void Convert(TileMap tileMap, GridMap gridMap)
    {
        gridMap.Clear();

        for (int layer = 0; layer < tileMap.GetLayersCount(); layer++)
        {
            int heightLevel = tileMap.GetLayerZIndex(layer); //Y level
            foreach (Vector2I cell in tileMap.GetUsedCells(layer))
            {
                Vector3I translatedCoord = new(cell.X - heightLevel, heightLevel, cell.Y + heightLevel);
                gridMap.SetCellItem(translatedCoord, 0); //don't really like the hardcode but eh
            }

        }
    }
}