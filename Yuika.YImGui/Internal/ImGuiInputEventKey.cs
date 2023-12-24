namespace Yuika.YImGui.Internal;

internal class ImGuiInputEventKey : ImGuiInputEvent
{
    public ImGuiKey Key { get; set; }
    public bool Down { get; set; }
    public float AnalogValue { get; set; }
}