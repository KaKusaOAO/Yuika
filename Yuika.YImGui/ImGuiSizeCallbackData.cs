using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui;

public class ImGuiSizeCallbackData
{
    public object? UserData { get; set; }
    public Vector2 Position { get; set; }
    public SizeF CurrentSize { get; set; }
    public SizeF DesiredSize { get; set; }
}