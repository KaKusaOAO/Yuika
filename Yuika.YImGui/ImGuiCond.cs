// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiCond
{
    None,
    
    Always          = 1 << 0,
    Once            = 1 << 1,
    FirstUseEver    = 1 << 2,
    Appearing       = 1 << 3
}