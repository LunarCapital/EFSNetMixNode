using Godot;
using System.Collections.Generic;

public partial class root : Node2D
{
    TileMap tileMap;
    GridMap gridMap;
    Viewport subViewport;
    Rid subCanvas;
    Rid subCanvasItem;
    List<Rid> tempTiles;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //this is all debug shit atm
        tileMap = GetNode<TileMap>("TileMap");
        GD.Print(tileMap.Name);

        foreach (Vector2I usedCell in tileMap.GetUsedCells(0))
        {
            GD.Print($"Cell: {usedCell}, Coords: {tileMap.MapToLocal(usedCell)}");
        }

        gridMap = GetNode<GridMap>("GridMap");

        foreach (Vector3I usedCell in gridMap.GetUsedCells())
        {
            GD.Print($"3D Cell: {usedCell}, Coords: {gridMap.MapToLocal(usedCell)}");
        }

        subViewport = GetNode<Viewport>("DEBUG3D/SubViewportContainer/SubViewport");
        subCanvas = RenderingServer.CanvasCreate();
        subCanvasItem = RenderingServer.CanvasItemCreate();
        RenderingServer.CanvasItemSetParent(subCanvasItem, subCanvas);
        RenderingServer.ViewportAttachCanvas(subViewport.GetViewportRid(), subCanvas);
        tempTiles = new List<Rid>();
        //end debug

        tileMap.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
 
	}

    public override void _Draw()
    {
        // base._Draw();
        Texture2D testTexture = (tileMap.TileSet.GetSource(0) as TileSetAtlasSource)?.Texture;

        if (GetCanvasItem().IsValid && testTexture.GetRid().IsValid)
        {
            testTexture.Draw(GetCanvasItem(), new Vector2(256, 64) + new Vector2(- testTexture.GetWidth() / 2, - testTexture.GetHeight() / 4 * 3));
        } else {
            GD.Print("didn't draw because invalid");
        }

              //welcome to the WAR CRIME SECTION
        foreach (Rid rid in tempTiles)
        {
            RenderingServer.FreeRid(rid);
        }

        TileSet tileSet = tileMap.TileSet;
// bool first = true;
        int layers = tileMap.GetLayersCount();
        for (int i = 0; i < layers; i++)
        {
            var usedCellsOnThisLayer = tileMap.GetUsedCells(i);
            foreach (Vector2I usedCell in usedCellsOnThisLayer)
            {
                Vector2 pos2d = tileMap.MapToLocal(usedCell);
                TileSetSource tsSource = tileSet.GetSource(tileMap.GetCellSourceId(i, usedCell));

                if (tsSource is TileSetAtlasSource)
                {
                    Texture2D texture = (tsSource as TileSetAtlasSource)?.Texture;

                    // if (first)
                    // {
                    //     first = false;
                    //     Image img = texture.GetImage();
                    //     for (int w = 0; w < img.GetWidth(); w++)
                    //     {
                    //         for (int h = 0; h < img.GetHeight(); h++)
                    //         {
                    //             GD.Print($"at {w},{h} pixel is {img.GetPixel(w, h)}");
                    //         }
                    //     }
                    // }

                    Rid tileRID = RenderingServer.CanvasItemCreate();
                    tempTiles.Add(tileRID);
                    RenderingServer.CanvasItemSetParent(tileRID, GetCanvasItem());

                    // RenderingServer.CanvasItemAddTextureRectRegion(
                    //     tileRID,
                    //     new Rect2(Vector2.Zero, texture.GetSize()),
                    //     texture.GetRid(),
                    //     new Rect2(Vector2.Zero, texture.GetSize())
                    // );

                    RenderingServer.CanvasItemAddTextureRect(
                        tileRID,
                        new Rect2(Vector2.Zero, texture.GetSize()),
                        texture.GetRid()
                    );

                    // texture.Draw(GetCanvasItem(), new Vector2(-360, 0));

                    RenderingServer.CanvasItemSetTransform(tileRID, new Transform2D(0, tileMap.MapToLocal(usedCell)  + new Vector2(- texture.GetWidth() / 2, - texture.GetHeight() / 4 * 3)));

                    // GD.Print($"i put a thingy on subcanvas without a transform");
                }
            }
        }
        //end WAR CRIME SECTION
    }
}
