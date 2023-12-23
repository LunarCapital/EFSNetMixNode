using Godot;
using System.Collections.Generic;
using System;

public partial class root : Node2D
{
    TileMap tileMap;
    GridMap gridMap;
    Viewport subViewport;
    Rid subCanvas;
    Rid subCanvasItem;
    SortedDictionary<Tuple<int, int, int>, (Rid tempRid, Texture2D texture)> tempTiles;

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

        // subViewport = GetNode<Viewport>("DEBUG3D/SubViewportContainer/SubViewport");
        // subCanvas = RenderingServer.CanvasCreate();
        // subCanvasItem = RenderingServer.CanvasItemCreate();
        // RenderingServer.CanvasItemSetParent(subCanvasItem, subCanvas);
        // RenderingServer.ViewportAttachCanvas(subViewport.GetViewportRid(), subCanvas);
        // tempTiles = new SortedDictionary<Tuple<int, int, int>, (Rid tempRid, Texture2D texture)>(new DrawOrder2D());
        // //end debug

        // GD.Print("PRE PREP TEMP");
        // PrepTempTiles();
        // GD.Print("POST PREP TEMP, PRE HIDE TILES");
        // tileMap.Hide();
        // GD.Print("POST HIDE TILES");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Draw()
    {
        // base._Draw();
        // Texture2D testTexture = (tileMap.TileSet.GetSource(0) as TileSetAtlasSource)?.Texture;

        // if (GetCanvasItem().IsValid && testTexture.GetRid().IsValid)
        // {
        //     testTexture.Draw(GetCanvasItem(), new Vector2(256, 64) + new Vector2(-testTexture.GetWidth() / 2, -testTexture.GetHeight() / 4 * 3));
        // }
        // else
        // {
        //     GD.Print("didn't draw because invalid");
        // }

        // foreach (Rid rid in tempTiles)
        // {
        //     RenderingServer.FreeRid(rid);
        // }
    }

    //temp func where i fill and sort temp tiles.
    //woe betide to main draw function which has to merge two sorted lists of tiles and entities fuck that shit at least it's linear time but oh man
    private void PrepTempTiles()
    {
        TileSet tileSet = tileMap.TileSet;

        int layers = tileMap.GetLayersCount();
        for (int i = 0; i < layers; i++)
        {
            GD.Print($"on layer {i}");
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

                    (int x, int y, int height) key = (
                            (int)tileMap.MapToLocal(usedCell).X - (texture.GetWidth() / 2),
                            (int)tileMap.MapToLocal(usedCell).Y - (texture.GetHeight() / 4 * 3) + (texture.GetHeight() / 2 * tileMap.GetLayerZIndex(i)),
                            tileMap.GetLayerZIndex(i)
                        );

                    Tuple<int, int, int> tupleKey = Tuple.Create(
                        (int)tileMap.MapToLocal(usedCell).X - (texture.GetWidth() / 2),
                        (int)tileMap.MapToLocal(usedCell).Y - (texture.GetHeight() / 4 * 3) + (texture.GetHeight() / 2 * tileMap.GetLayerZIndex(i)),
                        tileMap.GetLayerZIndex(i)
                    );

                    GD.Print($"attempting to add cell with key ({key.x}, {key.y}, {key.height}). hash is {key.GetHashCode()}");
                    GD.Print($"going to add {tileRID.GetHashCode()}");

                    GD.Print($"temptiles size is {tempTiles.Count}");

                    if (tempTiles.ContainsKey(tupleKey))
                    {
                        GD.Print($"key present alrdy? {key}");
                        GD.Print($"val? {tempTiles[tupleKey].GetHashCode()}");
                    }

                    //current hypothesis: value tuple being mutable means key is being fucking changed somehow which is HILARIOUS BUT WHAT THE FUCK

                    tempTiles.Add(
                        tupleKey,
                        (tileRID, texture)
                    ); //storing ground pos NOT base pos (etc i'm accommodating for height)

                    // RenderingServer.CanvasItemAddTextureRectRegion(
                    //     tileRID,
                    //     new Rect2(Vector2.Zero, texture.GetSize()),
                    //     texture.GetRid(),
                    //     new Rect2(Vector2.Zero, texture.GetSize())
                    // );

                    // texture.Draw(GetCanvasItem(), new Vector2(-360, 0));

                    // RenderingServer.CanvasItemSetTransform(tileRID, new Transform2D(0, tileMap.MapToLocal(usedCell)  + new Vector2(- texture.GetWidth() / 2, - texture.GetHeight() / 4 * 3)));

                    // GD.Print($"i put a thingy on subcanvas without a transform");
                }
            }
        }

        foreach (var pair in tempTiles)
        {
                GD.Print($"printing layer {pair.Key.Item3} tiles IN DRAW ORDER: x: {pair.Key.Item1}, y: {pair.Key.Item2}");

                RenderingServer.CanvasItemSetParent(pair.Value.tempRid, GetCanvasItem());

                RenderingServer.CanvasItemAddTextureRect(
                    pair.Value.tempRid,
                    new Rect2(Vector2.Zero, pair.Value.texture.GetSize()),
                    pair.Value.texture.GetRid()
                );
                RenderingServer.CanvasItemSetTransform(pair.Value.tempRid, new Transform2D(0, new Vector2(pair.Key.Item1, pair.Key.Item2 - (32 * pair.Key.Item3)))); //don't like hardcoded 32, probs need to fix texture as const somehow
        }
    }

    public class DrawOrder2D : Comparer<Tuple<int, int, int>>
    {
        public override int Compare(Tuple<int, int, int> a, Tuple<int, int, int> b)
        {
            //ground pos first, THEN height
            //larger y = closer to viewer
            if (a.Item2 != b.Item2)
            {
                return a.Item2 - b.Item2; //pos number
            }
            else
            {
                if (a.Item3 != b.Item3)
                {
                    return a.Item3 - b.Item3;
                }
                else
                {
                    return a.Item1 - b.Item1;
                }
            }
        }
    }
}
