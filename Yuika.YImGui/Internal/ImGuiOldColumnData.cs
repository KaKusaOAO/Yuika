// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;

namespace Yuika.YImGui.Internal;

internal class ImGuiOldColumnData
{
    public float OffsetNorm { get; set; }
    public float OffsetNormBeforeResize { get; set; }
    public ImGuiOldColumnFlags Flags { get; set; }
    public Rectangle ClipRect { get; set; }
}