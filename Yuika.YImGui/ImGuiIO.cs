// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Yuika.YImGui.Internal;

namespace Yuika.YImGui;

public class ImGuiIO
{
    public const int MouseButtonCount = 5;
    
    public ImGuiConfigFlags ConfigFlags { get; set; } = ImGuiConfigFlags.None;
    public ImGuiBackendFlags BackendFlags { get; set; } = ImGuiBackendFlags.None;
    public SizeF DisplaySize { get; set; } = new SizeF(-1, -1);
    public float DeltaTime { get; set; } = 1f / 60;
    public float IniSavingRate { get; set; } = 5;
    public string IniFilename { get; set; } = "imgui.ini";
    public string LogFilename { get; set; } = "imgui_log.txt";
    public object? UserData { get; set; }
    
    public ImFontAtlas? Fonts { get; set; }
    public float FontGlobalScale { get; set; } = 1;
    public bool FontAllowUserScaling { get; set; }
    public ImFont? FontDefault { get; set; }
    public Vector2 DisplayFramebufferScale { get; set; } = Vector2.One;
    
#if USE_DOCKING
    
    // Docking options
    
    public bool ConfigDockingNoSplit { get; set; }
    public bool ConfigDockingWithShift { get; set; }
    public bool ConfigDockingAlwaysTabBar { get; set; }
    public bool ConfigDockingTransparentPayload { get; set; }
    
    // Viewport options
    
    public bool ConfigViewportsNoAutoMerge { get; set; } 
    public bool ConfigViewportsNoTaskBarIcon { get; set; }
    public bool ConfigViewportsNoDecoration { get; set; } = true;
    public bool ConfigViewportsNoDefaultParent { get; set; } 
    
#endif
    
    // Miscellaneous options
    
    public bool MouseDrawCursor { get; set; }

#if NET6_0
    [SupportedOSPlatformGuard("macos")]
    [SupportedOSPlatformGuard("tvos")]
    [SupportedOSPlatformGuard("ios")]
    public bool ConfigMacOSBehaviors { get; set; } = 
        OperatingSystem.IsMacOS() || OperatingSystem.IsTvOS() || OperatingSystem.IsIOS();
#else
    public bool ConfigMacOSBehaviors { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
    
    public bool ConfigInputTrickleEventQueue { get; set; } = true;
    public bool ConfigInputTextCursorBlink { get; set; } = true;
    public bool ConfigInputTextEnterKeepActive { get; set; }
    public bool ConfigDragClickToInputText { get; set; }
    public bool ConfigWindowsResizeFromEdges { get; set; } = true;
    public bool ConfigWindowsMoveFromTitleBarOnly { get; set; }
    public float ConfigMemoryCompactTImer { get; set; } = 60;

    public float MouseDoubleClickTime { get; set; } = 0.3f;
    public float MouseDoubleClickMaxDist { get; set; } = 6;
    public float MouseDragThreshold { get; set; } = 6;
    public float KeyRepeatDelay { get; set; } = 0.275f;
    public float KeyRepeatRate { get; set; } = 0.05f;
    
    public bool ConfigDebugBeginReturnValueOnce { get; set; }
    public bool ConfigDebugBeginReturnValueLoop { get; set; }
    public bool ConfigDebugIgnoreFocusLoss { get; set; }
    public bool ConfigDebugIniSettings { get; set; }
    
    public string? BackendPlatformName { get; set; }
    public string? BackendRendererName { get; set; }
    public object? BackendPlatformUserData { get; set; }
    public object? BackendRendererUserData { get; set; }
    // public object? BackendLanguageUserData { get; set; }

    public Func<object, string>? GetClipboardTextFn { get; set; }
    public Action<object, string>? SetClipboardTextFn { get; set; }
    public object? ClipboardUserData { get; set; }
    
    public Action<ImGuiViewport, ImGuiPlatformImeData> SetPlatformImeDataFn { get; set; }
    public char PlatformLocaleDecimalPoint { get; set; } = '.';

    public void AddKeyEvent(ImGuiKey key, bool down)
    {
        if (!AppAcceptingEvents) return;
        AddKeyAnalogEvent(key, down, down ? 1 : 0);
    }

    public void AddKeyAnalogEvent(ImGuiKey key, bool down, float v)
    {
        Debug.Assert(Ctx != null);

        if (key == ImGuiKey.None || !AppAcceptingEvents)
            return;
        
        Debug.Assert(key != (ImGuiKey) ImGuiKeyMod.Shortcut);
        
        
    }
    public void AddMousePosEvent(float x, float y) => throw new NotImplementedException();
    public void AddMouseButtonEvent(int button, bool down) => throw new NotImplementedException();
    public void AddMouseWheelEvent(float wheelX, float wheelY) => throw new NotImplementedException();
    public void AddMouseSourceEvent(ImGuiMouseSource source) => throw new NotImplementedException();
    public void AddFocusEvent(bool focused) => throw new NotImplementedException();

    public void AddInputCharacter(int codepoint)
    {
        Debug.Assert(Ctx != null);
        
        if (codepoint == 0 || !AppAcceptingEvents)
            return;

        ImGuiInputEventText e = new ImGuiInputEventText();
        e.Type = ImGuiInputEventType.Text;
        e.Source = ImGuiInputSource.Keyboard;
        e.EventId = Ctx.InputEventsNextEventId++;
        e.Character = codepoint;
        
        Ctx.InputEventsQueue.Add(e);
    }

    public void AddInputCharacterUTF16(char c)
    {
        if ((c == 0 && InputQueueSurrogate == 0) || !AppAcceptingEvents)
            return;

        if (char.IsHighSurrogate(c))
        {
            if (InputQueueSurrogate != 0)
            {
                AddInputCharacter(0xfffd); // Invalid
            }

            InputQueueSurrogate = c;
            return;
        }

        int cp = c;
        if (InputQueueSurrogate != 0)
        {
            if (!char.IsLowSurrogate(c))
            {
                AddInputCharacter(0xfffd); // Invalid
            }
            else
            {
                // cp = ((InputQueueSurrogate - 0xd800) << 10) + (c - 0xdc00) + 0x10000;
                cp = char.ConvertToUtf32(InputQueueSurrogate, c);
            }

            InputQueueSurrogate = (char) 0;
        }
        
        AddInputCharacter(cp);
    }

    public void AddInputCharacters(string str)
    {
        if (!AppAcceptingEvents) return;

        foreach (int cp in Encoding.UTF32.GetBytes(str)
                     .Chunk(4)
                     .Select(arr => BitConverter.IsLittleEndian ? arr : arr.Reverse())
                     .Select(arr => BitConverter.ToInt32(arr.ToArray())))
        {
            AddInputCharacter(cp);
        }
    }

    public void AddInputCharacters(byte[] utf8Str) => AddInputCharacters(Encoding.UTF8.GetString(utf8Str));

    public bool WantCaptureMouse { get; internal set; }
    public bool WantCaptureKeyboard { get; internal set; }
    public bool WantTextInput { get; internal set; }
    public bool WantSetMousePos { get; internal set; }
    public bool WantSaveIniSettings { get; internal set; }
    public bool NavActive { get; internal set; }
    public bool NavVisible { get; internal set; }
    public float Framerate { get; internal set; }
    public int MetricsRenderVertices { get; internal set; }
    public int MetricsRenderIndices { get; internal set; }
    public int MetricsRenderWindows { get; internal set; }
    public int MetricsActiveWindows { get; internal set; }
    public Vector2 MouseDelta { get; internal set; }
    
    internal ImGuiContext Ctx { get; set; }

    public Vector2 MousePos { get; set; } = new Vector2(float.MinValue, float.MinValue);
    public bool[] MouseDown { get; set; } = new bool[MouseButtonCount];
    public float MouseWheel { get; set; }
    public float MouseWheelH { get; set; }
    public ImGuiMouseSource MouseSource { get; set; } = ImGuiMouseSource.Mouse;
    public bool KeyCtrl { get; set; }
    public bool KeyShift { get; set; }
    public bool KeyAlt { get; set; }
    public bool KeySuper { get; set; }
    
    public ImGuiKeyChord KeyMods { get; set; }
    public ImGuiKeyData[] KeysData { get; set; } = new ImGuiKeyData[ImGui.KeysDataSize];
    
    public bool WantCaptureMouseUnlessPopupClose { get; set; }
    public Vector2 MousePosPrev { get; set; } = new Vector2(float.MinValue, float.MinValue);
    public Vector2[] MouseClickedPos { get; set; } = new Vector2[MouseButtonCount];
    public double[] MouseClickedTime { get; set; } = new double[MouseButtonCount];
    public bool[] MouseClicked { get; set; } = new bool[MouseButtonCount];
    public bool[] MouseDoubleClicked { get; set; } = new bool[MouseButtonCount];
    public ushort[] MouseClickedCount { get; set; } = new ushort[MouseButtonCount];
    public ushort[] MouseClickedLastCount { get; set; } = new ushort[MouseButtonCount];
    public bool[] MouseReleased { get; set; } = new bool[MouseButtonCount];
    public bool[] MouseDownOwned { get; set; } = new bool[MouseButtonCount];
    public bool[] MouseDownOwnedUnlessPopupClose { get; set; } = new bool[MouseButtonCount];
    public bool MouseWheelRequestAxisSwap { get; set; }
    public float[] MouseDownDuration { get; set; } = new float[MouseButtonCount];
    public float[] MouseDownDurationPrev { get; set; } = new float[MouseButtonCount];
    public float[] MouseDragMaxDistanceSqr { get; set; } = new float[MouseButtonCount];
    public float PenPressure { get; set; }
    public bool AppFocusLost { get; set; }
    public bool AppAcceptingEvents { get; set; }
    public sbyte BackendUsingLegacyKeyArrays { get; set; }
    public bool BackendUsingLegacyNavInputArray { get; set; }
    public char InputQueueSurrogate { get; set; }
    public List<char> InputQueueCharacters { get; set; } = new List<char>();

    public ImGuiIO()
    {
        for (var i = MouseDownDuration.Length - 1; i >= 0; i--)
        {
            MouseDownDuration[i] = MouseDownDurationPrev[i] = -1;
        }
        
        for (var i = KeysData.Length - 1; i >= 0; i--)
        {
            KeysData[i] = new ImGuiKeyData();
            KeysData[i].DownDuration = KeysData[i].DownDurationPrev = -1;
        }
    }
}