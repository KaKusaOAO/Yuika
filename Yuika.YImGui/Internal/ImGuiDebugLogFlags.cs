// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal enum ImGuiDebugLogFlags
{
    None,
    
    // @formatter:off
    
    EventActiveId           = 1 << 0,
    EventFocus              = 1 << 1,
    EventPopup              = 1 << 2,
    EventNav                = 1 << 3,
    EventClipper            = 1 << 4,
    EventSelection          = 1 << 5,
    EventIO                 = 1 << 6,
    
#if USE_DOCKING
    EventDocking            = 1 << 7,
    EventViewport           = 1 << 8,
    EventMask               = EventActiveId | EventFocus | EventPopup | EventNav | EventClipper | EventSelection |
                              EventIO | EventDocking | EventViewport,
#else
    EventMask               = EventActiveId | EventFocus | EventPopup | EventNav | EventClipper | EventSelection |
                              EventIO,
#endif
    
    OutputToTTY             = 1 << 0,
    OutputToTestEngine      = 1 << 0,
    // @formatter:on
}