﻿namespace Yuika.YImGui;

public class ImGuiInputTextCallbackData
{
    public IImGuiContext Context { get; set; }
    public ImGuiInputTextFlags EventFlag { get; set; }
    public ImGuiInputTextFlags Flags { get; set; }
    public object? UserData { get; set; }
    
    public char EventChar { get; set; }
    public ImGuiKey EventKey { get; set; }
    public string Text { get; set; }
    public bool TextDirty { get; set; } // ?
    public int CursorPos { get; set; }
    public int SelectionStart { get; set; }
    public int SelectionEnd { get; set; }

    public void DeleteChars(int pos, int count);
    public void InsertChars(int pos, string text);

    public void SelectAll()
    {
        SelectionStart = 0;
        SelectionEnd = Text.Length;
    }
    
    public void ClearSelection() => SelectionStart = SelectionEnd = Text.Length;
    public bool HasSelection => SelectionStart != SelectionEnd;
}