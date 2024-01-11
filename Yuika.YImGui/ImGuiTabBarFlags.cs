// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiTabBarFlags 
{
    None,

    Reorderable                  = 1 << 0,
    AutoSelectNewTabs            = 1 << 1,
    TabListPopupButton           = 1 << 2,
    NoCloseWithMiddleMouseButton = 1 << 3,
    NoTabListScrollingButtons    = 1 << 4,
    NoTooltip                    = 1 << 5,

    FittingPolicyResizeDown      = 1 << 6,
    FittingPolicyScroll          = 1 << 7,
    
    FittingPolicyMask = FittingPolicyResizeDown | FittingPolicyScroll,
    FittingPolicyDefault = FittingPolicyResizeDown,
}
