using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui;

#if USE_DOCKING

public class ImGuiPlatformMonitor 
{
    public Vector2 MainPosition { get; set; }
    public SizeF MainSize { get; set; }
    public Vector2 WorkPosition { get; set; }
    public SizeF WorkSize { get; set; }
    public float DpiScale { get; set; }
    public IImGuiPlatformMonitorHandle? PlatformHandle { get; set; }
}

#endif