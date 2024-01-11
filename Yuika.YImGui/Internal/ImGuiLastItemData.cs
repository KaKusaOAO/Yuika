// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;

namespace Yuika.YImGui.Internal;

internal class ImGuiLastItemData
{
    public uint Id { get; set; }
    public ImGuiItemFlags InFlags { get; set; }
    public ImGuiItemStatusFlags StatusFlags { get; set; }
    public RectangleF Rect { get; set; }
    public RectangleF NavRect { get; set; }
    public RectangleF DisplayRect { get; set; }
    public RectangleF ClipRect { get; set; }
}
