// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiActivateFlags
{
    None,
    
    // @formatter:off
    PreferInput        = 1 << 0,
    PreferTweak        = 1 << 1,
    TryToPreserveState = 1 << 2,
    FromTabbing        = 1 << 3
    // @formatter:on
}