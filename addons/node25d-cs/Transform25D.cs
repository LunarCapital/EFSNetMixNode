using Godot;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Calculates the 2D transformation from a 3D position and a Basis25D.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Transform25D : IEquatable<Transform25D>
{
    // Public fields store information that is used to calculate the properties.

    /// <summary>
    /// we need to offset this by HALF the 3d object's height, because 3d pos is calced from the MIDDLE
    /// </summary>
    public float height3DOffset;

    /// <summary>
    /// Controls how the 3D position is transformed into 2D.
    /// </summary>
    public Basis25D basis;

    /// <summary>
    /// The 3D position of the object. Should be updated on every frame before everything else.
    /// </summary>
    public Vector3 spatialPosition;

    // Public properties calculate on-the-fly.

    /// <summary>
    /// The 2D transformation of this object. Slower than FlatPosition.
    /// </summary>
    public readonly Transform2D FlatTransform
    {
        get
        {
            return new Transform2D(0, FlatPosition);
        }
    }

    /// <summary>
    /// The 2D position of this object.
    /// </summary>
    public readonly Vector2 FlatPosition
    {
        get
        {
            // Vector2 pos = spatialPosition.X * basis.x;
            // pos += spatialPosition.Y * basis.y;
            // pos += spatialPosition.Z * basis.z;
            // return pos;

            int x2d = (int)(spatialPosition.X * 2 / Math.Sqrt(18) * 96);
            int y2d = (int)((spatialPosition.Z * 2 / Math.Sqrt(18) * 48) - ((spatialPosition.Y - height3DOffset) * 32));
            return new Vector2(x2d, y2d);
        }
    }

    // Constructors
    public Transform25D(Transform25D transform25D, float height3DOffset = 0)
    {
        basis = transform25D.basis;
        spatialPosition = transform25D.spatialPosition;
        this.height3DOffset = height3DOffset;
    }
    public Transform25D(Basis25D basis25D, float height3DOffset = 0)
    {
        basis = basis25D;
        spatialPosition = Vector3.Zero;
        this.height3DOffset = height3DOffset;
    }
    public Transform25D(Basis25D basis25D, Vector3 position3D, float height3DOffset = 0)
    {
        basis = basis25D;
        spatialPosition = position3D;
        this.height3DOffset = height3DOffset;
    }
    public Transform25D(Vector2 xAxis, Vector2 yAxis, Vector2 zAxis, float height3DOffset = 0)
    {
        basis = new Basis25D(xAxis, yAxis, zAxis);
        spatialPosition = Vector3.Zero;
        this.height3DOffset = height3DOffset;
    }
    public Transform25D(Vector2 xAxis, Vector2 yAxis, Vector2 zAxis, Vector3 position3D, float height3DOffset = 0)
    {
        basis = new Basis25D(xAxis, yAxis, zAxis);
        spatialPosition = position3D;
        this.height3DOffset = height3DOffset;
    }

    public static bool operator ==(Transform25D left, Transform25D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Transform25D left, Transform25D right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object obj)
    {
        if (obj is Transform25D)
        {
            return Equals((Transform25D)obj);
        }
        return false;
    }

    public bool Equals(Transform25D other)
    {
        return basis.Equals(other.basis) && spatialPosition.Equals(other.spatialPosition);
    }

    public bool IsEqualApprox(Transform25D other)
    {
        return basis.IsEqualApprox(other.basis) && spatialPosition.IsEqualApprox(other.spatialPosition);
    }

    public override int GetHashCode()
    {
        return basis.GetHashCode() ^ spatialPosition.GetHashCode();
    }

    public override string ToString()
    {
        string s = String.Format("({0}, {1})", new object[]
        {
            basis.ToString(),
            spatialPosition.ToString()
        });
        return s;
    }

    public string ToString(string format)
    {
        string s = String.Format("({0}, {1})", new object[]
        {
            basis.ToString(format),
            spatialPosition.ToString(format)
        });
        return s;
    }
}
