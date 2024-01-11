// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiFocusRequestFlags
{
    None,
    
    // @formatter:off
    RestoreFocusedChild = 1 << 0,
    UnlessBelowModal    = 1 << 1,
    // @formatter:on
}