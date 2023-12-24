namespace Yuika.YImGui.Internal;

internal class ImGuiInputEventMouseButton : ImGuiInputEvent
{
    public int Button { get; set; }
    public bool Down { get; set; }
    public ImGuiMouseSource MouseSource { get; set; }
}