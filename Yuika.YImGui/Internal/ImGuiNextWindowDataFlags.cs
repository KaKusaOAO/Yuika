// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal enum ImGuiNextWindowDataFlags
{
    None,
    
    HasPos            = 1 << 0,
    HasSize           = 1 << 1,
    HasContentSize    = 1 << 2,
    HasCollpased      = 1 << 3,
    HasSizeConstraint = 1 << 4,
    HasFocus          = 1 << 5,
    HasBgAlpha        = 1 << 6,
    HasScroll         = 1 << 7,
    HasChildFlags     = 1 << 8,
#if USE_DOCKING
    HasViewport       = 1 << 9,
    HasDock           = 1 << 10,
    HasWindowClass    = 1 << 11,
#endif
}