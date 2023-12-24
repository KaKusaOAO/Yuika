namespace Yuika.YImGui.Internal;

internal class ImGuiKeyRoutingTable
{
    public short[] Index { get; set; } = new short[ImGui.NamedKeyCount];
    public List<ImGuiKeyRoutingData> Entries { get; set; } = new List<ImGuiKeyRoutingData>();
    public List<ImGuiKeyRoutingData> EntriesNext { get; set; } = new List<ImGuiKeyRoutingData>();

    public ImGuiKeyRoutingTable()
    {
        Clear();
    }
    
    public void Clear()
    {
        Array.Fill(Index, (short) -1);
        Entries.Clear();
        EntriesNext.Clear();
    }
}