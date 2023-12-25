using System.Numerics;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

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