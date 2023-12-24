namespace Yuika.YImGui.Internal;

internal class ImGuiKeyRoutingData
{
    public short NextEntryIndex { get; set; } = -1;
    public ushort Mods { get; set; }
    public byte RoutingNextScore { get; set; } = byte.MaxValue;
    public uint RoutingCurr { get; set; } = unchecked((uint)-1);
    public uint RoutingNext { get; set; } = unchecked((uint)-1);
}