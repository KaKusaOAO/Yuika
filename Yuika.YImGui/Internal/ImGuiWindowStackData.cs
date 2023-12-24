namespace Yuika.YImGui.Internal;

internal class ImGuiWindowStackData
{
    public ImGuiWindow Window { get; set; }
    public ImGuiLastItemData ParentLastItemDataBackup { get; set; }
    public ImGuiStackSizes StackSizesOnBegin { get; set; }
}
