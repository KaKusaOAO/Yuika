// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public static partial class ImGui
{
    #region -- Text
    
    [Obsolete("Use ImGui.Text(...)")]
    public static void TextUnformatted(string text) => Text(text);

    public static void Text(string text) => TextEx(text, ImGuiTextFlags.NoWidthForLargeClippedText);

    public static void TextColored(Color color, string text)
    {
        PushStyleColor(ImGuiCol.Text, color);
        Text(text);
        PopStyleColor();
    }

    public static void TextDisabled(string text)
    {
        ImGuiContext ctx = EnsureContext();
        PushStyleColor(ImGuiCol.Text, ctx.Style.Colors[ImGuiCol.TextDisabled]);    
        Text(text);
        PopStyleColor();
    }

    public static void TextWrapped(string text)
    {
        ImGuiContext ctx = EnsureContext();
        bool needBackup = ctx.CurrentWindow.DC.TextWrapPos < 0;
        if (needBackup)
        {
            PushTextWrapPos();
        }

        Text(text);

        if (needBackup)
        {
            PopTextWrapPos();
        }
    }
    
    public static void LabelText(string label) => throw new NotImplementedException();
    public static void BulletText(string text) => throw new NotImplementedException();
    public static void SeparatorText(string label) => throw new NotImplementedException();

    #endregion

    #region -- Main

    public static bool Button(string label, SizeF size = default) => throw new NotImplementedException();
    public static bool SmallButton(string label) => throw new NotImplementedException();
    public static bool InvisibleButton(string strId, SizeF size, ImGuiButtonFlags flags = ImGuiButtonFlags.None) 
        => throw new NotImplementedException();
    public static bool ArrowButton(string strId, ImGuiDir dir) => throw new NotImplementedException();
    public static unsafe bool CheckBox(string label, bool* v) => throw new NotImplementedException();
    public static unsafe bool CheckboxFlags(string label, int* flags, int flagsValue) 
        => throw new NotImplementedException();
    public static unsafe bool CheckboxFlags(string label, uint* flags, uint flagsValue) 
        => throw new NotImplementedException();
    public static bool RadioButton(string label, bool active) => throw new NotImplementedException();
    public static unsafe bool RadioButton(string label, int* v, int vButton) => throw new NotImplementedException();

    public static void ProgressBar(float fraction, SizeF? size = null, string? overlay = null)
    {
        size ??= new SizeF(-float.Epsilon, 0);
        throw new NotImplementedException();
    }

    public static void Bullet() => throw new NotImplementedException();

    #endregion

    #region -- Internal

    #region -- Widgets

    private static void TextEx(string text, ImGuiTextFlags flags)
    {
        ImGuiWindow window = CurrentWindow;
        if (window.SkipItems) return;

        ImGuiContext ctx = EnsureContext();
        
        Vector2 textPos = window.DC.CursorPos with {Y = window.DC.CursorPos.Y + window.DC.CurrLineTextBaseOffset};
        float wrapPosX = window.DC.TextWrapPos;
        bool wrapEnabled = wrapPosX >= 0;

        if (text.Length <= 2000 || wrapEnabled)
        {
            float wrapWidth = wrapEnabled ? CalcWrapWidthForPos(window.DC.CursorPos, wrapPosX) : 0;
            SizeF textSize = CalcTextSize(text, wrapWidth: wrapWidth);

            RectangleF bb = new RectangleF(textPos.AsPoint(), textPos.AsSize() + textSize);
            ItemSize(textSize, 0);
            if (!ItemAdd(bb, 0)) return;

            RenderTextWrapped(bb.Location.AsVector(), text, wrapWidth);
        }

        throw new NotImplementedException();
    }

    private static void ButtonEx(string label, SizeF? size = null, ImGuiButtonFlags flags = ImGuiButtonFlags.None)
    {
        size ??= SizeF.Empty;
        throw new NotImplementedException();
    }

    private static void ArrowButtonEx(string strId, ImGuiDir dir, SizeF size,
        ImGuiButtonFlags flags = ImGuiButtonFlags.None)
    {
        throw new NotImplementedException();
    }
    
    #endregion

    #endregion
}