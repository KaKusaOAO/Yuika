using System.Numerics;

namespace Yuika.YImGui;

public class ImDrawCmd
{
    public Vector4 ClipRect { get; set; }
    public ImTextureID TextureId { get; set; }
    public uint VtxOffset { get; set; }
    public uint IdxOffset { get; set; }
    public uint ElementCount { get; set; }
    public ImDrawCallback? UserCallback { get; set; }
    public object? UserCallbackData { get; set; }
}