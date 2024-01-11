// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImGuiPayload
{
    public object? Data { get; set; }
    
    public uint SourceId { get; set; }
    public uint SourceParentId { get; set; }
    public int DataFrameCount { get; set; }
    public string DataType { get; set; } = string.Empty;
    public bool IsPreview { get; set; }
    public bool IsDelivery { get; set; }

    public ImGuiPayload()
    {
        Clear();
    }

    public void Clear()
    {
        SourceId = SourceParentId = 0;
        Data = null;
        DataType = string.Empty;
        DataFrameCount = -1;
        IsPreview = IsDelivery = false;
    }

    public bool IsDataType(string type)
    {
        return DataFrameCount != -1 && type == DataType;
    }
}