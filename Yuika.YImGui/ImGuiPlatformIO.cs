using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui;

#if USE_DOCKING

public class ImGuiPlatformIO 
{
    public ViewportCallback? PlatformCreateWindow { get; set; }
    public ViewportCallback? PlatformDestroyWindow { get; set; }
    public ViewportCallback? PlatformShowWindow { get; set; }
    public SetWindowPosCallback? PlatformSetWindowPos { get; set; }
    public GetWindowPosCallback? PlatformGetWindowPos { get; set; }
    public SetWindowSizeCallback? PlatformSetWindowSize { get; set; }
    public GetWindowSizeCallback? PlatformGetWindowSize { get; set; }
    public ViewportCallback? PlatformSetWindowFocus { get; set; }
    public GetWindowBoolCallback? PlatformGetWindowFocus { get; set; }
    public GetWindowBoolCallback? PlatformGetWindowMinimized { get; set; }
    public SetWindowTitleCallback? PlatformSetWindowTitle { get; set; }
    public SetWindowAlphaCallback? PlatformSetWindowAlpha { get; set; }
    public ViewportCallback? PlatformUpdateWindow { get; set; }
    public RenderWindowCallback? PlatformRenderWindow { get; set; }
    public SwapBuffersCallback? PlatformSwapBuffers { get; set; }
    public GetWindowDpiCallback? PlatformGetWindowDpiScale { get; set; }
    public ViewportCallback? PlatformOnChangedViewport { get; set; }
    public CreateVkSurfaceCallback? PlatformCreateVkSurface { get; set; }

    public ViewportCallback? RendererCreateWindow { get; set; }
    public ViewportCallback? RendererDestroyWindow { get; set; }
    public SetWindowSizeCallback? RendererSetWindowSize { get; set; }
    public RenderWindowCallback? RendererRenderWindow { get; set; }
    public SwapBuffersCallback? RendererSwapBuffers { get; set; }

    public List<ImGuiPlatformMonitor> Monitors { get; set; } = new List<ImGuiPlatformMonitor>();
    public List<ImGuiViewport> Viewports { get; set; } = new List<ImGuiViewport>();

    public delegate void ViewportCallback(ImGuiViewport vp);
    public delegate Vector2 GetWindowPosCallback(ImGuiViewport vp);
    public delegate void SetWindowPosCallback(ImGuiViewport vp, Vector2 pos);
    public delegate SizeF GetWindowSizeCallback(ImGuiViewport vp);
    public delegate void SetWindowSizeCallback(ImGuiViewport vp, SizeF size);
    public delegate bool GetWindowBoolCallback(ImGuiViewport vp);
    public delegate void SetWindowTitleCallback(ImGuiViewport vp, string title);
    public delegate void SetWindowAlphaCallback(ImGuiViewport vp, float alpha);
    public delegate void RenderWindowCallback(ImGuiViewport vp, object arg);
    public delegate void SwapBuffersCallback(ImGuiViewport vp, object arg);
    public delegate float GetWindowDpiCallback(ImGuiViewport vp);
    public delegate int CreateVkSurfaceCallback(ImGuiViewport vp, ulong vkInst, IntPtr vkAllocators, out ulong vkSurface);
}

#endif