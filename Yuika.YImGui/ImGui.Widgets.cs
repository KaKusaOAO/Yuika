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
    
    public static void LabelText(string label);
    public static void BulletText(string text);
    public static void SeparatorText(string label);

    #endregion

    #region -- Main

    public static bool Button(string label, SizeF size = default);
    public static bool SmallButton(string label);
    public static bool InvisibleButton(string strId, SizeF size, ImGuiButtonFlags flags = ImGuiButtonFlags.None);
    public static bool ArrowButton(string strId, ImGuiDir dir);
    public static unsafe bool CheckBox(string label, bool* v);
    public static unsafe bool CheckboxFlags(string label, int* flags, int flagsValue);
    public static unsafe bool CheckboxFlags(string label, uint* flags, uint flagsValue);
    public static bool RadioButton(string label, bool active);
    public static unsafe bool RadioButton(string label, int* v, int vButton);

    public static void ProgressBar(float fraction, SizeF? size = null, string? overlay = null)
    {
        size ??= new SizeF(-float.Epsilon, 0);
        throw new NotImplementedException();
    }

    public static void Bullet();

    #endregion
    
    
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
            SizeF textSize = CalcTextSize(text, false, wrapWidth);

            RectangleF bb = new RectangleF(new PointF(textPos), new SizeF(textPos) + textSize);
            ItemSize(ref textSize, 0);
            if (!ItemAdd(bb, 0)) return;

            RenderTextWrapped(bb.Location, text, wrapWidth);
        }

        throw new NotImplementedException();
    }
}