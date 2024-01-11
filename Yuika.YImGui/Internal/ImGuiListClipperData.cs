// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiListClipperData
{
    public ImGuiListClipper ListClipper { get; set; }
    public float LossynessOffset { get; set; }
    public int StepNo { get; set; }
    public int ItemsFrozen { get; set; }
    public List<ImGuiListClipperRange> Ranges { get; set; } = new List<ImGuiListClipperRange>();

    public void Reset(ImGuiListClipper clipper)
    {
        ListClipper = clipper;
        StepNo = ItemsFrozen = 0;
        Ranges.Clear();
    }
}