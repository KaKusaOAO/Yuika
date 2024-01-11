// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiViewportP : ImGuiViewport
{
    public ImGuiWindow Window { get; set; }
    public int Idx { get; set; }
    
}