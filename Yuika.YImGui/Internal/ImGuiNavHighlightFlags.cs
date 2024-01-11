// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiNavHighlightFlags
{
    None,
    
    TypeDefault = 1 << 0,
    TypeThin = 1 << 1,
    AlwaysDraw = 1 << 2,
    NoRounding = 1 << 3
}