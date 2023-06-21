using Godot;
using System;

public partial class Player : Node
{
	/// <summary>
	/// The number of 2D units in one 3D unit.
	/// </summary>
	public const int SCALE = 32;
	//NOTE: not so cut and dry now because i'm isometric-ing it's now unfortunately 2 / sqrt(18) * [48 or 96 depending on z-axis or x-axis]

	private Node3D node3d;
	private Node2D node2d;
	private float height3DOffset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		node3d = GetChild<Node3D>(0); //TODO: fix this fucking hardcoded index shit fuck me

		//this really should be abstracted out elsewhere
		Shape3D shape = node3d.GetNode<CollisionShape3D>("CollisionShape3D").Shape;
		if (shape is BoxShape3D)
		{
			height3DOffset = (shape as BoxShape3D)?.Size.Y / 2 ?? 0; //intellisense i hate you
		}
		//end shit block

		// transform25D = new Transform25D(Basis25D.FortyFive * SCALE, height3DOffset);
		node2d = GetChild<Node2D>(1); //TODO: fix hardcode index fug
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (node3d != null)
		{
			int x2d = (int)(node3d.Position.X * 2 / Math.Sqrt(18) * 96);
			int y2d = (int)((node3d.Position.Z * 2 / Math.Sqrt(18) * 48) - ((node3d.Position.Y - height3DOffset) * 32));
			node2d.Position = new(x2d, y2d);

            node2d.ZIndex = (int)Math.Floor(node3d.Position.Y);
		}
		// GlobalPosition = transform25D.FlatPosition; // may need to change from Node to Node2D
	}
}

/**

some math:
x:0, y:0, z:0 => x:0, y:0

x: -sqrt(18)/2, y: 0, z: 0 => x: -96, y: 0
x:  sqrt(18)/2, y: 0, z: 0 => x:  96, y: 0

x: 0, y: 0, z: -sqrt(18)/2 =>  x: 0, y: -48



X2D = X3D * 2 / sqrt(18) * 96
Y2D = (Z3D * 2 / sqrt(18) * 48) + (Y3D * 32)

(do y later)


*/
