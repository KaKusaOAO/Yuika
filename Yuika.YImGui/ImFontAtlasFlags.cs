// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImFontAtlasFlags
{
    None,
    
    // @formatter:off
    NoPowerOfTwoHeight  = 1 << 0,
    NoMouseCursors      = 1 << 1,
    NoBakedLines        = 1 << 2,    
    // @formatter:on
}