namespace Yuika.YImGui.Internal;

internal class ImGuiInputEvent
{
    public ImGuiInputEventType Type { get; set; }
    public ImGuiInputSource Source { get; set; }
    public uint EventId { get; set; }
}