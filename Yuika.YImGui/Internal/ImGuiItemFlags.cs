// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiItemFlags 
{
    None,

    NoTabStop                = 1 << 0,
    ButtonRepeat             = 1 << 1,
    Disabled                 = 1 << 2,
    NoNav                    = 1 << 3,
    NoNavDefaultFocus        = 1 << 4,
    SelectableDontClosePopup = 1 << 5,
    MixedValue               = 1 << 6,
    ReadOnly                 = 1 << 7,
    NoWindowHoverableCheck   = 1 << 8,
    AllowOverlap             = 1 << 9,

    Inputable                = 1 << 10,
    HasSelectionUserData     = 1 << 11,
}
