// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
public enum ImGuiNextItemDataFlags
{
    None,
    
    HasWidth = 1 << 0,
    HasOpen  = 1 << 1
}