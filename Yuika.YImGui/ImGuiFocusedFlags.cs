// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiFocusedFlags 
{
    None,

    ChildWindows     = 1 << 0,
    RootWindow       = 1 << 1,
    AnyWindow        = 1 << 2,
    NoPopupHierarchy = 1 << 3,
#if USE_DOCKING
    DockHierarchy    = 1 << 4,
#endif
    RootAndChildWindows = RootWindow | ChildWindows
}
