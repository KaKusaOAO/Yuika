// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiOldColumnFlags 
{
    None,
    
    NoBorder                = 1 << 0,
    NoResize                = 1 << 1,
    NoPreserveWidths        = 1 << 2,
    NoForceWithinWindow     = 1 << 3,
    GrowParentContentsSize  = 1 << 4
}