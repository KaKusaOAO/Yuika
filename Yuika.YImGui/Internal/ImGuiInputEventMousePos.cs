using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiInputEventMousePos : ImGuiInputEvent
{
    // public float PosX { get; set; }
    // public float PosY { get; set; }
    
    public Vector2 Position { get; set; }
    public ImGuiMouseSource MouseSource { get; set; }
}