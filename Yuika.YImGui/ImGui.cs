// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public static partial class ImGui
{
    #region -- Content creation and access

    // - Each context create its own ImFontAtlas by default. 
    //   You may instance one yourself and pass it to CreateContext() to share a font atlas between contexts.

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
    #endregion

    #region -- Main
    
    // ReSharper disable once InconsistentNaming
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
        ctx.IO.Fonts!.Locked = false;
        
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
    #endregion

    #region -- Demo, Debug, Information

    public static void ShowDemoWindow(ref bool open) => throw new NotImplementedException();
    public static unsafe void ShowDemoWindow(bool* open) => throw new NotImplementedException();
    public static void ShowMetricsWindow(ref bool open) => throw new NotImplementedException();
    public static unsafe void ShowMetricsWindow(bool* open) => throw new NotImplementedException();
    public static void ShowDebugLogWindow(ref bool open) => throw new NotImplementedException();
    public static unsafe void ShowDebugLogWindow(bool* open) => throw new NotImplementedException();
    public static void ShowIdStackToolWindow(ref bool open) => throw new NotImplementedException();
    public static unsafe void ShowIdStackToolWindow(bool* open) => throw new NotImplementedException();
    public static void ShowAboutWindow(ref bool open) => throw new NotImplementedException();
    public static unsafe void ShowAboutWindow(bool* open) => throw new NotImplementedException();
    public static void ShowStyleEditor(ref ImGuiStyle style) => throw new NotImplementedException();
    public static void ShowStyleSelector(string label) => throw new NotImplementedException();
    public static void ShowFontSelector(string label) => throw new NotImplementedException();
    public static void ShowUserGuide() => throw new NotImplementedException();
    public static string Version { get; }

    #endregion

    #region -- Styles

    public static void StyleColorsDark(ImGuiStyle? dst = null)
    {
        ImGuiStyle style = dst ?? Style;
        style.Colors = ImGuiColors.Dark;
    }
    
    public static void StyleColorsLight(ImGuiStyle? dst = null)
    {
        ImGuiStyle style = dst ?? Style;
        style.Colors = ImGuiColors.Light;
    }
    
    public static void StyleColorsClassic(ImGuiStyle? dst = null)
    {
        ImGuiStyle style = dst ?? Style;
        style.Colors = ImGuiColors.Classic;
    }

    #endregion

    #region -- Window

    public static bool Begin(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        unsafe 
        {
            return Begin(name, null, flags);
        }
    }

    public static bool Begin(string name, ref bool open, ImGuiWindowFlags flags = ImGuiWindowFlags.None) 
    {
        unsafe
        {
            return Begin(name, (bool*) Unsafe.AsPointer(ref open), flags);
        }
    }

    public static unsafe bool Begin(string name, bool* open, ImGuiWindowFlags flags = ImGuiWindowFlags.None) 
    {
        ImGuiContext ctx = EnsureContext();
        ImGuiStyle style = ctx.Style;

        Debug.Assert(name != null && name.Length > 0);
        Debug.Assert(ctx.WithinFrameScope);
        Debug.Assert(ctx.FrameCountEnded != ctx.FrameCount);

        // Find or create window.
        ImGuiWindow? window = FindWindowByName(name);
        bool windowJustCreated = window == null;
        if (windowJustCreated)
        {
            window = CreateNewWindow(name, flags);
        }

        Debug.Assert(window != null);

        if (flags.HasFlag(ImGuiWindowFlags.NoInputs)) 
        {
            flags |= ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
        }

        if (flags.HasFlag(ImGuiWindowFlags.NavFlattened)) 
        {
            Debug.Assert(flags.HasFlag(ImGuiWindowFlags.ChildWindow));
        }

        int currentFrame = ctx.FrameCount;
        bool firstBeginOfTheFrame = window.LastFrameActive != currentFrame;
        window.IsFallbackWindow = !ctx.CurrentWindowStack.Any() && ctx.WithinFrameScopeWithImplicitWindow;

        // Update the Appearing flag (note: the BeginDocked() path may also set this to true later)
        bool windowJustActivatedByUser = window.LastFrameActive < currentFrame - 1;
        if (flags.HasFlag(ImGuiWindowFlags.Popup))
        {
            ImGuiPopupData popupRef = ctx.OpenPopupStack[ctx.BeginPopupStack.Count];
            windowJustActivatedByUser |= window.PopupId != popupRef.PopupId;
            windowJustActivatedByUser |= window != popupRef.Window;
        }

        // Update Flags, LastFrameActive, BeginOrderXXX fields
        bool windowWasAppearing = window.Appearing;
        if (firstBeginOfTheFrame)
        {
            UpdateWindowInFocusOrderList(window, windowJustCreated, flags);
            window.Appearing = windowJustActivatedByUser;
            if (window.Appearing)
            {
                SetWindowConditionAllowFlags(window, ImGuiCond.Appearing, true);
            }

            window.FlagsPreviousFrame = window.Flags;
            window.Flags = flags;
            window.ChildFlags = ctx.NextWindowData.Flags.HasFlag(ImGuiNextWindowDataFlags.HasChildFlags)
                ? ctx.NextWindowData.ChildFlags
                : ImGuiChildFlags.None;
            
            window.LastFrameActive = currentFrame;
            window.LastTimeActive = (float)ctx.Time;
            window.BeginOrderWithinParent = 0;
            window.BeginOrderWithinContext = (short)ctx.WindowsActiveCount++;
        }
        else 
        {
            flags = window.Flags;
        }

#if USE_DOCKING
        // Docking
        // (NB: during the frame dock nodes are created, it is possible that
        // `window.DockIsActive == false` even though `window.DockNode.Windows.Size > 1`)
        Debug.Assert(window.DockNode == null || window.DockNodeAsHost == null);

        if (ctx.NextWindowData.Flags.HasFlag(ImGuiNextWindowDataFlags.HasDock))
        {
            SetWindowDock(window, ctx.NextWindowData.DockId, ctx.NextWindowData.DockCond);
        }

        if (firstBeginOfTheFrame)
        {
            bool hasDockNode = window.DockId != 0 || window.DockNode != null;
            bool newAutoDockNode = !hasDockNode && GetWindowAlwaysWantOwnTabBar(window);
            bool dockNodeWasVisible = window.DockNodeIsVisible;
            bool dockTabWasVisible = window.DockTabIsVisible;

            if (hasDockNode || newAutoDockNode)
            {
                BeginDocked(window, open);
                flags = window.Flags;

                if (window.DockIsActive)
                {
                    Debug.Assert(window.DockNode != null);
                    ctx.NextWindowData.Flags &= ~ImGuiNextWindowDataFlags.HasSizeConstraint;
                }
                
                // Amend the Appearing flag
                if (window.DockTabIsVisible && !dockTabWasVisible && dockNodeWasVisible && !window.Appearing &&
                    !windowWasAppearing)
                {
                    window.Appearing = true;
                    SetWindowConditionAllowFlags(window, ImGuiCond.Appearing, true);
                }
            }
            else
            {
                window.DockIsActive = window.DockNodeIsVisible = window.DockTabIsVisible = false;
            }
        }
#endif
        
        // Parent window is latched only on the first call to Begin() of the frame, so further append-calls can be done
        // from a different window stack
#if USE_DOCKING
        ImGuiWindow? parentWindowInStack = window.DockIsActive && window.DockNode?.HostWindow != null
            ? window.DockNode.HostWindow
            : ctx.CurrentWindowStack.LastOrDefault()?.Window;
#else
        ImGuiWindow? parentWindowInStack = ctx.CurrentWindowStack.LastOrDefault()?.Window;
#endif
        ImGuiWindow? parentWindow = firstBeginOfTheFrame
            ? flags.HasFlag(ImGuiWindowFlags.ChildWindow | ImGuiWindowFlags.Popup) ? parentWindowInStack : null
            : window.ParentWindow; 
        Debug.Assert(parentWindow != null || !flags.HasFlag(ImGuiWindowFlags.ChildWindow));
        
        // We allow window memory to be compacted so recreated the base stack when needed
        if (!window.IdStack.Any())
        {
            window.IdStack.Add(window.Id);
        }
        
        // Add to stack
        ctx.CurrentWindow = window;
        ImGuiWindowStackData windowStackData = new ImGuiWindowStackData
        {
            Window = window,
            ParentLastItemDataBackup = ctx.LastItemData
        };
        windowStackData.StackSizesOnBegin.SetToContextState(ctx);
        ctx.CurrentWindowStack.Add(windowStackData);

        if (flags.HasFlag(ImGuiWindowFlags.ChildMenu))
        {
            ctx.BeginMenuCount++;
        }

        // Update ->RootWindow and others pointers (before any possible call to FocusWindow)
        if (firstBeginOfTheFrame)
        {
            UpdateWindowParentAndRootLinks(window, flags, parentWindow);
            window.ParentWindowInBeginStack = parentWindowInStack;
        }

        if (!flags.HasFlag(ImGuiWindowFlags.NavFlattened))
        {
            PushFocusScope(window.Id);
        }

        window.NavRootFocusScopeId = ctx.CurrentFocusScopeId;
        ctx.CurrentWindow = null;

        if (flags.HasFlag(ImGuiWindowFlags.Popup))
        {
            ImGuiPopupData popupRef = ctx.OpenPopupStack[ctx.BeginPopupStack.Count];
            popupRef.Window = window;
            popupRef.ParentNavLayer = (int) parentWindowInStack!.DC.NavLayerCurrent;
            ctx.BeginPopupStack.Add(popupRef);
            window.PopupId = popupRef.PopupId;
        }
        
        // Process SetNextWindow***() calls
        bool windowPosSetByApi = false;
        bool windowSizeXSetByApi = false;
        bool windowSizeYSetByApi = false;
        
        throw new NotImplementedException();

        // When reusing window again multiple times a frame, just append content (don't need to setup again)
        if (firstBeginOfTheFrame)
        {
            // Initialize
            bool windowIsChildTooltip = flags.HasFlag(ImGuiWindowFlags.ChildWindow) && flags.HasFlag(ImGuiWindowFlags.Tooltip);
            bool windowJustAppearingAfterHiddenForResize = window.HiddenFramesCannotSkipItems > 0;
            window.Active = true;
            window.HasCloseButton = open != null;
            window.ClipRect = new RectangleF(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
            throw new NotImplementedException();
            
            if (flags.HasFlag(ImGuiWindowFlags.DockNodeHost))
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
            
            bool windowTitleVisibleElsewhere = false;
            if ((window.Viewport != null && window.Viewport.Window == window) || window.DockIsActive)
            {
                windowTitleVisibleElsewhere = true;
            }
            
            throw new NotImplementedException();

            if (windowTitleVisibleElsewhere && !windowJustCreated && name != window.Name)
            {
                window.Name = name;
            }

            throw new NotImplementedException();
        }
        else 
        {
            throw new NotImplementedException();
        }

        if (!flags.HasFlag(ImGuiWindowFlags.DockNodeHost))
        {
            throw new NotImplementedException();
        }

        throw new NotImplementedException();

        // Update visibility
        if (firstBeginOfTheFrame)
        {
            if (window.DockIsActive && !window.DockTabIsVisible)
            {
                if (window.LastFrameJustFocused == ctx.FrameCount)
                {
                    window.HiddenFramesCannotSkipItems = 1;
                }
                else 
                {
                    window.HiddenFramesCanSkipItems = 1;
                }
            }

            throw new NotImplementedException();
            
            // Don't render if style alpha is 0.0 at the time of Begin().
            // This is arbitrary and inconsistent but has been there for a long while (may remove at some point)
            if (style.Alpha <= 0)
            {
                window.HiddenFramesCanSkipItems = 1;
            }

            // Update the Hidden flag
            bool hiddenRegular = window.HiddenFramesCanSkipItems > 0 || window.HiddenFramesCannotSkipItems > 0;
            window.Hidden = hiddenRegular || window.HiddenFramesForRenderOnly > 0;

            if (window.DisableInputFrames > 0)
            {
                window.DisableInputFrames--;
                window.Flags |= ImGuiWindowFlags.NoInputs;
            }

            // Update the SkipItems flag, used to early out of all items functions (no layout required)
            bool skipItems = false;
            if (window.Collapsed || !window.Active || hiddenRegular)
            {
                if (window.AutoFitFramesX <= 0 && window.AutoFitFramesY <= 0 && window.HiddenFramesCannotSkipItems <= 0)
                {
                    skipItems = true;
                }
            }

            window.SkipItems = skipItems;

            // Restore NavLayersActiveMaskNext to previous value when not visible, so a CTRL+Tab back can use a safe value.
            if (window.SkipItems)
            {
                window.DC.NavLayersActiveMaskNext = window.DC.NavLayersActiveMask;
            }

            if (window.SkipItems && !window.Appearing)
            {
                Debug.Assert(!window.Appearing);
            }
        }

        if (!window.IsFallbackWindow && ((ctx.IO.ConfigDebugBeginReturnValueOnce && windowJustCreated) ||
                                         (ctx.IO.ConfigDebugBeginReturnValueLoop &&
                                          ctx.DebugBeginReturnValueCullDepth == ctx.CurrentWindowStack.Count)))
        {
            if (window.AutoFitFramesX > 0) window.AutoFitFramesX++;
            if (window.AutoFitFramesY > 0) window.AutoFitFramesY++;
            return false;
        }

        return !window.SkipItems;
    }

    public static void End()
    {
        ImGuiContext ctx = EnsureContext();
        ImGuiWindow window = ctx.CurrentWindow!;

        if (ctx.CurrentWindowStack.Count <= 1 && ctx.WithinFrameScopeWithImplicitWindow)
        {
            if (ctx.CurrentWindowStack.Count <= 1)
                throw new ImGuiException("Calling End() too many times!");
            
            return;
        }
        
        Debug.Assert(ctx.CurrentWindowStack.Any());

        if (window.Flags.HasFlag(ImGuiWindowFlags.ChildWindow) 
#if USE_DOCKING
            &&
            !window.Flags.HasFlag(ImGuiWindowFlags.DockNodeHost) && !window.DockIsActive
#endif
            )
        {
            if (!ctx.WithinEndChild)
                throw new ImGuiException("Must call EndChild() and not End()!");
        }

        if (window.DC.CurrentColumns != null) EndColumns();
        
#if USE_DOCKING
        if (!window.Flags.HasFlag(ImGuiWindowFlags.DockNodeHost)) PopClipRect();
#else
        PopClipRect();
#endif
        
        if (!window.Flags.HasFlag(ImGuiWindowFlags.NavFlattened)) PopFocusScope();

        throw new NotImplementedException();
    }

    #endregion

    #region -- Child Windows

    public static bool BeginChild(string strId, SizeF size = default, ImGuiChildFlags childFlags = ImGuiChildFlags.None,
        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None) => throw new NotImplementedException();

    public static bool BeginChild(uint id, SizeF size = default, ImGuiChildFlags childFlags = ImGuiChildFlags.None,
        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None) => throw new NotImplementedException();

    public static void EndChild()
    {
        ImGuiContext ctx = EnsureContext();
        ImGuiWindow childWindow = ctx.CurrentWindow!;
        
        Debug.Assert(!ctx.WithinEndChild);
        Debug.Assert(childWindow.Flags.HasFlag(ImGuiWindowFlags.ChildWindow));

        ctx.WithinEndChild = true;
        SizeF childSize = childWindow.Size;
        End();

        if (childWindow.BeginCount == 1)
        {
            ImGuiWindow parentWindow = ctx.CurrentWindow!;
            RectangleF bb = new RectangleF(parentWindow.DC.CursorPos.AsPoint(),
                new SizeF(parentWindow.DC.CursorPos.AsPoint()) + childSize);
            ItemSize(childSize);

            if ((childWindow.DC.NavLayersActiveMask != 0 || childWindow.DC.NavWindowHasScrollY) &&
                !childWindow.Flags.HasFlag(ImGuiWindowFlags.NavFlattened))
            {
                ItemAdd(bb, childWindow.ChildId);
                RenderNavHighlight(bb, childWindow.ChildId);
            }
            else
            {
                ItemAdd(bb, 0);
            }

            if (ctx.HoveredWindow == childWindow)
            {
                ctx.LastItemData.StatusFlags |= ImGuiItemStatusFlags.HoveredWindow;
            }
        }

        ctx.WithinEndChild = false;
        throw new NotImplementedException();
    }

    #endregion

    #region -- Windows Utilities
    // - 'current window' = the window we are appending into while inside a Begin()/End() block. 'next window' = next window we will Begin() into.

    public static bool IsWindowAppearing => CurrentWindowRead.Appearing;
    public static bool IsWindowCollapsed => CurrentWindowRead.Collapsed;

    public static bool IsWindowFocused(ImGuiFocusedFlags flags = ImGuiFocusedFlags.None)
    {
        ImGuiContext ctx = EnsureContext();
        
        ImGuiWindow? refWindow = ctx.NavWindow;
        if (refWindow == null) return false;
        if (flags.HasFlag(ImGuiFocusedFlags.AnyWindow)) return true;
        
        ImGuiWindow curWindow = ctx.CurrentWindow;
        Debug.Assert(curWindow != null);

        bool popupHierarchy = !flags.HasFlag(ImGuiFocusedFlags.NoPopupHierarchy);
        if (flags.HasFlag(ImGuiFocusedFlags.RootWindow))
        {
            curWindow = GetCombinedRootWindow(curWindow, popupHierarchy);
        }

        if (flags.HasFlag(ImGuiFocusedFlags.ChildWindows))
        {
            return IsWindowChildOf(refWindow, curWindow, popupHierarchy);
        }

        return refWindow == curWindow;
    }

    public static bool IsWindowHovered(ImGuiHoveredFlags flags = ImGuiHoveredFlags.None)
    {
        Debug.Assert(((int) flags & (int) ~ImGuiHoveredFlagsPrivate.AllowedMaskForIsWindowHovered) == 0, 
            "Invalid flags for IsWindowHovered()!");

        ImGuiContext ctx = EnsureContext();
        ImGuiWindow? refWindow = ctx.HoveredWindow;
        if (refWindow == null) return false;
        
        
        if (!flags.HasFlag(ImGuiHoveredFlags.AnyWindow))
        {
            ImGuiWindow curWindow = ctx.CurrentWindow;
            Debug.Assert(curWindow != null);

            bool popupHierarchy = !flags.HasFlag(ImGuiHoveredFlags.NoPopupHierarchy);
            if (flags.HasFlag(ImGuiHoveredFlags.RootWindow))
            {
                curWindow = GetCombinedRootWindow(curWindow, popupHierarchy);
            }

            bool result;
            if (flags.HasFlag(ImGuiHoveredFlags.ChildWindows))
            {
                result = IsWindowChildOf(refWindow, curWindow, popupHierarchy);
            }
            else
            {
                result = refWindow == curWindow;
            }

            if (!result) return false;
        }

        if (!IsWindowContentHoverable(refWindow, flags)) return false;

        if (!flags.HasFlag(ImGuiHoveredFlags.AllowWhenBlockedByActiveItem))
        {
            if (ctx.ActiveId != 0 && !ctx.ActiveIdAllowOverlap && ctx.ActiveId != refWindow.MoveId) return false;
        }

        if (flags.HasFlag(ImGuiHoveredFlags.ForTooltip))
        {
            flags = ApplyHoverFlagsForTooltip(flags, ctx.Style.HoverFlagsForTooltipMouse);
        }

        if (flags.HasFlag(ImGuiHoveredFlags.Stationary) && ctx.HoverWindowUnlockedStationaryId != refWindow.Id)
        {
            return false;
        }

        return true;
    }
    
    public static ImDrawList WindowDrawList => throw new NotImplementedException();
    public static Vector2 WindowPos => EnsureContext().CurrentWindow.Position;
    public static SizeF WindowSize => throw new NotImplementedException();
    public static float WindowWidth => EnsureContext().CurrentWindow.Size.Width;
    public static float WindowHeight => EnsureContext().CurrentWindow.Size.Height;
    
    #endregion

    #region -- Window manipulation

    public static void SetNextWindowPos(Vector2 pos, ImGuiCond cond = ImGuiCond.None, Vector2 pivot = default) 
        => throw new NotImplementedException();
    public static void SetNextWindowSize(SizeF size, ImGuiCond cond = ImGuiCond.None) 
        => throw new NotImplementedException();

    public static void SetNextWindowSizeConstraints(SizeF min, SizeF max, ImGuiSizeCallback? customCallback = null,
        object? customCallbackData = null) => throw new NotImplementedException();
    public static void SetNextWindowContentSize(SizeF size) => throw new NotImplementedException();
    public static void SetNextWindowCollapsed(bool collapsed, ImGuiCond cond = ImGuiCond.None) 
        => throw new NotImplementedException();
    public static void SetNextWindowFocus() => throw new NotImplementedException();
    public static void SetNextWindowScroll(Vector2 scroll) => throw new NotImplementedException();
    public static void SetNextWindowBgAlpha(float alpha) => throw new NotImplementedException();

    public static void SetWindowPos(Vector2 pos, ImGuiCond cond = ImGuiCond.None)
    {
        ImGuiWindow window = CurrentWindowRead;
        SetWindowPos(window, pos, cond);
    }

    public static void SetWindowSize(SizeF size, ImGuiCond cond = ImGuiCond.None)
    {
        ImGuiWindow window = EnsureContext().CurrentWindow!;
        SetWindowSize(window, size, cond);
    }
    
    public static void SetWindowCollapsed(bool collapsed, ImGuiCond cond = ImGuiCond.None)
        => throw new NotImplementedException();
    public static void SetWindowFocus() => throw new NotImplementedException();
    public static void SetWindowFontScale(float scale) => throw new NotImplementedException();

    public static void SetWindowPos(string name, Vector2 pos, ImGuiCond cond = ImGuiCond.None)
    {
        ImGuiWindow? window = FindWindowByName(name);
        if (window == null) return;
        SetWindowPos(window, pos, cond);
    }

    public static void SetWindowSize(string name, SizeF size, ImGuiCond cond = ImGuiCond.None)
    {
        ImGuiWindow? window = FindWindowByName(name);
        if (window == null) return;
        SetWindowSize(window, size, cond);
    }
    
    public static void SetWindowCollapsed(string name, bool collapsed, ImGuiCond cond = ImGuiCond.None)
        => throw new NotImplementedException();

    public static void SetWindowFocus(string? name)
    {
        if (name == null)
        {
            FocusWindow(null);
            return;
        }

        ImGuiWindow? window = FindWindowByName(name);
        if (window == null) return;

        FocusWindow(window);
    }

    #endregion

    #region -- Content region
    // - Retrieve available space from a given point. GetContentRegionAvail() is frequently useful.
    // - Those functions are bound to be redesigned (they are confusing,
    //   incomplete and the Min/Max return values are in local window coordinates which increases confusion)

    public static Vector2 GetContentRegionAvail() => throw new NotImplementedException();
    public static Vector2 GetContentRegionMax() => throw new NotImplementedException();
    public static Vector2 GetWindowContentRegionMin() => throw new NotImplementedException();
    public static Vector2 GetWindowContentRegionMax() => throw new NotImplementedException();

    #endregion

    #region -- Windows Scrolling
    // - Any change of Scroll will be applied at the beginning of next frame in the first call to Begin().
    // - You may instead use SetNextWindowScroll() prior to calling Begin() to avoid this delay, as an alternative to using SetScrollX()/SetScrollY().

    // public static float GetScrollX();
    // public static float GetScrollY();
    // public static void SetScrollX(float scrollX);
    // public static void SetScrollY(float scrollY);

    public static float ScrollX
    {
        get => throw new NotImplementedException(); 
        set => throw new NotImplementedException();
    }

    public static float ScrollY
    {
        get => throw new NotImplementedException(); 
        set => throw new NotImplementedException();
    }

    public static float GetScrollMaxX() => throw new NotImplementedException();
    public static float GetScrollMaxY() => throw new NotImplementedException();
    public static void SetScrollHereX(float centerXRatio = 0.5f) => throw new NotImplementedException();
    public static void SetScrollHereY(float centerYRatio = 0.5f) => throw new NotImplementedException();
    public static void SetScrollFromPosX(float localX, float centerXRatio = 0.5f) => throw new NotImplementedException();
    public static void SetScrollFromPosY(float localY, float centerYRatio = 0.5f) => throw new NotImplementedException();

    #endregion

    #region -- Parameter stacks (shared)

    public static void PushFont(ImFont font) => throw new NotImplementedException();
    public static void PopFont() => throw new NotImplementedException();
    public static void PushStyleColor(ImGuiCol idx, Color color) => throw new NotImplementedException();
    public static void PopStyleColor(int count = 1) => throw new NotImplementedException();
    public static void PushStyleVar(ImGuiStyleVar idx, float val) => throw new NotImplementedException();
    public static void PushStyleVar(ImGuiStyleVar idx, Vector2 val) => throw new NotImplementedException();
    public static void PopStyleVar(int count = 1) => throw new NotImplementedException();
    public static void PushTabStop(bool tabStop) => throw new NotImplementedException();
    public static void PopTabStop() => throw new NotImplementedException();
    public static void PushButtonRepeat(bool repeat) => throw new NotImplementedException();
    public static void PopButtonRepeat() => throw new NotImplementedException();

    #endregion
    
    
    #region -- Parameter stacks (current window)

    public static void PushItemWidth(float itemWidth) => throw new NotImplementedException();
    public static void PopItemWidth() => throw new NotImplementedException();
    public static void SetNextItemWidth(float itemWidth) => throw new NotImplementedException();
    public static float CalcItemWidth() => throw new NotImplementedException();
    public static void PushTextWrapPos(float wrapLocalPosX = 0) => throw new NotImplementedException();
    public static void PopTextWrapPos() => throw new NotImplementedException();
    
    #endregion

    #region -- Style read access
    // - Use the ShowStyleEditor() function to interactively see/edit the colors.

    public static ImFont Font => EnsureContext().Font;
    public static float FontSize => EnsureContext().FontSize;
    public static Vector2 FontTextUvWhitePixel => EnsureContext().DrawListSharedData.TexUvWhitePixel;
    public static Color GetColor(ImGuiCol idx, float alphaMul = 1) => throw new NotImplementedException();
    
    #endregion

    #region -- Layout cursor positioning
    // - By "cursor" we mean the current output position.
    // - The typical widget behavior is to output themselves at the current cursor position, then move the cursor one line down.
    // - You can call SameLine() between widgets to undo the last carriage return and output at the right of the preceding widget.
    // - Attention! We currently have inconsistencies between window-local and absolute positions we will aim to fix with future API:
    //    - Absolute coordinate:        GetCursorScreenPos(), SetCursorScreenPos(), all ImDrawList:: functions. -> this is the preferred way forward.
    //    - Window-local coordinates:   SameLine(), GetCursorPos(), SetCursorPos(), GetCursorStartPos(), GetContentRegionMax(), GetWindowContentRegion*(), PushTextWrapPos()
    // - GetCursorScreenPos() = GetCursorPos() + GetWindowPos(). GetWindowPos() is almost only ever useful to convert from window-local to absolute coordinates.

    public static Vector2 CursorScreenPos
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    
    public static Vector2 CursorPos
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    
    public static float CursorPosX
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    
    public static float CursorPosY
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public static Vector2 CursorStartPos => throw new NotImplementedException();

    #endregion

    #region -- Other layout functions

    public static void Separator() => throw new NotImplementedException();
    public static void SameLine(float offsetFromStartX = 0, float spacing = -1) => throw new NotImplementedException();
    public static void NewLine() => throw new NotImplementedException();
    public static void Spacing() => throw new NotImplementedException();
    public static void Dummy(SizeF size) => throw new NotImplementedException();
    public static void Indent(float indentW = 0) => throw new NotImplementedException();
    public static void Unindent(float indentW = 0) => throw new NotImplementedException();
    public static void BeginGroup() => throw new NotImplementedException();
    public static void EndGroup() => throw new NotImplementedException();
    public static void AlignTextToFramePadding() => throw new NotImplementedException();

    public static float TextLineHeight => throw new NotImplementedException();
    public static float TextLineHeightWithSpacing => throw new NotImplementedException();
    public static float FrameHeight => throw new NotImplementedException();
    public static float FrameHeightWithSpacing => throw new NotImplementedException();

    #endregion

    #region -- ID stack / scopes
    // Read the FAQ (docs/FAQ.md or http://dearimgui.com/faq) for more details about how ID are handled in dear imgui.
    // - Those questions are answered and impacted by understanding of the ID stack system:
    //   - "Q: Why is my widget not reacting when I click on it?"
    //   - "Q: How can I have widgets with an empty label?"
    //   - "Q: How can I have multiple widgets with the same label?"
    // - Short version: ID are hashes of the entire ID stack. If you are creating widgets in a loop you most likely
    //   want to push a unique identifier (e.g. object pointer, loop index) to uniquely differentiate them.
    // - You can also use the "Label##foobar" syntax within widget label to distinguish them from each others.
    // - In this header file we use the "label"/"name" terminology to denote a string that will be displayed + used as an ID,
    //   whereas "str_id" denote a string that is only used as an ID and not normally displayed.

    public static void PushID(string id) => throw new NotImplementedException();
    public static void PushID(int id) => throw new NotImplementedException();
    public static void PopID() => throw new NotImplementedException();
    public static void GetID(string id) => throw new NotImplementedException();

    #endregion

    #region -- Tooltips
    // - Tooltips are windows following the mouse. They do not take focus away.
    // - A tooltip window can contain items of any types. SetTooltip() is a shortcut for the 'if (BeginTooltip()) { Text(...); EndTooltip(); }' idiom.

    public static bool BeginTooltip() => throw new NotImplementedException();
    public static void EndTooltip() => throw new NotImplementedException();
    public static void SetTooltip(string text) => throw new NotImplementedException();

    public static bool BeginItemTooltip() => throw new NotImplementedException();
    public static void SetItemTooltip(string text) => throw new NotImplementedException();

    #endregion

    #region -- Popups, Modals

    public static bool BeginPopup(string strId, ImGuiWindowFlags flags = ImGuiWindowFlags.None) 
        => throw new NotImplementedException();

    public static unsafe bool BeginPopupModal(string name, bool* open = null,
        ImGuiWindowFlags flags = ImGuiWindowFlags.None) => throw new NotImplementedException();

    public static void EndPopup() => throw new NotImplementedException();

    #endregion

    #region -- Popups: open/close functions
    // - OpenPopup(): set popup state to open. ImGuiPopupFlags are available for opening options.
    // - If not modal: they can be closed by clicking anywhere outside them, or by pressing ESCAPE.
    // - CloseCurrentPopup(): use inside the BeginPopup()/EndPopup() scope to close manually.
    // - CloseCurrentPopup() is called by default by Selectable()/MenuItem() when activated (FIXME: need some options).
    // - Use ImGuiPopupFlags_NoOpenOverExistingPopup to avoid opening a popup if there's already one at the same level. This is equivalent to e.g. testing for !IsAnyPopupOpen() prior to OpenPopup().
    // - Use IsWindowAppearing() after BeginPopup() to tell if a window just opened.
    // - IMPORTANT: Notice that for OpenPopupOnItemClick() we exceptionally default flags to 1 (== ImGuiPopupFlags_MouseButtonRight) for backward compatibility with older API taking 'int mouse_button = 1' parameter

    public static void OpenPopup(string strId, ImGuiPopupFlags flags = ImGuiPopupFlags.None) => throw new NotImplementedException();
    public static void OpenPopup(uint id, ImGuiPopupFlags flags = ImGuiPopupFlags.None) => throw new NotImplementedException();

    public static void OpenPopupOnItemClick(string? strId = null,
        ImGuiPopupFlags flags = ImGuiPopupFlags.MouseButtonRight) => throw new NotImplementedException();

    public static void CloseCurrentPopup() => throw new NotImplementedException();

    #endregion

    #region -- Popups: open+begin combined functions helpers
    // - Helpers to do OpenPopup+BeginPopup where the Open action is triggered by e.g. hovering an item and right-clicking.
    // - They are convenient to easily create context menus, hence the name.
    // - IMPORTANT: Notice that BeginPopupContextXXX takes ImGuiPopupFlags just like OpenPopup() and unlike BeginPopup(). For full consistency, we may add ImGuiWindowFlags to the BeginPopupContextXXX functions in the future.
    // - IMPORTANT: Notice that we exceptionally default their flags to 1 (== ImGuiPopupFlags_MouseButtonRight) for backward compatibility with older API taking 'int mouse_button = 1' parameter, so if you add other flags remember to re-add the ImGuiPopupFlags_MouseButtonRight.

    public static bool BeginPopupContextItem(string? strId = null, 
        ImGuiPopupFlags flags = ImGuiPopupFlags.MouseButtonRight) => throw new NotImplementedException();
    
    public static bool BeginPopupContextWindow(string? strId = null,
        ImGuiPopupFlags flags = ImGuiPopupFlags.MouseButtonRight) => throw new NotImplementedException();
    
    public static bool BeginPopupContextVoid(string? strId = null,
        ImGuiPopupFlags flags = ImGuiPopupFlags.MouseButtonRight) => throw new NotImplementedException();

    #endregion

    #region -- Popups: query functions
    // - IsPopupOpen(): return true if the popup is open at the current BeginPopup() level of the popup stack.
    // - IsPopupOpen() with ImGuiPopupFlags_AnyPopupId: return true if any popup is open at the current BeginPopup() level of the popup stack.
    // - IsPopupOpen() with ImGuiPopupFlags_AnyPopupId + ImGuiPopupFlags_AnyPopupLevel: return true if any popup is open.

    public static bool IsPopupOpen(string strId, ImGuiPopupFlags flags = ImGuiPopupFlags.None) 
        => throw new NotImplementedException();

    #endregion
    
    #region -- Tables
    // Implementations are in ImGui.Tables.cs
    #endregion

    #region -- Tables: Headers & Columns declaration
    // Implementations are in ImGui.Tables.cs
    #endregion

    #region -- Tables: Sorting & Miscellaneous functions
    // Implementations are in ImGui.Tables.cs
    #endregion

    #region -- Legacy Columns API (prefer using Tables!)
    // Implementations are in ImGui.Tables.cs
    #endregion

    #region -- Tab Bars, Tabs
    // Implementations are in ImGui.Tables.cs
    #endregion

#if USE_DOCKING
    #region -- Docking

    public static uint GetWindowDockId() => throw new NotImplementedException();
    
    /// <summary>
    /// Is the current window docked into another window?
    /// </summary>
    /// <returns><c>true</c> if the current window is docked into another window.</returns>
    public static bool IsWindowDocked() => throw new NotImplementedException();
    
    #endregion
#endif
    
    #region -- Clipping

    public static void PushClipRect(Vector2 clipRectMin, Vector2 clipRectMax, bool intersectWithCurrentClipRect) 
        => throw new NotImplementedException();
    public static void PopClipRect() => throw new NotImplementedException();

    #endregion

    #region -- Viewports

    /// <summary>
    /// Return primary/default viewport. This can never be <c>null</c>.
    /// </summary>
    public static ImGuiViewport MainViewport => EnsureContext().Viewports.First();

    #endregion

    #region -- Background/Foreground Draw Lists

    /// <summary>
    /// Get background draw list for the viewport associated to the current window.
    /// </summary>
    /// <remarks>
    /// This draw list will be the first rendering one.
    /// Useful to quickly draw shapes/text behind ImGui contents.
    /// </remarks>
    public static ImDrawList BackgroundDrawList 
    {
        get 
        {
            ImGuiContext ctx = EnsureContext();
            return GetBackgroundDrawList(ctx.CurrentWindow!.Viewport);
        }
    }
    
    /// <summary>
    /// Get foreground draw list for the viewport associated to the current window.
    /// </summary>
    /// <remarks>
    /// This draw list will be the last rendered one.
    /// Useful to quickly draw shapes/text over ImGui contents.
    /// </remarks>
    public static ImDrawList ForegroundDrawList 
    { 
        get 
        {
            ImGuiContext ctx = EnsureContext();
            return GetForegroundDrawList(ctx.CurrentWindow!.Viewport);
        }
    }
    
    /// <summary>
    /// Get background draw list for the given viewport.
    /// </summary>
    /// <remarks>
    /// This draw list will be the first rendering one.
    /// Useful to quickly draw shapes/text behind ImGui contents.
    /// </remarks>
    public static ImDrawList GetBackgroundDrawList(ImGuiViewport viewport) => 
        GetViewportBgFgDrawList(viewport, 0, "##Background");
    
    /// <summary>
    /// Get foreground draw list for the given viewport.
    /// </summary>
    /// <remarks>
    /// This draw list will be the last rendered one.
    /// Useful to quickly draw shapes/text over ImGui contents.
    /// </remarks>
    public static ImDrawList GetForegroundDrawList(ImGuiViewport viewport) =>
        GetViewportBgFgDrawList(viewport, 1, "##Foreground");

    #endregion

    #region -- Miscellaneous Utilities

    public static IImDrawListSharedData DrawListSharedData => EnsureContext().DrawListSharedData;

    public static ImGuiStorage StateStorage
    {
        get => EnsureContext().CurrentWindow!.DC.StateStorage;
        
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        set
        {
            ImGuiWindow window = EnsureContext().CurrentWindow!;
            window.DC.StateStorage = value ?? window.StateStorage;
        }
    }

    public static void SetStateStorage(ImGuiStorage? storage) => 
        StateStorage = storage ?? EnsureContext().CurrentWindow!.StateStorage;

    #endregion
    
    #region -- Text Utilities

    public static SizeF CalcTextSize(string text, bool hideTextAfterDoubleHash = false, float wrapWidth = -1)
    {
        throw new NotImplementedException();
    }

    #endregion

#if USE_DOCKING

    #region -- (Optional) Platform/OS interface for multi-viewport support

    public static ImGuiPlatformIO PlatformIO => EnsureContext().PlatformIO;

    public static void UpdatePlatformWindows() => throw new NotImplementedException();
    public static void RenderPlatformWindowsDefault(object? platformRenderArg = null, object? rendererRenderArg = null) 
        => throw new NotImplementedException();
    public static void DestroyPlatformWindows() => throw new NotImplementedException();
    public static ImGuiViewport FindViewportByID(uint id) => throw new NotImplementedException();
    public static ImGuiViewport FindViewportByPlatformHandle(object? platformHandle) 
        => throw new NotImplementedException();

    #endregion
    
#endif
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ImGuiContext EnsureContext()
    {
        ImGuiContext? ctx = CurrentContext as ImGuiContext;
        Debug.Assert(ctx != null, "No current context. Did you call ImGui.CreateContext() and ImGui.CurrentContext = ... ?");
        return ctx;
    }
    
    private static ImGuiWindow CreateNewWindow(string name, ImGuiWindowFlags flags) 
    {
        ImGuiContext ctx = EnsureContext();
        ImGuiWindow window = new ImGuiWindow(ctx, name);
        window.Flags = flags;
        ctx.WindowsById.SetObject(window.Id, window);

        throw new NotImplementedException();

        if (flags.HasFlag(ImGuiWindowFlags.NoBringToFrontOnFocus))
        {
            ctx.Windows.Insert(0, window);
        }
        else
        {
            ctx.Windows.Add(window);
        }

        return window;
    }

    private static uint[] _crc32LookupTable = 
    {
        0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3, 
        0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
        0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 
        0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
        0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 
        0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
        0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F, 
        0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
        0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433, 
        0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
        0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457, 
        0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
        0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 
        0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
        0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F, 
        0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
        0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683, 
        0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
        0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7, 
        0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
        0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B, 
        0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
        0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 
        0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
        0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713, 
        0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
        0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777, 
        0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
        0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 
        0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
        0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF, 
        0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D,
    };

    internal static unsafe uint ImHashData(void* dataPtr, int dataSize, uint seed = 0) 
    {
        uint crc = ~seed;
        byte* data = (byte*) dataPtr;

        while (dataSize-- != 0) 
        {
            crc = (crc >> 8) ^ _crc32LookupTable[(crc & 0xff) ^ *data++];
        }

        return ~crc;
    }

    internal static uint ImHashData(byte[] data, uint seed = 0) 
    {
        unsafe 
        {
            fixed (void* ptr = data) 
            {
                return ImHashData(ptr, data.Length, seed);
            }
        }
    }

    internal static uint ImHashStr(string str, int dataSize = 0, uint seed = 0) 
    {
        seed = ~seed;
        uint crc = seed;
        byte[] data = Encoding.UTF8.GetBytes(str);
        int index = 0;

        if (dataSize != 0) 
        {
            while (dataSize-- != 0)
            {
                byte c = data[index++];
                if (c == '#' && dataSize >= 2 && data[index] == '#' && data[index + 1] == '#') 
                {
                    crc = seed;
                }

                crc = (crc >> 8) ^ _crc32LookupTable[(crc & 0xff) ^ c];
            }
        } 
        else 
        {
            byte c;
            while ((c = data[index++]) != 0)
            {
                if (c == '#' && data[index] == '#' && data[index + 1] == '#') 
                {
                    crc = seed;
                }

                crc = (crc >> 8) ^ _crc32LookupTable[(crc & 0xff) ^ c];
            }
        }

        return ~crc;
    }

    private static ImGuiWindow GetCombinedRootWindow(ImGuiWindow window, bool popupHierarchy)
    {
        ImGuiWindow? lastWindow = null;
        while (lastWindow != window)
        {
            lastWindow = window;
            window = window.RootWindow;
            if (popupHierarchy)
            {
                window = window.RootWindowPopupTree;
            }
        }
        
        return window;
    }

    private static int GetWindowDisplayLayer(ImGuiWindow window) => window.Flags.HasFlag(ImGuiWindowFlags.Tooltip) ? 1 : 0;
    
    private static void UpdateWindowInFocusOrderList(ImGuiWindow window, bool justCreated, ImGuiWindowFlags newFlags)
    {
        ImGuiContext ctx = EnsureContext();

        bool newIsExplicitChild = newFlags.HasFlag(ImGuiWindowFlags.ChildWindow) &&
                                  (!newFlags.HasFlag(ImGuiWindowFlags.Popup) ||
                                   newFlags.HasFlag(ImGuiWindowFlags.ChildMenu));
        bool childFlagChanged = newIsExplicitChild != window.IsExplicitChild;

        if ((justCreated || childFlagChanged) && !newIsExplicitChild)
        {
            Debug.Assert(!ctx.WindowsFocusOrder.Contains(window));
            ctx.WindowsFocusOrder.Add(window);
            window.FocusOrder = (short) (ctx.WindowsFocusOrder.Count - 1);
        } 
        else if (!justCreated && childFlagChanged && newIsExplicitChild)
        {
            Debug.Assert(ctx.WindowsFocusOrder[window.FocusOrder] == window);
            for (int n = window.FocusOrder + 1; n < ctx.WindowsFocusOrder.Count; n++)
            {
                ctx.WindowsFocusOrder[n].FocusOrder--;
            }

            ctx.WindowsFocusOrder.RemoveAt(window.FocusOrder);
            window.FocusOrder = -1;
        }

        window.IsExplicitChild = newIsExplicitChild;
    }

    private static void SetWindowConditionAllowFlags(ImGuiWindow window, ImGuiCond flags, bool enabled)
    {
        window.SetWindowPosAllowFlags =
            enabled ? window.SetWindowPosAllowFlags | flags : window.SetWindowPosAllowFlags & ~flags;
        window.SetWindowSizeAllowFlags =
            enabled ? window.SetWindowSizeAllowFlags | flags : window.SetWindowSizeAllowFlags & ~flags;
        window.SetWindowCollapsedAllowFlags =
            enabled ? window.SetWindowCollapsedAllowFlags | flags : window.SetWindowCollapsedAllowFlags & ~flags;
#if USE_DOCKING
        window.SetWindowDockAllowFlags =
            enabled ? window.SetWindowDockAllowFlags | flags : window.SetWindowDockAllowFlags & ~flags;
#endif
    }

    private static ImGuiHoveredFlags ApplyHoverFlagsForTooltip(ImGuiHoveredFlags userFlags,
        ImGuiHoveredFlags sharedFlags)
    {
        throw new NotImplementedException();
    }

    private static ImDrawList GetViewportBgFgDrawList(ImGuiViewport viewport, int drawListNo, string drawListName)
    {
        throw new NotImplementedException();
    }
}