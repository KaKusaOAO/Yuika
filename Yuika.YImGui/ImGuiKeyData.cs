namespace Yuika.YImGui;

public struct ImGuiKeyData
{
    public bool Down { get; set; }
    public float DownDuration { get; set; }
    public float DownDurationPrev { get; set; }
    public float AnalogValue { get; set; }
}