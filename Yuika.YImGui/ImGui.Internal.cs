using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public static partial class ImGui
{
    #region -- Window
    
    private static ImGuiWindow CurrentWindowRead 
    {
        get 
        {
            ImGuiContext ctx = EnsureContext();
            return ctx.CurrentWindow!;
        }
    }

    private static ImGuiWindow CurrentWindow 
    {
        get 
        {
            ImGuiContext ctx = EnsureContext();
            ctx.CurrentWindow!.WriteAccessed = true;
            return ctx.CurrentWindow;
        }
    }
    
    private static ImGuiWindow? FindWindowByName(string name) 
    {
        uint id = ImHashStr(name);
        return FindWindowById(id);
    }

    private static ImGuiWindow? FindWindowById(uint id) 
    {
        ImGuiContext ctx = EnsureContext();
        return ctx.WindowsById.GetObject<ImGuiWindow?>(id);
    }
    
    private static void UpdateWindowParentAndRootLinks(ImGuiWindow window, ImGuiWindowFlags flags, ImGuiWindow? parentWindow);
    
    private static Vector2 CalcWindowNextAutoFitSize(ImGuiWindow window);
    
    private static bool IsWindowChildOf(ImGuiWindow window, ImGuiWindow potentialParent, bool popupHierarchy)
    {
        
    }

    private static bool IsWindowWithinBeginStackOf(ImGuiWindow window, ImGuiWindow potentialParent)
    {
        if (window.RootWindow == potentialParent) return true;

        while (window != null)
        {
            if (window == potentialParent) return true;
            window = window.ParentWindowInBeginStack;
        }

        return false;
    }
    
    private static bool IsWindowAbove(ImGuiWindow potentialAbove, ImGuiWindow potentialBelow)
    {
        ImGuiContext ctx = EnsureContext();

        int displayLayerDelta = GetWindowDisplayLayer(potentialAbove) - GetWindowDisplayLayer(potentialBelow);
        if (displayLayerDelta != 0)
        {
            return displayLayerDelta > 0;
        }

        foreach (ImGuiWindow candidate in ctx.Windows)
        {
            if (candidate == potentialAbove) return true;
            if (candidate == potentialBelow) return false;
        }

        return false;
    }

    private static bool IsWindowNavFocusable(ImGuiWindow window) => 
        window.WasActive && window == window.RootWindow && !window.Flags.HasFlag(ImGuiWindowFlags.NoNavFocus);
    
    private static void SetWindowPos(ImGuiWindow window, Vector2 pos, ImGuiCond cond)
    {
        if (cond != ImGuiCond.None && !window.SetWindowPosAllowFlags.HasFlag(cond)) return;
        
        Debug.Assert(cond == ImGuiCond.None || Enum.GetValues<ImGuiCond>().Contains(cond));
        window.SetWindowPosAllowFlags &= ~(ImGuiCond.Once | ImGuiCond.FirstUseEver | ImGuiCond.Appearing);
        window.SetWindowPosVal = new Vector2(float.MaxValue, float.MaxValue);

        Vector2 oldPos = window.Position;
        window.Position = pos.Truncate();
        
        Vector2 offset = window.Position - oldPos;
        if (offset == Vector2.Zero) return;

        MarkIniSettingsDirty(window);
        
        window.DC.CursorPos += offset;
        window.DC.CursorMaxPos += offset;
        window.DC.IdealMaxPos += offset;
        window.DC.CursorStartPos += offset;
    }

    private static void SetWindowSize(ImGuiWindow window, SizeF size, ImGuiCond cond)
    {
        throw new NotImplementedException();
    }
    
    #endregion

    #region -- Windows: Display Order and Focus Order
    
    private static void FocusWindow(ImGuiWindow? window)
    {
        throw new NotImplementedException();
    }

    private static void BringWindowToFocusFront(ImGuiWindow window);
    private static void BringWindowToDisplayFront(ImGuiWindow window);
    private static void BringWindowToDisplayBack(ImGuiWindow window);
    private static void BringWindowToDisplayBehind(ImGuiWindow window);

    #endregion

    #region -- Fonts, drawing

    

    #endregion

    #region -- Init
    
    private static void Initialize()
    {
        ImGuiContext? ctx = CurrentContext as ImGuiContext;
        Debug.Assert(ctx != null);
        Debug.Assert(!ctx.Initialized);

        // Add .ini handle for ImGuiWindow and ImGuiTable types
        {
            ImGuiSettingsHandler iniHandler;
            iniHandler.TypeName = "Window";
            iniHandler.TypeHash = ImHashStr("Window");
            
        }

        ctx.IO.GetClipboardTextFn = ctxObj =>
        {
            ImGuiContext ctx = (ImGuiContext) ctxObj;
            // ctx.Clip
            throw new NotImplementedException();
        };

        ctx.IO.ClipboardUserData = ctx;

        // Create default viewport
        ImGuiViewportP viewport = new ImGuiViewportP();
        ctx.Viewports.Add(viewport);

#if USE_DOCKING
        viewport.Id = ViewportDefaultId;
        viewport.Idx = 0;
        viewport.PlatformWindowCreated = true;
        viewport.Flags = ImGuiViewportFlags.OwnedByApp;

        ctx.ViewportCreatedCount++;
        ctx.PlatformIO.Viewports.Add(viewport);

        DockContextInitialize(ctx);
#endif

        ctx.Initialized = true;
        throw new NotImplementedException();
    }

    private static void Shutdown()
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

        // Clear everything else
        ctx.Windows.Clear();
        ctx.WindowsFocusOrder.Clear();
        ctx.WindowsTempSortOrder.Clear();
        

#if USE_DOCKING
        ctx.CurrentViewport = ctx.MouseViewport = ctx.MouseLastHoveredViewport = null;
#endif
        ctx.Viewports.Clear();

        ctx.Initialized = false;
        throw new NotImplementedException();
    }

    #endregion

    #region -- Basic Helpers for widget code

    private static void ItemSize(ref SizeF size, float textBaselineY = -1)
    {
        throw new NotImplementedException();
    }

    #endregion
    
#if USE_DOCKING

    #region -- Docking
    
    //  - They are convenient to easily create context menus, hence the name.
    private static void DockContextInitialize(ImGuiContext context)     
    {
        throw new NotImplementedException();
    }

    private static void DockContextShutdown(ImGuiContext context) 
    {
        throw new NotImplementedException();
    }

    private static void DockContextClearNodes(ImGuiContext context, uint rootId, bool clearSettingsRefs) 
    {
        Debug.Assert(context == EnsureContext());
        throw new NotImplementedException();
    }
    
    #endregion

    #region -- Docking - Builder function needs to be generally called before the node is used/submitted.

    private static void DockBuilderFinish(uint nodeId);

    #endregion

#endif
    
    #region -- [EXPERIMENTAL] Focus Scope

    private static void PushFocusScope(uint id);
    private static void PopFocusScope();

    #endregion

    #region -- Internal Columns API

    private static void EndColumns();

    #endregion

    #region -- Render helpers
    
    private static void RenderText(Vector2 pos, string text, bool hideTextAfterHash = true)
    {
        throw new NotImplementedException();
    }

    #endregion
}