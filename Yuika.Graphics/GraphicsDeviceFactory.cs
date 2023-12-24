namespace Yuika.Graphics;

public class GraphicsDeviceFactory
{
    public static GraphicsDeviceFactory Shared { get; } = new();
    
    private GraphicsDeviceFactory() {}
}