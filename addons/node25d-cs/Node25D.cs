using Godot;
using System;

/// <summary>
/// This node converts a 3D position to 2D using a 2.5D transformation matrix.
/// The transformation of its 2D form is controlled by its 3D child.
/// </summary>
[Tool]
public partial class Node25D : Node2D, IComparable<Node25D>
{
	/// <summary>
	/// The number of 2D units in one 3D unit.
	/// </summary>
	public const int SCALE = 32;

	private Node3D spatialNode;
	private Transform25D transform25D;

	public Basis25D Basis25D
	{
		get { return transform25D.basis; }
	}

	public Transform25D Transform25D
	{
		get { return transform25D; }
	}

	public override void _Ready()
	{
		Node25DReady();
	}

	public override void _Process(double delta)
	{
		Node25DProcess();
	}

	/// <summary>
	/// Call this method in _Ready, or before you run Node25DProcess.
	/// </summary>
	protected void Node25DReady()
	{
		spatialNode = GetChild<Node3D>(0);

        float height3DOffset = 0;
        Shape3D shape = spatialNode.GetNode<CollisionShape3D>("CollisionShape3D").Shape;
        if (shape is BoxShape3D)
        {
            height3DOffset = (shape as BoxShape3D)?.Size.Y/2 ?? 0; //intellisense i hate you
        }

		transform25D = new Transform25D(Basis25D.FortyFive * SCALE, height3DOffset);
		// Changing the basis here will change the default for all Node25D instances.
	}

	/// <summary>
	/// Call this method in _Process, or whenever the position of this object changes.
	/// </summary>
	protected void Node25DProcess()
	{
		// CheckViewMode();
		if (spatialNode != null)
		{
			transform25D.spatialPosition = spatialNode.Position;
		}
		GlobalPosition = transform25D.FlatPosition;
	}

	// private void CheckViewMode()
	// {
	// 	if (Input.IsActionJustPressed("TopDownMode"))
	// 	{
	// 		transform25D.basis = Basis25D.TopDown * SCALE;
	// 	}
	// 	else if (Input.IsActionJustPressed("FrontSideMode"))
	// 	{
	// 		transform25D.basis = Basis25D.FrontSide * SCALE;
	// 	}
	// 	else if (Input.IsActionJustPressed("FortyFiveMode"))
	// 	{
	// 		transform25D.basis = Basis25D.FortyFive * SCALE;
	// 	}
	// 	else if (Input.IsActionJustPressed("IsometricMode"))
	// 	{
	// 		transform25D.basis = Basis25D.Isometric * SCALE;
	// 	}
	// 	else if (Input.IsActionJustPressed("ObliqueYMode"))
	// 	{
	// 		transform25D.basis = Basis25D.ObliqueY * SCALE;
	// 	}
	// 	else if (Input.IsActionJustPressed("ObliqueZMode"))
	// 	{
	// 		transform25D.basis = Basis25D.ObliqueZ * SCALE;
	// 	}
	// }

	public int CompareTo(object obj)
	{
		if (obj is Node25D)
		{
			return CompareTo((Node25D)obj);
		}
		return 1;
	}

	public int CompareTo(Node25D other)
	{
		double thisIndex = transform25D.spatialPosition.Y + 0.001f * transform25D.spatialPosition.Z;
		double otherIndex = other.transform25D.spatialPosition.Y + 0.001f * other.transform25D.spatialPosition.Z;
		double diff = thisIndex - otherIndex;
		if (diff > 0)
		{
			return 1;
		}
		if (diff < 0)
		{
			return -1;
		}
		return 0;
	}

}
