// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiContext : IImGuiContext
{
    public bool Initialized { get; set; }
    public bool FontAtlasOwnedByContext { get; set; }
    public ImGuiIO IO { get; } = new ImGuiIO();
#if USE_DOCKING
    public ImGuiPlatformIO PlatformIO { get; } = new ImGuiPlatformIO();
#endif
    public ImGuiStyle Style { get; set; }
    public ImFont Font { get; set; }
    public float FontSize { get; set; }
    public float FontBaseSize { get; set; }
    public ImDrawListSharedData DrawListSharedData { get; set; }
    public double Time { get; set; }
    public int FrameCount { get; set; }
    public int FrameCountEnded { get; set; }
    public int FrameCountPlatformEnded { get; set; }
    public int FrameCountRendered { get; set; }
    public bool WithinFrameScope { get; set; }
    public bool WithinFrameScopeWithImplicitWindow { get; set; }
    public bool WithinEndChild { get; set; }

    // Inputs
    public List<ImGuiInputEvent> InputEventsQueue { get; set; } = new List<ImGuiInputEvent>();
    public List<ImGuiInputEvent> InputEventsTrail { get; set; } = new List<ImGuiInputEvent>();
    public ImGuiMouseSource InputEventsNextMouseSource { get; set; }
    public uint InputEventsNextEventId { get; set; }

    // Windows state
    public List<ImGuiWindow> Windows { get; set; } = new List<ImGuiWindow>();
    public List<ImGuiWindow> WindowsFocusOrder { get; set; } = new List<ImGuiWindow>();
    public List<ImGuiWindow> WindowsTempSortOrder { get; set; } = new List<ImGuiWindow>();
    public List<ImGuiWindowStackData> CurrentWindowStack { get; set; } = new List<ImGuiWindowStackData>();
    public ImGuiStorage WindowsById { get; set; } = new ImGuiStorage();
    public int WindowsActiveCount { get; set; }
    public Vector2 WindowsHoverPadding { get; set; }
    public ImGuiWindow? CurrentWindow { get; set; }
    public ImGuiWindow? HoveredWindow { get; set; }
    public ImGuiWindow? HoveredWindowUnderMovingWindow { get; set; }
    public ImGuiWindow? MovingWindow { get; set; }
    public ImGuiWindow? WheelingWindow { get; set; }
    public Vector2 WheelingWindowRefMousePos { get; set; }
    public int WheelingWindowStartFrame { get; set; }
    public int WheelingWindowScrolledFrame { get; set; }
    public float WheelingWindowReleaseTimer { get; set; }
    public Vector2 WheelingWindowWheelRemainder { get; set; }
    public Vector2 WheelingAxisAvg { get; set; }

    // Item / widgets state and tracking information
    public uint DebugHookIdInfo { get; set; }
    public uint HoveredId { get; set; }
    public uint HoveredIdPreviousFrame { get; set; }
    public bool HoveredIdAllowOverlap { get; set; }
    public bool HoveredIdDisabled { get; set; }
    public float HoveredIdTimer { get; set; }
    public float HoveredIdNotActiveTimer { get; set; }
    public uint ActiveId { get; set; }
    public uint ActiveIdIsAlive { get; set; }
    public float ActiveIdTimer { get; set; }
    public bool ActiveIdIsJustActivated { get; set; }
    public bool ActiveIdAllowOverlap { get; set; }
    public bool ActiveIdNoClearOnFocusLoss { get; set; }
    public bool ActiveIdHasBeenPressedBefore { get; set; }
    public bool ActiveIdHasBeenEditedBefore { get; set; }
    public bool ActiveIdHasBeenEditedThisFrame { get; set; }
    public Vector2 ActiveIdClickOffset { get; set; }
    public ImGuiWindow ActiveIdWindow { get; set; }
    public ImGuiInputSource ActiveIdSource { get; set; }
    public int ActiveIdMouseButton { get; set; }
    public uint ActiveIdPreviousFrame { get; set; }
    public bool ActiveIdPreviousFrameIsAlive { get; set; }
    public bool ActiveIdPreviousFrameHasBeenEditedBefore { get; set; }
    public ImGuiWindow ActiveIdPreviousFrameWindow { get; set; }
    public uint LastActiveId { get; set; }
    public float LastActiveIdTimer { get; set; }

    public double LastKeyModsChangeTime { get; set; }
    public double LastKeyModsChangeFromNoneTime { get; set; }
    public double LastKeyboardPressTime { get; set; }
    public ImGuiKeyOwnerData[] KeysOwnerData { get; set; } = new ImGuiKeyOwnerData[ImGui.NamedKeyCount];
    public ImGuiKeyRoutingTable KeysRoutingTable { get; set; }
    public uint ActiveIdUsingNavDirMask { get; set; }
    public bool ActiveIdUsingAllKeyboardKeys { get; set; }
    public ImGuiKeyChord DebugBreakInShortcutRouting { get; set; }

    // Next window/item data
    public uint CurrentFocusScopeId { get; set; }
    public ImGuiItemFlags CurrentItemFlags { get; set; }
    public uint DebugLocateId { get; set; }
    public ImGuiNextItemData NextItemData { get; set; }
    public ImGuiLastItemData LastItemData { get; set; }
    public ImGuiNextWindowData NextWindowData { get; set; }
    public bool DebugShowGroupRects { get; set; }

    // Shared stacks
    public ImGuiCol DebugFlashStyleColorIdx { get; set; }
    public List<ImFont> FontStack { get; set; } = new List<ImFont>();
    public List<uint> FocusScopeStack { get; set; } = new List<uint>();
    public List<ImGuiItemFlags> ItemFlagsStack { get; set; } = new List<ImGuiItemFlags>();
    public List<ImGuiPopupData> OpenPopupStack { get; set; } = new List<ImGuiPopupData>();
    public List<ImGuiPopupData> BeginPopupStack { get; set; } = new List<ImGuiPopupData>();
    public List<ImGuiNavTreeNodeData> NavTreeNodeStack { get; set; } = new List<ImGuiNavTreeNodeData>();

    public int BeginMenuCount { get; set; }

    // Viewports
    public List<ImGuiViewportP> Viewports { get; set; } = new List<ImGuiViewportP>();

#if USE_DOCKING
    public float CurrentDpiScale { get; set; }
    public ImGuiViewportP? CurrentViewport { get; set; }
    public ImGuiViewportP? MouseViewport { get; set; }
    public ImGuiViewportP? MouseLastHoveredViewport { get; set; }
    public uint PlatformLastFocusedViewportId { get; set; }
    public ImGuiPlatformMonitor FallbackMonitor { get; set; }
    public int ViewportCreatedCount { get; set; }
    public int PlatformWindowsCreatedCount { get; set; }
    public int ViewportFocusedStampCount { get; set; }
#endif

    // Gamepad / keyboard Navigation
    public ImGuiWindow? NavWindow { get; set; }
    public uint NavId { get; set; }
    public uint NavFocusScopeId { get; set; }
    public uint NavActivateId { get; set; }
    public uint NavActivateDownId { get; set; }
    public uint NavActivatePressedId { get; set; }
    public ImGuiActivateFlags NavActivateFlags { get; set; }
    public uint NavJustMovedToId { get; set; }
    public uint NavJustMovedToFocusScopeId { get; set; }
    public ImGuiKeyChord NavJustMovedToKeyMods { get; set; }
    public uint NavNextActivateId { get; set; }
    public ImGuiActivateFlags NavNextActivateFlags { get; set; }
    public ImGuiInputSource NavInputSource { get; set; }
    public ImGuiNavLayer NavLayer { get; set; }
    public long NavLastValidSelectionUserData { get; set; }
    public bool NavIdIsAlive { get; set; }
    public bool NavMousePosDirty { get; set; }
    public bool NavDisableHighlight { get; set; }
    public bool NavDisableMouseHover { get; set; }

    // Navigation: Init & Move Requests
    public bool NavAnyRequest { get; set; }
    public bool NavInitRequest { get; set; }
    public bool NavInitRequestFromMove { get; set; }
    public ImGuiNavItemData NavInitResult { get; set; }
    public bool NavMoveSubmitted { get; set; }
    public bool NavMoveScoringItems { get; set; }
    public bool NavMoveForwardToNextFrame { get; set; }
    public ImGuiNavMoveFlags NavMoveFlags { get; set; }
    public ImGuiScrollFlags NavMoveScrollFlags { get; set; }
    public ImGuiKeyChord NavMoveKeyMods { get; set; }
    public ImGuiDir NavMoveDir { get; set; }
    public ImGuiDir NavMoveDirForDebug { get; set; }
    public ImGuiDir NavMoveClipDir { get; set; }
    public RectangleF NavScoringRect { get; set; }
    public RectangleF NavScoringNoClipRect { get; set; }
    public int NavScoringDebugCount { get; set; }
    public int NavTabbingDir { get; set; }
    public int NavTabbingCounter { get; set; }
    public ImGuiNavItemData NavMoveResultLocal { get; set; }
    public ImGuiNavItemData NavMoveResultLocalVisible { get; set; }
    public ImGuiNavItemData NavMoveResultOther { get; set; }
    public ImGuiNavItemData NavTabbingResultFirst { get; set; }

    // Navigation: Windowing (Ctrl-Tab for list, or Menu button + keys or directional pads to move/resize)
    public ImGuiKeyChord ConfigNavWindowingKeyNext { get; set; }
    public ImGuiKeyChord ConfigNavWindowingKeyPrev { get; set; }
    public ImGuiWindow NavWindowingTarget { get; set; }
    public ImGuiWindow NavWindowingTargetAnim { get; set; }
    public ImGuiWindow NavWindowingListWindow { get; set; }
    public float NavWindowingTimer { get; set; }
    public float NavWindowingHighlightAlpha { get; set; }
    public bool NavWindowingToggleLayer { get; set; }
    public Vector2 NavWindowingAccumDeltaPos { get; set; }
    public SizeF NavWindowingAccumDeltaSize { get; set; }

    // Render
    public float DimBgRatio { get; set; }

    // Drag and Drop
    public bool DragDropActive { get; set; }
    public bool DragDropWithinSource { get; set; }
    public bool DragDropWithinTarget { get; set; }
    public ImGuiDragDropFlags DragDropSourceFlags { get; set; }
    public int DragDropSourceFrameCount { get; set; }
    public int DragDropMouseButton { get; set; }
    public ImGuiPayload DragDropPayload { get; set; }
    public RectangleF DragDropTargetRect { get; set; }
    public RectangleF DragDropTargetClipRect { get; set; }
    public uint DragDropTargetId { get; set; }
    public ImGuiDragDropFlags DragDropAcceptFlags { get; set; }
    public float DragDropAcceptIdCurrRectSurface { get; set; }
    public uint DragDropAcceptIdCurr { get; set; }
    public uint DragDropAcceptIdPrev { get; set; }
    public int DragDropAcceptFrameCount { get; set; }
    public uint DragDropHoldJustPressedId { get; set; }
    public object? DragDropPayloadObj { get; set; }

    // Clipper
    public int ClipperTempDataStacked { get; set; }
    public List<ImGuiListClipperData> ClipperTempData { get; set; } = new List<ImGuiListClipperData>();

    // Tables
    public ImGuiTable? CurrentTable { get; set; }
    public uint DebugBreakInTable { get; set; }
    public int TablesTempDataStacked { get; set; }
    public List<ImGuiTableTempData> TablesTempData { get; set; } = new List<ImGuiTableTempData>();
    public ImPool<ImGuiTable> Tables { get; set; } = new ImPool<ImGuiTable>();
    public List<float> TablesLastTimeActive { get; set; } = new List<float>();
    public List<ImDrawChannel> DrawChannelsTempMergeBuffer { get; set; } = new List<ImDrawChannel>();

    // Tab bars
    public ImGuiTabBar? CurrentTabBar { get; set; }
    public ImPool<ImGuiTabBar> TabBars { get; set; } = new ImPool<ImGuiTabBar>();
    public List<Either<object, int>> CurrentTabBarStack { get; set; } = new List<Either<object, int>>();
    public List<ImGuiShrinkWidthItem> ShrinkWidthBuffer { get; set; } = new List<ImGuiShrinkWidthItem>();
    
    // Hover delay system
    public uint HoverItemDelayId { get; set; }
    public uint HoverItemDelayIdPreviousFrame { get; set; }
    public float HoverItemDelayTimer { get; set; }
    public float HoverItemDelayClearTime { get; set; }
    public uint HoverItemUnlockedStationaryId { get; set; }
    public uint HoverWindowUnlockedStationaryId { get; set; }
    
    // Mouse state
    public ImGuiMouseCursor MouseCursor { get; set; }
    public float MouseStationaryTimer { get; set; }
    public Vector2 MouseLastValidPos;
    
    // Widget state
    public ImGuiInputTextState InputTextState { get; set; }
    public ImFont InputTextPasswordFont { get; set; }

    // Debug Tools
    public ImGuiDebugLogFlags DebugLogFlags { get; set; }
    public ImGuiDebugLogFlags DebugLogAutoDisableFlags { get; set; }
    public sbyte DebugBeginReturnValueCullDepth { get; set; }
    
    // Misc
    public float[] FramerateSecPerFrame { get; set; } = new float[60];
    public int FramerateSecPerFrameIdx { get; set; }
    public int FramerateSecPerFrameCount { get; set; }
    public float FramerateSecPerFrameAccum { get; set; }

    public ImGuiContext(ImFontAtlas? sharedFontAtlas)
    {
        IO.Ctx = this;
        throw new NotImplementedException();
    }
}
