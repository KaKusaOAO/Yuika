// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public class ImDrawList
{
    public List<ImDrawCmd> CmdBuffer { get; set; } = new List<ImDrawCmd>();
    public List<ImDrawIdx> IdxBuffer { get; set; } = new List<ImDrawIdx>();
    public List<ImDrawVert> VtxBuffer { get; set; } = new List<ImDrawVert>();
    public ImDrawListFlags Flags { get; set; }
    
    internal uint VtxCurrentIdx { get; set; }
    internal ImDrawListSharedData Data { get; set; }
    internal string OwnerName { get; set; }
    internal int VtxWriteIndex { get; set; }
    internal int IdxWriteIndex { get; set; }
    internal List<Vector4> ClipRectStack { get; set; } = new List<Vector4>();
    internal List<ImTextureID> TextureIdStack { get; set; } = new List<ImTextureID>();
    internal List<Vector2> Path { get; set; } = new List<Vector2>();
    internal ImDrawCmdHeader CmdHeader { get; set; }
    internal ImDrawListSplitter Splitter { get; set; }
    internal float FringeScale { get; set; }

    public void PushClipRect(Vector2 clipRectMin, Vector2 clipRectMax, bool intersectWithCurrentClipRect = false) => throw new NotImplementedException();
    public void PushClipRectFullscreen() => throw new NotImplementedException();
    public void PopClipRect() => throw new NotImplementedException();
    public void PushTextureId(ImTextureID textureId) => throw new NotImplementedException();
    public void PopTextureId() => throw new NotImplementedException();
    
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

    public void AddLine(Vector2 p1, Vector2 p2, ColorF col, float thickness = 1) => throw new NotImplementedException();

    public void AddRect(Vector2 min, Vector2 max, ColorF col, float rounding = 0, ImDrawFlags flags = ImDrawFlags.None,
        float thickness = 1) => throw new NotImplementedException();

    public void AddRectFilled(Vector2 min, Vector2 max, ColorF col, float rounding = 0,
        ImDrawFlags flags = ImDrawFlags.None) => throw new NotImplementedException();
    
    public void AddText(Vector2 pos, ColorF col, string text) => throw new NotImplementedException();

    public void AddText(ImFont font, float fontSize, Vector2 pos, ColorF col, string text, float wrapWidth = 0,
        RectangleF? cpuFineClipRect = null) => throw new NotImplementedException();

    public void AddBezierCubic(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ColorF col, float thickness, int numSegments = 0) => throw new NotImplementedException();
    
    public void AddBezierQuadratic(Vector2 p1, Vector2 p2, Vector2 p3, ColorF col, float thickness, int numSegments = 0) => throw new NotImplementedException();

    public void AddPolyline(IEnumerable<Vector2> points, ColorF col, ImDrawFlags flags, float thickness) => throw new NotImplementedException();

    public void AddConvexPolyFilled(IEnumerable<Vector2> points, ColorF col)
    {
        List<Vector2> pointList = points.ToList();
        if (pointList.Count < 3 || col.A == 0) return;

        Vector2 uv = Data.TexUvWhitePixel;

        if (Flags.HasFlag(ImDrawListFlags.AntiAliasedFill))
        {
            // Anti-aliased Fill
            float aaSize = FringeScale;
            ColorF colTrans = col.WithAlpha(0);
            int indexCount = (pointList.Count - 2) * 3 + pointList.Count * 6;
            int vertexCount = pointList.Count * 2;
            PrimReserve(indexCount, vertexCount);

            // Add indices for fill
            uint vtxInnerIdx = VtxCurrentIdx;
            uint vtxOuterIdx = VtxCurrentIdx + 1;
            for (int i = 2; i < pointList.Count; i++)
            {
                IdxBuffer[IdxWriteIndex++] = (ushort) vtxInnerIdx;
                IdxBuffer[IdxWriteIndex++] = (ushort) (vtxInnerIdx + ((i - 1) << 1));
                IdxBuffer[IdxWriteIndex++] = (ushort) (vtxInnerIdx + (i << 1));
            }

            throw new NotImplementedException();
        }
        else
        {
            // Non Anti-aliased Fill
            int indexCount = (pointList.Count - 2) * 3;
            int vertexCount = pointList.Count;
            PrimReserve(indexCount, vertexCount);

            for (int i = 0; i < vertexCount; i++)
            {
                VtxBuffer[VtxWriteIndex].Position = pointList[i];
                VtxBuffer[VtxWriteIndex].Uv = uv;
                VtxBuffer[VtxWriteIndex].Color = col;
                VtxWriteIndex++;
            }
            
            for (int i = 2; i < pointList.Count; i++)
            {
                IdxBuffer[IdxWriteIndex++] = (ushort) VtxCurrentIdx;
                IdxBuffer[IdxWriteIndex++] = (ushort) (VtxCurrentIdx + i - 1);
                IdxBuffer[IdxWriteIndex++] = (ushort) (VtxCurrentIdx + i);
            }

            VtxCurrentIdx += (ushort) vertexCount;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PathClear()
    {
        Path.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PathLineTo(Vector2 pos)
    {
        Path.Add(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PathLineToMergeDuplicate(Vector2 pos)
    {
        if (!Path.Any() || Path.Last() != pos)
        {
            Path.Add(pos);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PathFillConvex(ColorF col)
    {
        AddConvexPolyFilled(Path, col);
        Path.Clear();
    }

    public void PathStroke(ColorF col, ImDrawFlags flags = ImDrawFlags.None, float thickness = 1)
    {
        AddPolyline(Path, col, flags, thickness);
        Path.Clear();
    }

    public void PathArcTo(Vector2 center, float radius, float aMin, float aMax, int numSegments = 0) => throw new NotImplementedException();
    public void PathArcToFast(Vector2 center, float radius, int aMinOf12, int aMaxOf12) => throw new NotImplementedException();

    public void PathEllipticalArcTo(Vector2 center, float radiusX, float radiusY, float rot, float aMin, float aMax,
        int numSegments = 0) => throw new NotImplementedException();

    public void PathRect(Vector2 min, Vector2 max, float rounding = 0, ImDrawFlags flags = ImDrawFlags.None) => throw new NotImplementedException();
    
    public void AddCallback(ImDrawCallback callback, object? callbackData) => throw new NotImplementedException();
    
    public void AddDrawCmd() => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ChannelsSplit(int count) => Splitter.Split(this, count);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ChannelsMerge() => Splitter.Merge(this);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ChannelsSetCurrent(int n) => Splitter.SetCurrentChannel(this, n);

    public void PrimReserve(int indexCount, int vertexCount) => throw new NotImplementedException();
    public void PrimUnreserve(int indexCount, int vertexCount) => throw new NotImplementedException();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrimWriteVtx(Vector2 pos, Vector2 uv, ColorF col)
    {
        VtxBuffer[VtxWriteIndex].Position = pos;
        VtxBuffer[VtxWriteIndex].Uv = uv;
        VtxBuffer[VtxWriteIndex].Color = col;
        VtxWriteIndex++;
        VtxCurrentIdx++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrimWriteIdx(ImDrawIdx idx)
    {
        IdxBuffer[IdxWriteIndex++] = idx;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PrimVtx(Vector2 pos, Vector2 uv, ColorF col)
    {
        PrimWriteIdx((ushort) VtxCurrentIdx);
        PrimWriteVtx(pos, uv, col);
    }

    internal void ResetForNewFrame() => throw new NotImplementedException();
    internal void ClearFreeMemory() => throw new NotImplementedException();
    internal void PopUnusedDrawCmd() => throw new NotImplementedException();
    internal void TryMergeDrawCmds() => throw new NotImplementedException();
}