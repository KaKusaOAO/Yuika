// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiSelectableFlags 
{
    None,

    DontClosePopups  = 1 << 0,
    SpanAllColumns   = 1 << 1,
    AllowDoubleClick = 1 << 2,
    Disabled         = 1 << 3,
    AllowOverlap     = 1 << 4,
}
