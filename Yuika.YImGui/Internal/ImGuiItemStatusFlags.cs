// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiItemStatusFlags 
{
    None,

    HoveredRect      = 1 << 0,
    HasDisplayRect   = 1 << 1,
    Edited           = 1 << 2,
    ToggledSelection = 1 << 3,
    ToggledOpen      = 1 << 4,
    HasDeactivated   = 1 << 5,
    Deactivated      = 1 << 6,
    HoveredWindow    = 1 << 7,
    Visible          = 1 << 8,
    HasClipRect      = 1 << 9,

#if ENABLE_TEST_ENGINE

    Openable         = 1 << 20,
    Opened           = 1 << 21,
    Checkable        = 1 << 22,
    Checked          = 1 << 23,
    Inputable        = 1 << 24,

#endif
}
