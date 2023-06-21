#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class GridMapPlacerPlugin : EditorPlugin
{
    Control dock;
    Popup errorPopup;

    IList<TileMap> currentTileMaps;
    IList<GridMap> currentGridMaps;

    OptionButton tileMapOptionBtn;
    OptionButton gridMapOptionBtn;

	public override void _EnterTree()
	{
    // Initialization of the plugin goes here.
        dock = (Control)GD.Load<PackedScene>("res://addons/gridmapplacer/Dock.tscn").Instantiate();
        AddControlToBottomPanel(dock, "Grid Map Placer");

        Button resetBtn = dock.GetNode<Button>("GridContainer/VBoxContainer/ResetDropdownsBtn");
        resetBtn.Pressed += () => OnResetDropdownsBtnPressed();

        Button populateGridMapBtn = dock.GetNode<Button>("GridContainer/VBoxContainer/PopulateGridMapBtn");
        populateGridMapBtn.Pressed += () => OnPopulateGridMapBtnPressed();

        errorPopup = dock.GetNode<Popup>("ErrorPopup");

        Button closePopupBtn = dock.GetNode<Button>("ErrorPopup/VBoxContainer/CloseBtn");
        closePopupBtn.Pressed += () => errorPopup.Hide();

        tileMapOptionBtn = dock.GetNode<OptionButton>("GridContainer/VBoxContainer/TileMapSelector");
        gridMapOptionBtn = dock.GetNode<OptionButton>("GridContainer/VBoxContainer/GridMapSelector");
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
        RemoveControlFromBottomPanel(dock);
        dock.Free();
	}

    private void OnResetDropdownsBtnPressed()
    {
        ClearOptionButtons();
        PopulateOptionButtons();
    }

    private void OnPopulateGridMapBtnPressed()
    {
        if (tileMapOptionBtn.Selected == -1 || gridMapOptionBtn.Selected == -1)
        {
            errorPopup.Show();
        } else
        {
            TileMap selectedTileMap = currentTileMaps[tileMapOptionBtn.Selected];
            GridMap selectedGridMap = currentGridMaps[gridMapOptionBtn.Selected];
            TileMapToGridMapConverter.Convert(selectedTileMap, selectedGridMap);
        }

    }

    private void ClearOptionButtons()
    {
        tileMapOptionBtn.Clear();
        gridMapOptionBtn.Clear();
    }

    private void PopulateOptionButtons()
    {
        (IList<TileMap> tileMaps, IList<GridMap> gridMaps) = ExtractTileAndGridMaps();

        for (int i = 0; i < tileMaps.Count; i++)
        {
            TileMap tm = tileMaps[i];
            tileMapOptionBtn.AddItem($"{tm.Name} - {i}", i);
        }

        for (int i = 0; i < gridMaps.Count; i++)
        {
            GridMap gm = gridMaps[i];
            gridMapOptionBtn.AddItem($"{gm.Name} - {i}", i);
        }

        currentTileMaps = tileMaps;
        currentGridMaps = gridMaps;
    }

    private (IList<TileMap> tileMaps, IList<GridMap> gridMaps) ExtractTileAndGridMaps()
    {
        List<TileMap> tileMaps = new();
        List<GridMap> gridMaps = new();

        var currentScene = GetTree().EditedSceneRoot;
        Queue<Node> q = new();

        q.Enqueue(currentScene);

        while (q.Count > 0)
        {
            Node node = q.Dequeue();
            if (node is TileMap) tileMaps.Add(node as TileMap);
            else if (node is GridMap) gridMaps.Add(node as GridMap);

            foreach (Node child in node.GetChildren())
            {
                q.Enqueue(child);
            }
        }

        return (tileMaps, gridMaps);
    }
}
#endif
