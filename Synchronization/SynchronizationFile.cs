using System;
using System.Collections.Generic;

namespace Synchronization
{
    [Serializable]
    public sealed class SynchronizationFile
    {
        public string Name;
        public List<SynchronizationString> Strings;
        public bool Useful { get; set; }
        public bool Newest { get; set; }

        public SynchronizationFile(string name, bool newest, params SynchronizationString[] strings)
        {
            Name = name;
            Newest = newest;
            Strings = new (strings);
        }
    }
}