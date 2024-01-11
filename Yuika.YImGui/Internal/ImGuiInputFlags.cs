// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiInputFlags
{
    None,
    
    // @formatter:off
    Repeat                     = 1 << 0,
    RepeatRateDefault          = 1 << 1,
    RepeatRateNavMove          = 1 << 2,
    RepeatRateNavTweak         = 1 << 3,
    RepeatRateMask             = RepeatRateDefault | RepeatRateNavMove | RepeatRateNavTweak,
    
    CondHovered                = 1 << 4,
    CondActive                 = 1 << 5,
    CondDefault                = CondHovered | CondActive,
    CondMask                   = CondHovered | CondActive,
    
    LockThisFrame              = 1 << 6,
    LockUntilRelease           = 1 << 7,
    
    RouteFocused               = 1 << 8,
    RouteGlobalLow             = 1 << 9,
    RouteGlobal                = 1 << 10,
    RouteGlobalHigh            = 1 << 11,
    RouteMask                  = RouteFocused | RouteGlobalLow | RouteGlobal | RouteGlobalHigh, 
    
    RouteAlways                = 1 << 12,
    RouteUnlessBgFocused       = 1 << 13,
    RouteExtraMask             = RouteAlways | RouteUnlessBgFocused,
    
    SupportedByIsKeyPressed    = Repeat | RepeatRateMask,
    SupportedByShortcut        = Repeat | RepeatRateMask | RouteMask | RouteExtraMask,
    SupportedBySetKeyOwner     = LockThisFrame | LockUntilRelease,
    SupportedBySetItemKeyOwner = SupportedBySetKeyOwner | CondMask
    // @formatter:on
}