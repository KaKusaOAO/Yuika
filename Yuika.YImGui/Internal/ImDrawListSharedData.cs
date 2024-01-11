// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImDrawListSharedData
{
    public Vector2 TexUvWhitePixel { get; set; }
    public ImFont Font { get; set; }
    public float FontSize { get; set; }
    public float CurveTessellationTol { get; set; }
    public float CircleSegmentMaxError { get; set; }
    public Vector4 ClipRectFullscreen { get; set; }
    public ImDrawListFlags InitialFlags { get; set; }

    public List<Vector2> TempBuffer { get; set; } = new List<Vector2>();

    public Vector2[] ArcFastVtx { get; set; } = new Vector2[ImGui.ArcFastTableSize];
    public float ArcFastRadiusCutoff { get; set; }
    public byte[] CircleSegmentCounts { get; set; } = new byte[64];
    public List<Vector4> TexUvLines { get; set; } = new List<Vector4>();

    public void SetCircleTessellationMaxError(float maxError) => throw new NotImplementedException();
}
