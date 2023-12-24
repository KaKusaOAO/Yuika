namespace Yuika.YImGui;

[Flags]
public enum ImGuiBackendFlags
{
    None,
    
    HasGamepad              = 1 << 0,
    HasMouseCursors         = 1 << 1,
    HasSetMousePos          = 1 << 2,
    RendererHasVtxOffset    = 1 << 3,

#if USE_DOCKING
    
    PlatformHasViewports    = 1 << 10,
    HasMouseHoveredViewport = 1 << 11,
    RendererHasViewports    = 1 << 12,
    
#endif
}