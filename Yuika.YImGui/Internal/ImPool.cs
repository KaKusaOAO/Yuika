// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Collections.Concurrent;
using System.Diagnostics;

namespace Yuika.YImGui.Internal;

public class ImPool<T> where T : new()
{
    public T[] Buf = Array.Empty<T>();
    public int[] FreeIndices = Array.Empty<int>();
    // public ConcurrentDictionary<uint, int> Map { get; set; } = new ConcurrentDictionary<uint, int>();
    public ImGuiStorage Map { get; set; } = new ImGuiStorage();
    public int FreeIdx { get; set; }
    public int AliveCount { get; set; }

    public int BufSize => Buf.Length;
    public int MapSize => Map.Data.Count;

    public T? TryGetMapData(int n)
    {
        int idx = Map.Data[n].ValueI;
        if (idx == -1) return default;
        return GetByIndex(n);
    }

    public void Clear()
    {
        Map.Clear();
        Array.Clear(Buf, 0, Buf.Length);
        Array.Clear(FreeIndices, 0, FreeIndices.Length);
    }

    public T? GetByKey(uint key)
    {
        int idx = Map.GetInt(key, -1); // Map.GetValueOrDefault(key, -1);
        return idx != -1 ? Buf[idx] : default;
    }

    public T GetByIndex(int idx) => Buf[idx];

    public int GetIndex(T p) => Array.IndexOf(Buf, p);

    public T GetOrAddByKey(uint key)
    {
        ref int idx = ref Map.GetIntRef(key, -1); // Map.GetValueOrDefault(key, -1);
        if (idx != -1)
        {
            return Buf[idx];
        }

        idx = FreeIdx;
        return Add();
    }

    public T Add()
    {
        int idx = FreeIdx;
        if (idx == Buf.Length)
        {
            Array.Resize(ref Buf, Buf.Length + 1);
            Array.Resize(ref FreeIndices, FreeIndices.Length + 1);
            FreeIdx++;
        }
        else
        {
            FreeIdx = FreeIndices[idx];
        }
        
        T result = new T();
        Buf[idx] = result;
        
        AliveCount++;
        return result;
    }

    public void Remove(uint key, T p) => Remove(key, GetIndex(p));

    public void Remove(uint key, int idx)
    {
        FreeIndices[idx] = FreeIdx;
        FreeIdx = idx;
        Map.SetInt(key, -1);
        AliveCount--;
    }
}