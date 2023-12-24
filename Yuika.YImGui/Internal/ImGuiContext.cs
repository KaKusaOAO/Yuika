namespace Yuika.YImGui.Internal;

internal class ImGuiContext : IImGuiContext
{
    public bool Initialized { get; set; }
    public bool FontAtlasOwnedByContext { get; set; }
    public ImGuiIO IO { get; } = new ImGuiIO();
    public ImGuiStyle Style { get; set; }
    public ImFont Font { get; set; }
    public float FontSize { get; set; }
    public float FontBaseSize { get; set; }
    public ImDrawListSharedData DrawListSharedData { get; set; }
    public double Time { get; set; }
    public int FrameCount { get; set; }
    public int FrameCountEnded { get; set; }
    
    // Inputs
    public List<ImGuiInputEvent> InputEventsQueue { get; set; } = new List<ImGuiInputEvent>();
    public List<ImGuiInputEvent> InputEventsTrail { get; set; } = new List<ImGuiInputEvent>();
    public ImGuiMouseSource InputEventsNextMouseSource { get; set; }
    public uint InputEventsNextEventId { get; set; }

    public List<ImGuiWindow> Windows { get; set; } = new List<ImGuiWindow>();
    public List<ImGuiWindow> WindowsFocusOrder { get; set; } = new List<ImGuiWindow>();
    public List<ImGuiWindow> WindowsTempSortOrder { get; set; } = new List<ImGuiWindow>();
    
    // Tab bars
    public List<Either<object, int>> CurrentTabBarStack { get; set; } = new List<Either<object, int>>();
    
    public ImGuiContext(ImFontAtlas? sharedFontAtlas)
    {
        IO.Ctx = this;
        
    }
}