using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BattleSystem.Character
{
    public static class CharacterLoader
    {
        private static readonly List<IDObject<GameObject>> loadedMaps = new();
        
        public static void UnloadAll()
        {
            loadedMaps.ForEach(x => Addressables.ReleaseInstance(x.Object));
            loadedMaps.Clear();
        }
        public static async Task<GameObject> Load(string id)
        {
            var operation = Addressables.InstantiateAsync(id);
            while (operation.IsDone == false) await Task.Yield();
            loadedMaps.Add(new IDObject<GameObject>(id, operation.Result));
            return operation.Result;
        }
        public static void Unload(string id)
        {
            var loadedMap = loadedMaps.FirstOrDefault(x => x.ID == id);
            if(loadedMap == default) throw new ArgumentException($"Character '{id}' is not loaded.");
            Addressables.ReleaseInstance(loadedMap.Object);
            loadedMaps.Remove(loadedMap);
        } 
    }
}