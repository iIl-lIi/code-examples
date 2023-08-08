using System;
using System.Collections.Generic;

namespace Synchronization
{
    [Serializable]
    public sealed class SynchronizationData
    {
        public const string SyncDataFolder = "ProjectSettings";

        public List<SynchronizationFile> Files;

        public SynchronizationData() { }
        public SynchronizationData(params SynchronizationFile[] strings) { Files = new (strings); }
    }
}