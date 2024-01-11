// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImDrawListSplitter
{
    public int Current { get; set; }
    public int Count { get; set; }
    public List<ImDrawChannel> Channels { get; set; } = new List<ImDrawChannel>();

    public void Clear();
    public void ClearFreeMemory();
    public void Split();
    public void Merge();
    public void SetCurrentChannel();
}