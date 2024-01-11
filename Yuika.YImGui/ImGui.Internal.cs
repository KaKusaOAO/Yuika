// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public static partial class ImGui
{
    #region -- Windows

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

    private static void UpdateWindowParentAndRootLinks(ImGuiWindow window, ImGuiWindowFlags flags,
        ImGuiWindow? parentWindow) => throw new NotImplementedException();

    private static Vector2 CalcWindowNextAutoFitSize(ImGuiWindow window) => throw new NotImplementedException();

    private static bool IsWindowChildOf(ImGuiWindow window, ImGuiWindow potentialParent, bool popupHierarchy)
    {
        throw new NotImplementedException();
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

        Debug.Assert(cond == ImGuiCond.None || Enum.GetValues(typeof(ImGuiCond)).Cast<ImGuiCond>().Contains(cond));
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static RectangleF WindowRectAbsToRel(ImGuiWindow window, RectangleF rect)
    {
        Vector2 off = window.DC.CursorStartPos;
        return new RectangleF(rect.Location - off.AsSize(), rect.Size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static RectangleF WindowRectRelToAbs(ImGuiWindow window, RectangleF rect)
    {
        Vector2 off = window.DC.CursorStartPos;
        return new RectangleF(rect.Location + off.AsSize(), rect.Size);
    }

    #endregion

    #region -- Windows: Display Order and Focus Order

    private static void FocusWindow(ImGuiWindow? window)
    {
        throw new NotImplementedException();
    }

    private static void FocusTopMostWindowUnderOne(ImGuiWindow underThisWindow, ImGuiWindow ignoreWindow,
        ImGuiViewport filterViewport, ImGuiFocusRequestFlags flags) => throw new NotImplementedException();

    private static void BringWindowToFocusFront(ImGuiWindow window) => throw new NotImplementedException();
    private static void BringWindowToDisplayFront(ImGuiWindow window) => throw new NotImplementedException();
    private static void BringWindowToDisplayBack(ImGuiWindow window) => throw new NotImplementedException();
    private static void BringWindowToDisplayBehind(ImGuiWindow window) => throw new NotImplementedException();

    #endregion

    #region -- Fonts, drawing

    private static void SetCurrentFont(ImFont font) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ImFont GetDefaultFont()
    {
        ImGuiContext ctx = EnsureContext();
        return ctx.IO.FontDefault ?? ctx.IO.Fonts!.Fonts.First();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ImDrawList GetForegroundDrawList(ImGuiWindow window) => GetForegroundDrawList(window.Viewport);

    #endregion

    #region -- Init

    private static void Initialize()
    {
        ImGuiContext? ctx = CurrentContext as ImGuiContext;
        Debug.Assert(ctx != null);
        Debug.Assert(!ctx.Initialized);

        // Add .ini handle for ImGuiWindow and ImGuiTable types
        {
            ImGuiSettingsHandler iniHandler = new ImGuiSettingsHandler();
            iniHandler.TypeName = "Window";
            iniHandler.TypeHash = ImHashStr("Window");
            AddSettingsHandler(iniHandler);
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

    #region -- NewFrame
    // ...
    #endregion

    #region -- Generic context hooks
    // ...
    #endregion

    #region -- Viewports
    // ...
    #endregion

    #region -- Settings

    private static void MarkIniSettingsDirty()
    {
        ImGuiContext ctx = EnsureContext();
        if (ctx.SettingsDirtyTimer <= 0)
        {
            ctx.SettingsDirtyTimer = ctx.IO.IniSavingRate;
        }
    }

    private static void MarkIniSettingsDirty(ImGuiWindow window)
    {
        ImGuiContext ctx = EnsureContext();
        if (!window.Flags.HasFlag(ImGuiWindowFlags.NoSavedSettings))
        {
            if (ctx.SettingsDirtyTimer <= 0)
            {
                ctx.SettingsDirtyTimer = ctx.IO.IniSavingRate;
            }
        }
    }

    #endregion

    #region -- Basic Helpers for widget code

    private static void ItemSize(SizeF size, float textBaselineY = -1)
    {
        throw new NotImplementedException();
    }

    private static bool ItemAdd(RectangleF bb, uint id, RectangleF? navBb = null,
        ImGuiItemFlags extraFlags = ImGuiItemFlags.None)
    {
        ImGuiContext ctx = EnsureContext();
        ImGuiWindow window = ctx.CurrentWindow!;

        ctx.LastItemData.Id = id;
        ctx.LastItemData.Rect = bb;
        ctx.LastItemData.NavRect = navBb ?? bb;
        ctx.LastItemData.InFlags = ctx.CurrentItemFlags | ctx.NextItemData.ItemFlags | extraFlags;
        ctx.LastItemData.StatusFlags = ImGuiItemStatusFlags.None;

        if (id != 0)
        {
            throw new NotImplementedException();
        }

        ctx.NextItemData.Flags = ImGuiNextItemDataFlags.None;
        ctx.NextItemData.ItemFlags = ImGuiItemFlags.None;

#if ENABLE_TEST_ENGINE
        if (id != 0)
        {
        }
#endif

        bool isRectVisible = bb.IntersectsWith(window.ClipRect);
        if (!isRectVisible)
        {
            if (id == 0 || (id != ctx.ActiveId && id != ctx.ActiveIdPreviousFrame && id != ctx.NavId))
            {
                if (!ctx.LogEnabled)
                {
                    return false;
                }
            }
        }

        throw new NotImplementedException();
    }

    private static bool IsWindowContentHoverable(ImGuiWindow window, ImGuiHoveredFlags flags = ImGuiHoveredFlags.None)
    {
        throw new NotImplementedException();
    }

    private static SizeF CalcItemSize(SizeF size, float defaultW, float defaultH) =>
        throw new NotImplementedException();

    private static float CalcWrapWidthForPos(Vector2 pos, float wrapPosX) => throw new NotImplementedException();

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

    private static uint DockNodeGetWindowMenuButtonId(ImGuiDockNode node) => ImHashStr("#COLLAPSE", seed: node.Id);

    private static ImGuiDockNode? GetWindowDockNode()
    {
        ImGuiContext ctx = EnsureContext();
        return ctx.CurrentWindow!.DockNode;
    }

    private static bool GetWindowAlwaysWantOwnTabBar(ImGuiWindow window)
    {
        ImGuiContext ctx = EnsureContext();
        if (ctx.IO.ConfigDockingAlwaysTabBar || window.WindowClass.DockingAlwaysTabBar)
        {
            if (!window.Flags.HasFlag(ImGuiWindowFlags.ChildWindow | ImGuiWindowFlags.NoTitleBar |
                                      ImGuiWindowFlags.NoDocking))
            {
                if (!window.IsFallbackWindow) return true;
            }
        }

        return false;
    }

    private static unsafe void BeginDocked(ImGuiWindow window, bool* open)
    {
        ImGuiContext ctx = EnsureContext();
        window.DockIsActive = window.DockNodeIsVisible = window.DockTabIsVisible = false;

        bool autoDockNode = GetWindowAlwaysWantOwnTabBar(window);
        if (autoDockNode)
        {
            if (window.DockId == 0)
            {
                Debug.Assert(window.DockNode == null);
                window.DockId = DockContextGenNodeId(ctx);
            }
        }
        else
        {
            bool wantUndock = false;
            wantUndock |= window.Flags.HasFlag(ImGuiWindowFlags.NoDocking);
            wantUndock |= ctx.NextWindowData.Flags.HasFlag(ImGuiNextWindowDataFlags.HasPos) &&
                          window.SetWindowPosAllowFlags.HasFlag(ctx.NextWindowData.PosCond) &&
                          ctx.NextWindowData.PosUndock;
            if (wantUndock)
            {
                DockContextProcessUndockWindow(ctx, window);
                return;
            }
        }

        ImGuiDockNode? node = window.DockNode;
        if (node != null)
        {
            Debug.Assert(window.DockId == node.Id);
        }

        if (window.DockId != 0 && node == null)
        {
            node = DockContextBindNodeToWindow(ctx, window);
            if (node == null) return;
        }

        if (node.LastFrameAlive < ctx.FrameCount)
        {
            ImGuiDockNode rootNode = DockNodeGetRootNode(node);
            if (rootNode.LastFrameAlive < ctx.FrameCount)
            {
                DockContextProcessUndockWindow(ctx, window);
            }
            else
            {
                window.DockIsActive = true;
            }

            return;
        }

        // Store style overrides

        // We can have zero-sized nodes (e.g. children of a small-size dockspace)
        Debug.Assert(node.HostWindow != null);
        Debug.Assert(node.IsLeafNode());
        Debug.Assert(node.Size is {Width: >= 0, Height: >= 0});
        node.State = ImGuiDockNodeState.HostWindowVisible;

        // Undock if we are submitted earlier than the host window
        if (!node.MergedFlags.HasFlag(ImGuiDockNodeFlags.KeepAliveOnly) &&
            window.BeginOrderWithinContext < node.HostWindow.BeginOrderWithinContext)
        {
            DockContextProcessUndockWindow(ctx, window);
            return;
        }

        // Position/Size window
        SetNextWindowPos(node.Position);
        SetNextWindowSize(node.Size);
        ctx.NextWindowData.PosUndock = false;
        window.DockIsActive = true;
        window.DockNodeIsVisible = true;
        window.DockTabIsVisible = false;
        if (node.MergedFlags.HasFlag(ImGuiDockNodeFlags.KeepAliveOnly)) return;

        // When the window is selected we mark it as visible.
        if (node.VisibleWindow == window)
        {
            window.DockTabIsVisible = true;
        }

        // Update window flag
        Debug.Assert(!window.Flags.HasFlag(ImGuiWindowFlags.ChildWindow));
        window.Flags |= ImGuiWindowFlags.ChildWindow | ImGuiWindowFlags.NoResize;
        window.ChildFlags |= ImGuiChildFlags.AlwaysUseWindowPadding;
        if (node.IsHiddenTabBar() || node.IsNoTabBar())
        {
            window.Flags |= ImGuiWindowFlags.NoTitleBar;
        }
        else
        {
            window.Flags &= ~ImGuiWindowFlags.NoTitleBar;
        }

        // Save new dock order only if the window has been visible once already
        // This allows multiple windows to be created in the same frame and have their respective dock orders preserved.
        if (node.TabBar && window.WasActive)
        {
            window.DockOrder = DockNodeGetTabOrder(window);
        }

        if ((node.WantCloseAll || node.WantCloseTabId == window.TabId) && open != null)
        {
            *open = false;
        }

        // Update ChildId to allow returning from Child to Parent with Escape
        ImGuiWindow parentWindow = window.DockNode!.HostWindow!;
        window.ChildId = parentWindow.GetId(window.Name);

        throw new NotImplementedException();
    }

    private static void SetWindowDock(ImGuiWindow window, uint dockId, ImGuiCond cond)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region -- Docking - Builder function needs to be generally called before the node is used/submitted.

    private static void DockBuilderFinish(uint nodeId) => throw new NotImplementedException();

    #endregion

#endif

    #region -- [EXPERIMENTAL] Focus Scope

    private static void PushFocusScope(uint id) => throw new NotImplementedException();
    private static void PopFocusScope() => throw new NotImplementedException();

    #endregion

    #region -- Internal Columns API

    private static void EndColumns() => throw new NotImplementedException();

    #endregion

    #region -- Tables: Internals

    // Implementations are in ImGui.Tables.cs

    #endregion

    #region -- Tables: Settings

    // Implementations are in ImGui.Tables.cs

    #endregion

    #region -- Tab Bars

    // ...

    #endregion

    #region -- Render helpers

    private static void RenderText(Vector2 pos, string text, bool hideTextAfterHash = true)
    {
        throw new NotImplementedException();
    }

    private static void RenderTextWrapped(Vector2 pos, string text, float wrapWidth)
    {
        throw new NotImplementedException();
    }

    private static void RenderNavHighlight(RectangleF bb, uint id,
        ImGuiNavHighlightFlags flags = ImGuiNavHighlightFlags.TypeDefault)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region -- Render helpers (those functions don't access any ImGui state!)

    private static void RenderArrow(ImDrawList drawList, Vector2 pos, ColorF col, ImGuiDir dir, float scale = 1) => 
        throw new NotImplementedException();

    private static void RenderBullet(ImDrawList drawList, Vector2 pos, ColorF col) => 
        throw new NotImplementedException();

    #endregion

    #region -- Widgets

    // ...

    #endregion

    #region -- Misc maths helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFloatAboveGuaranteedIntegerPrecision(float f) => f is <= -16777216 or >= 16777216;

    #endregion

    #region -- Debug Log

    [StringFormatMethod("format")]
    internal static void DebugLog(string format, params object[] args)
    {
        Console.WriteLine(format, args);
    }

    #endregion

    #region -- Debug Tools

    internal static void DebugBreakClearData() => throw new NotImplementedException();

    internal static bool DebugBreakButton(string label, string descriptionOfLocation) => 
        throw new NotImplementedException();

    internal static void DebugBreakButtonTooltip(bool keyboardOnly, string descriptionOfLocation) => 
        throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DebugStartItemPicker()
    {
        ImGuiContext ctx = EnsureContext();
        ctx.DebugItemPickerActive = true;
    }

    internal static void ShowFontAtlas(ImFontAtlas atlas) => throw new NotImplementedException();

    internal static unsafe void DebugHookIdInfo(uint id, ImGuiDataType dataType, object? data)
    {
        ImGuiContext ctx = EnsureContext();
        ImGuiWindow window = ctx.CurrentWindow!;
        ImGuiIDStackTool tool = ctx.DebugIDStackTool;

        switch (dataType)
        {
            case ImGuiDataType.S32:
                break;
            case (ImGuiDataType) ImGuiDataTypePrivate.String:
                break;
            case (ImGuiDataType) ImGuiDataTypePrivate.Pointer:
                break;
            case (ImGuiDataType) ImGuiDataTypePrivate.ID:
                break;
            default:
                throw new ImGuiException();
        }

        throw new NotImplementedException();
    }

    #endregion
}