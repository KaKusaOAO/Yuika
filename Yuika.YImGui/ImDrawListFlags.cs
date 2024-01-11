// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImDrawListFlags
{
    None,
    
    AntiAliasedLines       = 1 << 0,
    AntiAliasesLinesUseTex = 1 << 1,
    AntiAliasesFill        = 1 << 2,
    AllowVtxOffset         = 1 << 3
}