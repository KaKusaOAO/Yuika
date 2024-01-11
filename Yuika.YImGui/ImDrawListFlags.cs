// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImDrawListFlags
{
    None,
    
    AntiAliasedLines       = 1 << 0,
    AntiAliasedLinesUseTex = 1 << 1,
    AntiAliasedFill        = 1 << 2,
    AllowVtxOffset         = 1 << 3
}