using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Synchronization.SynchronizationData;

namespace Synchronization
{
    public sealed class Synchronizer
    {
        private readonly string _folder;
        private readonly string _syncStringPrefix;
        private readonly string _syncDataFilePath;
        
        private readonly Dictionary<string, List<string>> _foundFiles  = new ();
        private readonly List<SynchronizationFile>        _syncFiles   = new ();
        private readonly List<string>                     _findFolders = new ();

        public Synchronizer(string folderName, string stringPrefix, string syncDataFileName)
        {
            _folder = folderName;
            _syncStringPrefix = stringPrefix;
            _syncDataFilePath = $"{SyncDataFolder}/{syncDataFileName}.json";
            LoadDataJson();   
        }
        ~Synchronizer()
        {
            RemoveUselessData();
            SaveDataJson();
        }

        public Synchronizer Synchronize(string fileName, params SynchronizationString[] strings)
        {
            FindFolders();
            FindAllFiles(fileName);

            var syncFile = GetSyncFile(fileName, strings);
            PreSynchronize(fileName, strings);

            foreach (var filePath in _foundFiles[fileName])
            {
                var fileLines = File.ReadAllLines(filePath);
                foreach (var syncDef in strings)
                {
                    var syncFileDef = GetSyncFileStr(syncDef, syncFile);
                    if (!syncFileDef.Sync && !syncFileDef.NeedSync) continue; 
                    syncFileDef.Value = syncDef.Value;
                    syncFileDef.Sync = true;

                    for (var i = 0; i < fileLines.Length; i++)
                    {
                        if (!fileLines[i].Contains($"{_syncStringPrefix}{syncFileDef.Key}")) continue;
                        fileLines[i] = $"{_syncStringPrefix}{syncFileDef.Key} {syncFileDef.Value}";
                        DebugSync(fileName, filePath, syncFileDef.Key, syncFileDef.Value);
                        break;
                    }
                }
                File.WriteAllLines(filePath, fileLines);
            }
            return this;
        }

        private void PreSynchronize(string fileName, SynchronizationString[] strs)
        {
            foreach (var file in _syncFiles) 
            {
                if (file.Name != fileName) continue;
                file.Useful = true;

                foreach (var str in strs)
                {
                    var syncFileDef = GetSyncFileStr(str, file);
                    syncFileDef.Useful = true;
                    if (syncFileDef.Value == str.Value) continue;
                    syncFileDef.NeedSync = true;
                    break;
                }
                break;
            }
        }
        private void FindFolders()
        {
            if (_findFolders.Count > 0) return;

            var foldersStack = new Stack<string>();
            foldersStack.Push(Application.dataPath);

            while (foldersStack.Count > 0)
            {
                var subfolders = Directory.GetDirectories(foldersStack.Pop());
                foreach (var subfolder in subfolders)
                {
                    if (subfolder.Split(_folder)[^1] == string.Empty)
                        _findFolders.Add(subfolder);
                    foldersStack.Push(subfolder);
                }
            }

            if (_findFolders.Count > 0) return; 
            Debug.Log("[Synchronizer]: <color=red>'ShaderLibrary' folders not found.</color>");
        }
        private static List<string> FindFilesInFolder(string folder, string fileName)
        {
            var filesList = new List<string>();
            var foldersStack = new Stack<string>();
            foldersStack.Push(folder);

            while (foldersStack.Count > 0)
            {
                var currentFolder = foldersStack.Pop();
                var files = Directory.GetFiles(currentFolder, fileName, SearchOption.TopDirectoryOnly);
                filesList.AddRange(files);

                var subfolders = Directory.GetDirectories(currentFolder);
                foreach (string subfolder in subfolders)
                    foldersStack.Push(subfolder);
            }

            if (filesList.Count == 0)
                Debug.LogError("[Synchronizer]: <color=red>Files not found.</color>");

            return filesList;
        }   
        private void FindAllFiles(string fileName)
        {
            if (!_foundFiles.ContainsKey(fileName))
            {
                var allFiles = new List<string>();
                foreach (var shaderLibraryFolder in _findFolders)
                {
                    var folderFiles = FindFilesInFolder(shaderLibraryFolder, fileName);
                    foreach (var file in folderFiles)
                    {
                        if (allFiles.Contains(file)) continue;
                        allFiles.Add(file);
                    }
                }
                _foundFiles.Add(fileName, allFiles);
            }

            if (_foundFiles[fileName].Count == 0)
                Debug.Log($"[Synchronizer]: <color=red>Synchronization error:</color> {fileName}");
        }
        private static SynchronizationString GetSyncFileStr(SynchronizationString str, SynchronizationFile file)
        {
            foreach (var fileDef in file.Strings)
            {
                if (fileDef.Key != str.Key) continue;
                fileDef.Useful = true;
                if (!file.Newest) fileDef.NeedSync = fileDef.Value != str.Value;
                return fileDef;
            }

            str.Sync = true;
            str.NeedSync = true;
            file.Strings.Add(str);
            return str;
        }
        private SynchronizationFile GetSyncFile(string fileName, SynchronizationString[] strings)
        {
            foreach (var syncFile in _syncFiles)
            {
                if (syncFile.Name != fileName) continue;
                return syncFile;
            }
            var newSyncFile = new SynchronizationFile(fileName, true, strings);
            _syncFiles.Add(newSyncFile);
            return newSyncFile;
        } 
        private void RemoveUselessData()
        {
            for (var i = 0; i < _syncFiles.Count; i++)
            {
                if (!_syncFiles[i].Useful) 
                {
                    _syncFiles.Remove(_syncFiles[i--]);
                    continue;
                }

                for (var j = 0; j < _syncFiles[i].Strings.Count; j++)
                {
                    if (_syncFiles[i].Strings[j].Useful) continue;
                    _syncFiles[i].Strings.Remove(_syncFiles[i].Strings[j--]);
                }
            }
        }
        private void SaveDataJson()
        {
            var json = JsonUtility.ToJson(new SynchronizationData(_syncFiles.ToArray()), true);
            using var sw = File.CreateText(_syncDataFilePath);
            sw.Write(json);
            sw.Close();
        }
        private void LoadDataJson()
        {
            if (!File.Exists(_syncDataFilePath)) return;
            var json = File.ReadAllText(_syncDataFilePath);
            _syncFiles.AddRange(JsonUtility.FromJson<SynchronizationData>(json).Files);
        }
        public static SynchronizationString String(string key, string value) => new (key, value, true, true);
        private static void DebugSync(string fileName, string filePath, string key, string value)
            => Debug.Log($"[Synchronizer]: <color=yellow>{fileName}</color> "+
                         $"#str SYNC_{key} <color=cyan>{value}</color> \n {filePath}");
    }
}