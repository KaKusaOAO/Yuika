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
    public ImGuiWindow CurrentWindow { get; set; }
    public ImGuiWindow? HoveredWindow { get; set; }
    public ImGuiWindow? HoveredWindowUnderMovingWindow { get; set; }
    public ImGuiWindow? MovingWindow { get; set; }
    public ImGuiWindow? WheelingWindow { get; set; }
    public Vector2 WheelingWindowRefMousePos { get; set; }
    public int WheelingWindowStartFrame { get; set; }
    public int WheelingWindowScrolledFrame { get; set; }
    public float WheelingWindowReleaseTimer { get; set; }
    public Vector2 WheelingWindowWheelRemainder { get; set; }
    public Vector2 WHeelingAxisAvg { get; set; }

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
