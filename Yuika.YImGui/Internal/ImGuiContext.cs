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
    
    // Shared stacks
    public ImGuiCol DebugFlashStyleColorIdx { get; set; }
    public List<ImFont> FontStack { get; set; } = new List<ImFont>();
    public List<uint> FocusScopeStack { get; set; } = new List<uint>();
    public List<ImGuiItemFlags> ItemFlagsStack { get; set; } = new List<ImGuiItemFlags>();
    public List<ImGuiPopupData> OpenPopupStack { get; set; } = new List<ImGuiPopupData>();
    public List<ImGuiPopupData> BeginPopupStack { get; set; } = new List<ImGuiPopupData>();
    
    public List<ImGuiViewportP> Viewports { get; set; } = new List<ImGuiViewportP>();
#if USE_DOCKING
    public ImGuiViewportP? CurrentViewport { get; set; }
    public ImGuiViewportP? MouseViewport { get; set; }
    public ImGuiViewportP? MouseLastHoveredViewport { get; set; }
    public uint PlarformLastFocusedViewportId { get; set; }
    public ImGuiPlatformMonitor FallbackMonitor { get; set; }
    public int ViewportCreatedCount { get; set; }
#endif

    // Tab bars
    public List<Either<object, int>> CurrentTabBarStack { get; set; } = new List<Either<object, int>>();

    // Debug Tools
    public sbyte DebugBeginReturnValueCullDepth { get; set; }

    public ImGuiContext(ImFontAtlas? sharedFontAtlas)
    {
        IO.Ctx = this;
        throw new NotImplementedException();
    }
}
