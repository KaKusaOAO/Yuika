// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiInputTextState
{
    public ImGuiContext Context { get; set; }
    public uint Id { get; set; }
    public int CurLen { get; set; }
}