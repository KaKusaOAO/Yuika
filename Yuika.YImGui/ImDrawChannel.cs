// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImDrawChannel
{
    public List<ImDrawCmd> CmdBuffer { get; set; } = new List<ImDrawCmd>();
    public List<ImDrawIdx> IdxBuffer { get; set; } = new List<ImDrawIdx>();   
}