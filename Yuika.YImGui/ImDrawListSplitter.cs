// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImDrawListSplitter
{
    public int Current { get; set; }
    public int Count { get; set; }
    public List<ImDrawChannel> Channels { get; set; } = new List<ImDrawChannel>();

    public void Clear() => throw new NotImplementedException();
    public void ClearFreeMemory() => throw new NotImplementedException();
    public void Split(ImDrawList drawList, int channelsCount) => throw new NotImplementedException();
    public void Merge(ImDrawList drawList) => throw new NotImplementedException();
    public void SetCurrentChannel(ImDrawList drawList, int index) => throw new NotImplementedException();
}