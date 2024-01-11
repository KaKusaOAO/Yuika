// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiNavMoveFlags
{
    None,
    
    LoopX = 1 << 0,
    LoopY = 1 << 1,
    WrapX = 1 << 2,
    WrapY = 1 << 3,
    WrapMask = LoopX | LoopY | WrapX | WrapY,
    
    AllowCurrentNavId = 1 << 4,
    AlsoScoreVisibleSet = 1 << 5,
    ScrollToEdgeY = 1 << 6,
    Forwarded = 1 << 7,
    DebugNoResult = 1 << 8,
    FocusApi = 1 << 9,
    IsTabbing = 1 << 10,
    IsPageMove = 1 << 11,
    Activate = 1 << 12,
    NoSelect = 1 << 13,
    NoSetNavHighlight = 1 << 14
}