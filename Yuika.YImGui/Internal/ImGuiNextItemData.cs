// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiNextItemData
{
    public ImGuiNextItemDataFlags Flags { get; set; }
    public ImGuiItemFlags ItemFlags { get; set; }
    public float Width { get; set; }
    public long SelectionUserData { get; set; }
    public ImGuiCond OpenCond { get; set; }
    public bool OpenVal { get; set; }

    public void ClearFlags()
    {
        Flags = ImGuiNextItemDataFlags.None;
        ItemFlags = ImGuiItemFlags.None;
    }
}

internal class ImGuiListClipper
{
    public ImGuiContext? Context { get; set; }
    public int DisplayStart { get; set; }
    public int DisplayEnd { get; set; }
    public int ItemsCount { get; set; }
    public float ItemsHeight { get; set; }
    public float StartPosY { get; set; }
    public object? TempData { get; set; }

    public void Begin(int itemsCount, float itemsHeight = -1)
    {
        if (Context == null)
        {
            Context = ImGui.EnsureContext();
        }

        ImGuiContext ctx = Context;
        ImGuiWindow window = ctx.CurrentWindow!;
        ImGui.DebugLog("Clipper: Begin({0},{1:f2}) in '{2}'", itemsCount, itemsHeight, window.Name);

        {
            ImGuiTable? table = ctx.CurrentTable;
            if (table != null && table.IsInsideRow)
            {
                ImGui.TableEndRow(table);
            }
        }

        StartPosY = window.DC.CursorPos.Y;
        ItemsHeight = itemsHeight;
        ItemsCount = itemsCount;
        DisplayStart = -1;
        DisplayEnd = 0;

        if (++ctx.ClipperTempDataStacked > ctx.ClipperTempData.Count)
        {
            int targetSize = ctx.ClipperTempDataStacked;
            for (int i = ctx.ClipperTempData.Count; i < targetSize; i++)
            {
                ctx.ClipperTempData.Add(new ImGuiListClipperData());
            }
        }

        ImGuiListClipperData data = ctx.ClipperTempData.Last(); // [Context.ClipperTempDataStacked - 1];
        data.Reset(this);
        data.LossynessOffset = window.DC.CursorStartPosLossyness.Y;
        TempData = data;
    }

    public void End()
    {
        ImGuiListClipperData? data = (ImGuiListClipperData?) TempData;
        if (data != null)
        {
            ImGuiContext ctx = Context!;
            ImGui.DebugLog("Clipper: End() in '{0}'", ctx.CurrentWindow!.Name);

            if (ItemsCount >= 0 && ItemsCount < int.MaxValue && DisplayStart >= 0)
            {
                SeekCursorForItem(ItemsCount);
            }
            
            Debug.Assert(data.ListClipper == this);
            data.StepNo = data.Ranges.Count;
            if (--ctx.ClipperTempDataStacked > 0)
            {
                data = ctx.ClipperTempData.Last();
                data.ListClipper.TempData = data;
            }

            TempData = null;
        }

        ItemsCount = -1;
    }

    private void SeekCursorForItem(int itemN)
    {
        ImGuiListClipperData data = (ImGuiListClipperData) TempData!;
        float posY = StartPosY + data.LossynessOffset + (itemN - data.ItemsFrozen) * ItemsHeight;
        SeekCursorAndSetupPrevLine(posY, ItemsHeight);
    }

    private static void SeekCursorAndSetupPrevLine(float posY, float lineHeight)
    {
        ImGuiContext ctx = ImGui.EnsureContext();
        ImGuiWindow window = ctx.CurrentWindow!;
        
        float offY = posY - window.DC.CursorPos.Y;
        window.DC.CursorPos.Y = posY;
        window.DC.CursorMaxPos.Y = Math.Max(window.DC.CursorMaxPos.Y, posY - ctx.Style.ItemSpacing.Y);
        window.DC.PrevLineSize.Y = lineHeight - ctx.Style.ItemSpacing.Y;

        {
            ImGuiOldColumns? columns = window.DC.CurrentColumns;
            if (columns != null)
            {
                columns.LineMinY = window.DC.CursorPos.Y;
            }
        }

        {
            ImGuiTable? table = ctx.CurrentTable;
            if (table != null)
            {
                if (table.IsInsideRow)
                {
                    ImGui.TableEndRow(table);
                }

                table.RowPosY2 = window.DC.CursorPos.Y;

                int rowIncrease = (int) ((offY / lineHeight) + 0.5f);
                table.RowBgColorCounter += rowIncrease;
            }
        }
    }

    public bool Step()
    {
        ImGuiContext ctx = ImGui.EnsureContext();
        bool needItemsHeight = ItemsHeight <= 0;
        bool result = StepInternal();

        if (result && DisplayStart == DisplayEnd) result = false;
        
        if (ctx.CurrentTable?.IsUnfrozenRows == false) 
            ImGui.DebugLog("Clipper: Step(): inside frozen table row.");
        if (needItemsHeight && ItemsHeight > 0)
            ImGui.DebugLog($"Clipper: Step(): computed ItemsHeight: {ItemsHeight:f2}");

        if (result)
        {
            ImGui.DebugLog($"Clipper: Step(): display {DisplayStart} to {DisplayEnd}.");
        }
        else
        {
            ImGui.DebugLog("Clipper: Step(): End.");
            End();
        }

        return result;
    }

    private bool StepInternal()
    {
        ImGuiContext ctx = Context!;
        ImGuiWindow window = ctx.CurrentWindow!;
        
        ImGuiListClipperData? data = (ImGuiListClipperData?) TempData;
        if (data == null)
        {
            throw new ImGuiException("Called ImGuiListClipper.Step() too many times, or before ImGuiListClipper.Begin() ?");
        }

        ImGuiTable? table = ctx.CurrentTable;
        if (table != null && table.IsInsideRow)
        {
            ImGui.TableEndRow(table);
        }
        
        // No items
        if (ItemsCount == 0 || GetSkipItemForListClipping())
        {
            return false;
        }

        if (data.StepNo == 0 && table != null && !table.IsUnfrozenRows)
        {
            DisplayStart = data.ItemsFrozen;
            DisplayEnd = Math.Min(data.ItemsFrozen + 1, ItemsCount);

            if (DisplayStart < DisplayEnd)
            {
                data.ItemsFrozen++;
            }

            return true;
        }

        bool calcClipping = false;
        if (data.StepNo == 0)
        {
            StartPosY = window.DC.CursorPos.Y;
            if (ItemsHeight <= 0)
            {
                data.Ranges.Add(ImGuiListClipperRange.FromIndices(data.ItemsFrozen, data.ItemsFrozen + 1));
                DisplayStart = Math.Max(data.Ranges[0].Min, data.ItemsFrozen);
                DisplayEnd = Math.Min(data.Ranges[0].Max, ItemsCount);
                data.StepNo = 1;
                return true;
            }

            calcClipping = true;
        }

        if (ItemsHeight <= 0)
        {
            Debug.Assert(data.StepNo == 1);

            if (table != null)
            {
                Debug.Assert(table.RowPosY1 == StartPosY && table.RowPosY2 == window.DC.CursorPos.Y);
            }

            ItemsHeight = (window.DC.CursorPos.Y - StartPosY) / (DisplayEnd - DisplayStart);
            bool affectedByFloatingPointPrecision = ImGui.IsFloatAboveGuaranteedIntegerPrecision(StartPosY) ||
                                                    ImGui.IsFloatAboveGuaranteedIntegerPrecision(window.DC.CursorPos.Y);
            if (affectedByFloatingPointPrecision)
            {
                ItemsHeight = window.DC.PrevLineSize.Y + ctx.Style.ItemSpacing.Y;
            }
            
            Debug.Assert(ItemsHeight > 0,
                "Unable to calculate item height! First item hasn't moved the cursor vertically!");

            calcClipping = true;
        }

        int alreadySubmitted = DisplayEnd;
        if (calcClipping)
        {
            if (ctx.LogEnabled)
            {
                data.Ranges.Add(ImGuiListClipperRange.FromIndices(0, ItemsCount));
            }
            else
            {
                bool isNavRequest = ctx.NavMoveScoringItems &&
                                    ctx.NavWindow?.RootWindowForNav == window.RootWindowForNav;
                
                if (isNavRequest)
                {
                    data.Ranges.Add(ImGuiListClipperRange.FromPositions(ctx.NavScoringNoClipRect.Location.Y,
                        ctx.NavScoringNoClipRect.LowerRight().Y, 0, 0));

                    if (ctx.NavMoveFlags.HasFlag(ImGuiNavMoveFlags.IsTabbing) && ctx.NavTabbingDir == -1)
                    {
                        data.Ranges.Add(ImGuiListClipperRange.FromIndices(ItemsCount - 1, ItemsCount));
                    }
                }
                
                // Add focused/active item
                RectangleF navRectAbs = ImGui.WindowRectRelToAbs(window, window.NavRectRel[0]);
                if (ctx.NavId != 0 && window.NavLastIds[0] == ctx.NavId)
                {
                    data.Ranges.Add(ImGuiListClipperRange.FromPositions(navRectAbs.Location.Y,
                        navRectAbs.LowerRight().Y, 0, 0));
                }
                
                // Add visible range
                int offMin = isNavRequest && ctx.NavMoveClipDir == ImGuiDir.Up ? -1 : 0;
                int offMax = isNavRequest && ctx.NavMoveClipDir == ImGuiDir.Down ? 1 : 0;
                data.Ranges.Add(ImGuiListClipperRange.FromPositions(window.ClipRect.Location.Y,
                    window.ClipRect.LowerRight().Y, offMin, offMax));
            }

            foreach (ImGuiListClipperRange range in data.Ranges.Where(r => r.PosToIndexConvert))
            {
                int m1 = (int) ((range.Min - window.DC.CursorPos.Y - data.LossynessOffset) / ItemsHeight);
                int m2 = (int) ((range.Max - window.DC.CursorPos.Y - data.LossynessOffset) / ItemsHeight + 0.999999f);
                    
                range.Min = Math.Clamp(alreadySubmitted + m1 + range.PosToIndexOffsetMin, alreadySubmitted,
                    ItemsCount - 1);
                range.Max = Math.Clamp(alreadySubmitted + m2 + range.PosToIndexOffsetMax, range.Min + 1,
                    ItemsCount);
                range.PosToIndexConvert = false;
            }
            
            SortAndFuseRanges(data.Ranges, data.StepNo);
        }

        while (data.StepNo < data.Ranges.Count)
        {
            DisplayStart = Math.Max(data.Ranges[data.StepNo].Min, alreadySubmitted);
            DisplayEnd = Math.Min(data.Ranges[data.StepNo].Max, ItemsCount);

            if (DisplayStart > alreadySubmitted)
            {
                SeekCursorForItem(DisplayStart);
            }

            data.StepNo++;
            if (DisplayStart == DisplayEnd && data.StepNo < data.Ranges.Count) continue;
            return true;
        }

        if (ItemsCount < int.MaxValue)
        {
            SeekCursorForItem(ItemsCount);
        }

        return false;
    }

    private static void SortAndFuseRanges(List<ImGuiListClipperRange> ranges, int offset = 0)
    {
        if (ranges.Count - offset <= 1) return;

        // ranges.Sort((a, b) => a.Min - b.Min);
        
        for (int sortEnd = ranges.Count - offset - 1; sortEnd > 0; --sortEnd)
        {
            for (int i = offset; i < sortEnd + offset; ++i)
            {
                if (ranges[i].Min > ranges[i + 1].Min)
                {
                    (ranges[i], ranges[i + 1]) = (ranges[i + 1], ranges[i]);
                }
            }
        }
        
        // Now fuse ranges together as much as possible.
        for (int i = 1 + offset; i < ranges.Count; i++)
        {
            Debug.Assert(!ranges[i].PosToIndexConvert && !ranges[i - 1].PosToIndexConvert);
            if (ranges[i - 1].Max < ranges[i].Min) continue;

            ranges[i - 1].Min = Math.Min(ranges[i - 1].Min, ranges[i].Min);
            ranges[i - 1].Max = Math.Max(ranges[i - 1].Max, ranges[i].Max);
            ranges.RemoveAt(i);
            i--;
        }
    }
    
    public void IncludeItemByIndex(int itemIndex) => IncludeItemByIndex(itemIndex, itemIndex + 1);

    public void IncludeItemByIndex(int itemBegin, int itemEnd)
    {
        ImGuiListClipperData data = (ImGuiListClipperData) TempData!;
        
        Debug.Assert(DisplayStart < 0);
        Debug.Assert(itemBegin <= itemEnd);

        if (itemBegin < itemEnd)
        {
            data.Ranges.Add(ImGuiListClipperRange.FromIndices(itemBegin, itemEnd));
        }
    }

    private static bool GetSkipItemForListClipping()
    {
        ImGuiContext ctx = ImGui.EnsureContext();
        return ctx.CurrentTable?.HostSkipItems ?? ctx.CurrentWindow!.SkipItems;
    }
}