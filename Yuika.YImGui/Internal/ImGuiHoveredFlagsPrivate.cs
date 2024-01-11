// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiHoveredFlagsPrivate
{
    DelayMask = ImGuiHoveredFlags.DelayNone | ImGuiHoveredFlags.DelayShort | ImGuiHoveredFlags.DelayNormal |
                ImGuiHoveredFlags.NoSharedDelay,

    AllowedMaskForIsWindowHovered = ImGuiHoveredFlags.ChildWindows | ImGuiHoveredFlags.RootWindow |
                                    ImGuiHoveredFlags.AnyWindow | ImGuiHoveredFlags.NoPopupHierarchy |
#if USE_DOCKING
                                    ImGuiHoveredFlags.DockHierarchy |
#endif
                                    ImGuiHoveredFlags.AllowWhenBlockedByPopup |
                                    ImGuiHoveredFlags.AllowWhenBlockedByActiveItem | ImGuiHoveredFlags.ForTooltip |
                                    ImGuiHoveredFlags.Stationary,

    AllowedMaskForIsItemHovered = ImGuiHoveredFlags.AllowWhenBlockedByPopup |
                                  ImGuiHoveredFlags.AllowWhenBlockedByActiveItem |
                                  ImGuiHoveredFlags.AllowWhenOverlapped | ImGuiHoveredFlags.AllowWhenDisabled |
                                  ImGuiHoveredFlags.NoNavOverride | ImGuiHoveredFlags.ForTooltip |
                                  ImGuiHoveredFlags.Stationary | DelayMask,
}