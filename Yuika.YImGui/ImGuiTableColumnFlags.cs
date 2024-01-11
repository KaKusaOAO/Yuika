// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiTableColumnFlags
{
    None,

    // @formatter:off
    Disabled                   = 1 << 0,
    DefaultHide                = 1 << 1,
    DefaultSort                = 1 << 2,
    WidthStretch               = 1 << 3,
    WidthFixed                 = 1 << 4,
    NoResize                   = 1 << 5,
    NoReorder                  = 1 << 6,
    NoHide                     = 1 << 7,
    NoClip                     = 1 << 8,
    NoSort                     = 1 << 9,
    NoSortAscending            = 1 << 10,
    NoSortDescending           = 1 << 11,
    NoHeaderLabel              = 1 << 12,
    NoHeaderWidth              = 1 << 13,
    PreferSortAscending        = 1 << 14,
    PreferSortDescending       = 1 << 15,
    IndentEnable               = 1 << 16,
    IndentDisable              = 1 << 17,
    AngledHeader               = 1 << 18,
    
    IsEnabled                  = 1 << 24,
    IsVisible                  = 1 << 25,
    IsSorted                   = 1 << 26,
    IsHovered                  = 1 << 27,
    
    WidthMask                  = WidthStretch | WidthFixed,
    IndentMask                 = IndentEnable | IndentDisable,
    StatusMask                 = IsEnabled | IsVisible | IsSorted | IsHovered,
    NoDirectResize             = 1 << 30,
    // @formatter:on
}