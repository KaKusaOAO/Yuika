using System.Diagnostics;

namespace Yuika.Graphics;

/// <summary>
/// The specific graphics API used by the <see cref="GraphicsDevice"/>.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public class GraphicsBackend : IEquatable<GraphicsBackend>
{
    /// <summary>
    /// Direct3D 11.
    /// </summary>
    public static GraphicsBackend Direct3D11 { get; }
    /// <summary>
    /// Vulkan.
    /// </summary>
    public static GraphicsBackend Vulkan { get; }
    /// <summary>
    /// OpenGL.
    /// </summary>
    public static GraphicsBackend OpenGL { get; }
    /// <summary>
    /// Metal.
    /// </summary>
    public static GraphicsBackend Metal { get; }
    /// <summary>
    /// OpenGL ES.
    /// </summary>
    public static GraphicsBackend OpenGLES { get; }

    public string Name { get; }
    
    public GraphicsBackend(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;

    public bool Equals(GraphicsBackend? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        if (obj.GetType() != GetType()) return false;
        return Equals((GraphicsBackend)obj);
    }

    public static bool operator ==(GraphicsBackend a, GraphicsBackend b) => a.Equals(b);
    public static bool operator !=(GraphicsBackend a, GraphicsBackend b) => !a.Equals(b);

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}