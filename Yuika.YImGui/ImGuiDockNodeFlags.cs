// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

#if USE_DOCKING

[Flags]
public enum ImGuiDockNodeFlags 
{
    None,

    KeepAliveOnly            = 1 << 0,
#if false
    NoCentralNode            = 1 << 1,
#endif
    NoDockingOverCentralNode = 1 << 2,
    PassthroughCentralNode   = 1 << 3,
    NoDockingSplit           = 1 << 4,
    NoResize                 = 1 << 5,
    AutoHideTabBar           = 1 << 6,
    NoUndocking              = 1 << 7,
}

#endif