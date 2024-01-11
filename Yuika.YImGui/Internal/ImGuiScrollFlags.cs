// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiScrollFlags
{
    None,
    
    KeepVisibleEdgeX = 1 << 0,
    KeepVisibleEdgeY = 1 << 1,
    KeepVisibleCenterX = 1 << 2,
    KeepVisibleCenterY = 1 << 3,
    AlwaysCenterX = 1 << 4,
    AlwaysCenterY = 1 << 5,
    NoScrollParent = 1 << 6,
    
    MaskX = KeepVisibleEdgeX | KeepVisibleCenterX | AlwaysCenterX,
    MaskY = KeepVisibleEdgeY | KeepVisibleCenterY | AlwaysCenterY
}