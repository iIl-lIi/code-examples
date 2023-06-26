using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Character.Implementations.Player
{
    public static class PlayerLoader
    {
        private static AssetReference PlayerPrefabAsset;
        
        public static IPlayer PlayerInstance { get; private set; }
        public static bool IsBusy { get; private set; }
    
        public static readonly Vector3 DefaultPlayerPosition = new(0, 10, 0);

        public static void Initialize(AssetReference playerPrefabAsset)
        {
            UnloadCurrentPlayer();
            PlayerPrefabAsset = playerPrefabAsset;
            IsBusy = false;
        }
        public static async UniTask<IPlayer> LoadPlayer(Vector3 position, Vector2 rotation, Transform parent)
        {
            if (IsBusy) return null;
            IsBusy = true;
            var instance = await PlayerPrefabAsset.InstantiateAsync(position, Quaternion.identity, parent).Task;
            PlayerInstance = instance.GetComponent<IPlayer>();
            await PlayerInstance.Initialize();
            PlayerInstance.RootTransform = instance.transform;
            PlayerInstance.SetPosition(position);
            PlayerInstance.SetRotation(rotation);
            IsBusy = false;
            return PlayerInstance;
        }
        public static void UnloadCurrentPlayer()
        {
            if (IsBusy || PlayerInstance == null) return;
            IsBusy = true;
            var playerO = PlayerInstance.RootTransform.gameObject;
            PlayerPrefabAsset.ReleaseInstance(playerO);
            PlayerInstance = null;
            IsBusy = false;
        }
    }
}