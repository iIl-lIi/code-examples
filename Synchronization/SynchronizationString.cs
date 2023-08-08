using System;

namespace Synchronization
{
    [Serializable]
    public sealed class SynchronizationString 
    {
        public string Key;
        public string Value;
        public bool Useful { get; set; }
        public bool NeedSync { get; internal set; }
        public bool Sync { get; internal set; }

        public SynchronizationString(string name, string value, bool useful = false, bool needSync = false)
        {
            Key = name;
            Value = value;
            Useful = useful;
            NeedSync = needSync;
        }
    }
}