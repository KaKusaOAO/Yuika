// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiKeyMod
{
    None,
    
    // @formatter:off
    Ctrl     = 1 << 12,
    Shift    = 1 << 13,
    Alt      = 1 << 14,
    Super    = 1 << 15,
    Shortcut = 1 << 11,
    
    Mask     = 0xf800
    // @formatter:on
}