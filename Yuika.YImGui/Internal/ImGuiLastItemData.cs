using System.Drawing;

namespace Yuika.YImGui.Internal;

internal class ImGuiLastItemData
{
    public uint Id { get; set; }
    public ImGuiItemFlags InFlags { get; set; }
    public ImGuiItemStatusFlags StatusFlags { get; set; }
    public Rectangle Rect { get; set; }
    public Rectangle NavRect { get; set; }
    public Rectangle DisplayRect { get; set; }
    public Rectangle ClipRect { get; set; }
}
