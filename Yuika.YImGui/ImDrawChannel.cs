using System.Drawing;
using System.Numerics;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public class ImDrawChannel
{
    public List<ImDrawCmd> CmdBuffer { get; set; } = new List<ImDrawCmd>();
    public List<ImDrawIdx> IdxBuffer { get; set; } = new List<ImDrawIdx>();   
}

public delegate void ImDrawCallback(ImDrawList list, ImDrawCmd cmd);

[Flags]
public enum ImDrawFlags
{
    None,
    
    Closed                   = 1 << 0,
    
    RoundCornersTopLeft      = 1 << 4,
    RoundCornersTopRight     = 1 << 5,
    RoundCornersBottomLeft   = 1 << 6,
    RoundCornersBottomRight  = 1 << 7,
    RoundCornersNone         = 1 << 8,
    
    RoundCornersTop = RoundCornersTopLeft | RoundCornersTopRight,
    RoundCornersBottom = RoundCornersBottomLeft | RoundCornersBottomRight,
    RoundCornersLeft = RoundCornersTopLeft | RoundCornersBottomLeft,
    RoundCornersRight = RoundCornersTopRight | RoundCornersBottomRight,
    RoundCornersAll = RoundCornersTopLeft | RoundCornersTopRight | RoundCornersBottomLeft | RoundCornersBottomRight,
    
    RoundCornersDefault = RoundCornersAll,
    RoundCornersMask = RoundCornersAll | RoundCornersNone
}

[Flags]
public enum ImDrawListFlags
{
    None,
    
    AntiAliasedLines       = 1 << 0,
    AntiAliasesLinesUseTex = 1 << 1,
    AntiAliasesFill        = 1 << 2,
    AllowVtxOffset         = 1 << 3
}

public class ImDrawList
{
    public List<ImDrawCmd> CmdBuffer { get; set; } = new List<ImDrawCmd>();
    public List<ImDrawIdx> IdxBuffer { get; set; } = new List<ImDrawIdx>();
    public List<ImDrawVert> VtxBuffer { get; set; } = new List<ImDrawVert>();
    public ImDrawListFlags Flags { get; set; }
    
    public uint VtxCurrentIdx { get; set; }
    internal ImDrawListSharedData Data { get; set; }
    public string OwnerName { get; set; }
    public List<ImDrawVert> VtxWriteList { get; set; } = new List<ImDrawVert>();
    public List<ImDrawIdx> IdxWriteList { get; set; } = new List<ImDrawIdx>();
    public List<Vector4> ClipRectStack { get; set; } = new List<Vector4>();
    public List<ImTextureID> TextureIdStack { get; set; } = new List<ImTextureID>();
    public List<Vector2> Path { get; set; } = new List<Vector2>();
    public ImDrawCmdHeader CmdHeader { get; set; }
    public ImDrawListSplitter Splitter { get; set; }
    public float FringeScale { get; set; }

    public void PushClipRect(Vector2 clipRectMin, Vector2 clipRectMax, bool intersectWithCurrentClipRect = false);
    public void PushClipRectFullscreen();
    public void PopClipRect();
    public void PushTextureId(ImTextureID textureId);
    public void PopTextureId();
    
    public Vector2 ClipRectMin
    {
        get
        {
            Vector4 cr = ClipRectStack.LastOrDefault();
            return new Vector2(cr.X, cr.Y);
        }
    }
    
    public Vector2 ClipRectMax
    {
        get
        {
            Vector4 cr = ClipRectStack.LastOrDefault();
            return new Vector2(cr.Z, cr.W);
        }
    }

    public void AddLine(Vector2 p1, Vector2 p2, uint col, float thickness = 1);
}

public class ImFontGlyph
{
    public bool Colored { get; set; }
    public bool Visible { get; set; }
    public int Codepoint { get; set; }
    public float AdvanceX { get; set; }
    public float X0 { get; set; }
    public float Y0 { get; set; }
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float U0 { get; set; }
    public float V0 { get; set; }
    public float U1 { get; set; }
    public float V1 { get; set; }
}

public class ImDrawCmdHeader
{
    
}

public class ImDrawVert
{
    public Vector2 Position { get; set; }
    public Vector2 Uv { get; set; }
    public Color Color { get; set; }
}

public class ImDrawCmd
{
    public Vector4 ClipRect { get; set; }
    public ImTextureID TextureId { get; set; }
    public uint VtxOffset { get; set; }
    public uint IdxOffset { get; set; }
    public uint ElementCount { get; set; }
    public ImDrawCallback? UserCallback { get; set; }
    public object? UserCallbackData { get; set; }
}

