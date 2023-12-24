using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiInputEventMouseWheel : ImGuiInputEvent
{
    // public float WheelX { get; set; }
    // public float WheelY { get; set; }
    
    public Vector2 Wheel { get; set; }
    public ImGuiMouseSource MouseSource { get; set; }
}