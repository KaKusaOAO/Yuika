// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public static partial class ImGui
{
    public static void ShowExampleAppMainMenuBar() {}
    public static unsafe void ShowExampleAppConsole(bool* open) {}
    public static unsafe void ShowExampleAppCustomRendering(bool* open) {}
    public static unsafe void ShowExampleAppDocuments(bool* open) {}
    public static unsafe void ShowExampleAppLog(bool* open) {}
    public static unsafe void ShowExampleAppLayout(bool* open) {}
    public static unsafe void ShowExampleAppPropertyEditor(bool* open) {}
    public static unsafe void ShowExampleAppSimpleOverlay(bool* open) {}
    public static unsafe void ShowExampleAppAutoResize(bool* open) {}
    public static unsafe void ShowExampleAppConstrainedResize(bool* open) {}
    public static unsafe void ShowExampleAppFullscreen(bool* open) {}
    public static unsafe void ShowExampleAppLongText(bool* open) {}
    public static unsafe void ShowExampleAppWindowTitles(bool* open) {}
    public static void ShowExampleMenuFile() {}

    private static void HelpMarker(string desc) 
    {
        TextDisabled("(?)");
        
        if (BeginItemTooltip())
        {
            PushTextWrapPos();
            throw new NotImplementedException();
            
            Text(desc);
            PopTextWrapPos();
            EndTooltip();
        }
    }
}