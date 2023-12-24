using System.Diagnostics;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public static class ImGui
{
    public const int NamedKeyBegin = 512;
    public const int NamedKeyEnd = (int) ImGuiKey.KeyCount;
    public const int NamedKeyCount = NamedKeyEnd - NamedKeyBegin;

    public const bool DisableObsoleteKeyIO = true;
    public const int KeysDataSize = DisableObsoleteKeyIO ? NamedKeyCount : (int) ImGuiKey.KeyCount;
    public const int KeysDataOffset = DisableObsoleteKeyIO ? NamedKeyBegin : 0;

    internal const int ArcFastTableSize = 48;

    public static void Initialize()
    {
        ImGuiContext? ctx = CurrentContext as ImGuiContext;
        Debug.Assert(ctx != null);
        Debug.Assert(!ctx.Initialized);

        {
            
        }

        ctx.IO.GetClipboardTextFn = ctxObj =>
        {
            ImGuiContext ctx = (ImGuiContext) ctxObj;
            // ctx.Clip
            throw new NotImplementedException();
        };

        ctx.IO.ClipboardUserData = ctx;

        ImGuiViewportP viewport = new ImGuiViewportP();

        ctx.Initialized = true;
        throw new NotImplementedException();
    }
    
    public static IImGuiContext CreateContext(ImFontAtlas? sharedFontAtlas = null)
    {
        IImGuiContext? prevCtx = CurrentContext;
        ImGuiContext ctx = new ImGuiContext(sharedFontAtlas);
        CurrentContext = ctx;
        Initialize();

        if (prevCtx != null)
        {
            // Restore previous context if any, else keep new one.
            CurrentContext = prevCtx;
        }

        return ctx;
    }

    public static void Shutdown()
    {
        ImGuiContext? ctx = CurrentContext as ImGuiContext;
        Debug.Assert(ctx != null);

        if (ctx.IO.Fonts != null && ctx.FontAtlasOwnedByContext)
        {
            ctx.IO.Fonts.Locked = false;
        }

        ctx.IO.Fonts = null;
        ctx.DrawListSharedData.TempBuffer.Clear();

        if (!ctx.Initialized) return;

        throw new NotImplementedException();
    }

    public static void DestroyContext(IImGuiContext? target = null)
    {
        IImGuiContext? prevCtx = CurrentContext;
        if (target == null)
        {
            target = prevCtx;
        }

        CurrentContext = target;
        Shutdown();
        CurrentContext = prevCtx != target ? prevCtx : null;
    }
    
    public static IImGuiContext? CurrentContext { get; set; }

    private static ImGuiContext EnsureContext()
    {
        ImGuiContext? ctx = CurrentContext as ImGuiContext;
        Debug.Assert(ctx != null, "No current context. Did you call ImGui.CreateContext() and ImGui.CurrentContext = ... ?");
        return ctx;
    }

    // Main
    public static ImGuiIO IO => EnsureContext().IO;

    public static ImGuiStyle Style => EnsureContext().Style;

    public static void NewFrame()
    {
        ImGuiContext ctx = EnsureContext();

        ctx.Time += ctx.IO.DeltaTime;
        
        throw new NotImplementedException();
    }

    public static void EndFrame()
    {
        ImGuiContext ctx = EnsureContext();
        Debug.Assert(ctx.Initialized);

        // Don't process EndFrame() multiple times.
        if (ctx.FrameCountEnded == ctx.FrameCount) return;
        
        // Debug.Assert(ctx.Within);
        
        // Unlock font atlas.
        ctx.IO.Fonts.Locked = false;
        
        // Clear input data for next frame.
        ctx.IO.MousePosPrev = ctx.IO.MousePos;
        ctx.IO.AppFocusLost = false;
        ctx.IO.MouseWheel = ctx.IO.MouseWheelH = 0;
        ctx.IO.InputQueueCharacters.Clear();
        
        throw new NotImplementedException();
    }

    public static void Render()
    {
        ImGuiContext ctx = EnsureContext();
        Debug.Assert(ctx.Initialized);

        if (ctx.FrameCountEnded != ctx.FrameCount) EndFrame();
        if (ctx.FrameCountRendered == ctx.FrameCount) return;
        ctx.FrameCountRendered = ctx.FrameCount;

        ctx.IO.MetricsRenderWindows = 0;
        
        
        throw new NotImplementedException();
    }
    
    public static ImDrawData DrawData { get; }

    public static void ShowDemoWindow(ref bool open);
    public static void ShowMetricsWindow(ref bool open);
    public static void ShowDebugLogWindow(ref bool open);
    public static void ShowIDStackToolWindow(ref bool open);
    public static void ShowAboutWindow(ref bool open);
    public static void ShowStyleEditor(ref ImGuiStyle style);
    public static void ShowStyleSelector(string label);
    public static void ShowFontSelector(string label);
    public static void ShowUserGuide();
    public static string Version { get; }
    
    // Windows
    public static bool Begin(string name, ref bool open, ImGuiWindowFlags flags = ImGuiWindowFlags.None);
    public static bool Begin(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None);
}