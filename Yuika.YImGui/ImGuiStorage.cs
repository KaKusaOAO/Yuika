using System.Runtime.CompilerServices;

namespace Yuika.YImGui;

public class ImGuiStorage
{
    public class ImGuiStoragePair
    {
        public uint Key;
        public int ValueI;
        public float ValueF;
        public object? ValueP { get; set; }
    }

    public List<ImGuiStoragePair> Data { get; set; } = new List<ImGuiStoragePair>();

    public void Clear()
    {
        Data.Clear();
    }

    public int GetInt(uint key, int defaultVal = 0) => Data
        .Where(d => d.Key == key)
        .Select(d => d.ValueI)
        .FirstOrDefault();

    public void SetInt(uint key, int val)
    {
        ImGuiStoragePair? data = Data.FirstOrDefault(d => d.Key == key);
        if (data != null)
        {
            data.ValueI = val;
            return;
        }

        data = new ImGuiStoragePair
        {
            Key = key,
            ValueI = val
        };

        Data.Add(data);
    }

    public bool GetBool(uint key, bool defaultVal = false) => GetInt(key, defaultVal ? 1 : 0) != 0;

    public void SetBool(uint key, bool val) => SetInt(key, val ? 1 : 0);
    
    public float GetFloat(uint key, float defaultVal = 0) => Data
        .Where(d => d.Key == key)
        .Select(d => d.ValueF)
        .FirstOrDefault();

    public void SetFloat(uint key, float val)
    {
        ImGuiStoragePair? data = Data.FirstOrDefault(d => d.Key == key);
        if (data != null)
        {
            data.ValueF = val;
            return;
        }

        data = new ImGuiStoragePair
        {
            Key = key,
            ValueF = val
        };

        Data.Add(data);
    }
    
    public object? GetObject(uint key) => Data
        .Where(d => d.Key == key)
        .Select(d => d.ValueP)
        .FirstOrDefault();

    public void SetObject(uint key, object val) 
    {
        ImGuiStoragePair? data = Data.FirstOrDefault(d => d.Key == key);
        if (data != null)
        {
            data.ValueP = val;
            return;
        }

        data = new ImGuiStoragePair
        {
            Key = key,
            ValueP = val
        };

        Data.Add(data);
    }

    public T GetObject<T>(uint key) => (T) GetObject(key)!;

    public unsafe ref int GetIntRef(uint key, int defaultVal = 0)
    {
        ImGuiStoragePair? data = Data.FirstOrDefault(d => d.Key == key);
        if (data == null)
        {
            data = new ImGuiStoragePair
            {
                Key = key,
                ValueI = defaultVal
            };
            
            Data.Add(data);
        }
        
        fixed (int* p = &data.ValueI)
        {
            return ref Unsafe.AsRef<int>(p);
        }
    }

    public void BuildSortByKey();
}