// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;

namespace Yuika.YImGui.Internal;

internal class ImGuiNavTreeNodeData
{
    public uint Id { get; set; }
    public ImGuiItemFlags InFlags { get; set; }
    public Rectangle NavRect { get; set; }
}