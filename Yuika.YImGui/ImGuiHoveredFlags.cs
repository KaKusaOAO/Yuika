namespace Yuika.YImGui;

[Flags]
public enum ImGuiHoveredFlags
{
    /// <summary>
    /// Return true if directly over the item/window, not obstructed by another window, 
    /// not obstructed by an active popup or modal blocking inputs under them.
    /// </summary>
    None,

    /// <summary>
    /// <see cref="ImGui.IsWindowHovered" />() only: Return true if any children of the window is hovered
    /// </summary>
    ChildWindows                 = 1 << 0,
    /// <summary>
    /// <see cref="ImGui.IsWindowHovered" />() only: Test from root window (top most parent of the current hierarchy)
    /// </summary>
    RootWindow                   = 1 << 1,
    /// <summary>
    /// <see cref="ImGui.IsWindowHovered" />() only: Return true if any window is hovered
    /// </summary>
    AnyWindow                    = 1 << 2,
    /// <summary>
    /// <see cref="ImGui.IsWindowHovered" />() only: Do not consider popup hierarchy (do not treat popup emitter as parent of popup) 
    /// (when used with <see cref="ChildWindows" /> or <see cref="RootWindow" />)
    /// </summary>
    NoPopupHierarchy             = 1 << 3,
#if USE_DOCKING
    /// <summary>
    /// <see cref="ImGui.IsWindowHovered" />() only: Consider docking hierarchy (treat dockspace host as parent of docked window) 
    /// (when used with <see cref="ChildWindows" /> or <see cref="RootWindow" />)
    /// </summary>
    DockHierarchy                = 1 << 4,
#endif
    /// <summary>
    /// Return <c>true</c> even if a popup window is normally blocking access to this item/window.
    /// </summary>
    AllowWhenBlockedByPopup      = 1 << 5,
#if false // FIXME: Unavailable yet.
    /// <summary>
    /// Return <c>true</c> even if a modal popup window is normally blocking access to this item/window.
    /// </summary>
    AllowWhenBlockedByModal      = 1 << 6,
#endif
    AllowWhenBlockedByActiveItem = 1 << 7,
    AllowWhenOverlappedByItem    = 1 << 8,
    AllowWhenOverlappedByWindow  = 1 << 9,
    AllowWhenDisabled            = 1 << 10,
    NoNavOverride                = 1 << 11,

    AllowWhenOverlapped = AllowWhenOverlappedByItem | AllowWhenOverlappedByWindow,
    RectOnly = AllowWhenBlockedByPopup | AllowWhenBlockedByActiveItem | AllowWhenOverlapped,
    RootAndChildWindows = RootWindow | ChildWindows,

    ForTooltip                   = 1 << 12,

    // (Advanced) Mouse hovering delays.
    // - generally you can use `ImGuiHoveredFlags.ForTooltip` to use application-standardized flags.
    // - use those if you need specific overrides.
    Stationary                   = 1 << 13,
    DelayNone                    = 1 << 14,
    DelayShort                   = 1 << 15,
    DelayNormal                  = 1 << 16,
    NoSharedDelay                = 1 << 17,
}