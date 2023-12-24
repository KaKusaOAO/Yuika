namespace Yuika.YImGui;

public interface IImGuiContext
{
    
}

public class ImGuiStorage
{
    public class ImGuiStoragePair
    {
        public uint Key { get; set; }
        public int? ValueI { get; set; }
        public float? ValueF { get; set; }
        public object? ValueP { get; set; }
    }

    public List<ImGuiStoragePair> Data { get; set; } = new List<ImGuiStoragePair>();

    public void Clear()
    {
        Data.Clear();
    }

    public int GetInt(uint key, int defaultVal = 0);
    public void SetInt(uint key, int val);
    public bool GetBool(uint key, bool defaultVal = false);
    public void SetBool(uint key, bool val);
    public float GetFloat(uint key, float defaultVal = 0);
    public void SetFloat(uint key, float val);
    public object? GetObject(uint key);
    public void SetObject(uint key, object val);

    public void BuildSortByKey();
}