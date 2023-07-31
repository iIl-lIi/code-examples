using Game.Character.Implementations.Player;
using Game.ScenesLoader;
using UnityEngine;
using Game.UI.Implementations.Windows.SelectLevel;
using Game.UI;
using UnityEngine.AddressableAssets;

namespace Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private string _MenuSceneName = "Menu";
        [SerializeField] private AssetReference _PlayerPrefab;
        [SerializeField] private RectTransform _UIRoot;
        [SerializeField] private ContainersList _UIContainers;
        [SerializeField] private LightingDataList _LightingDataList;

        private async void Awake()
        {
            Application.targetFrameRate = 144;
            UIController.Initialize(_UIRoot, _UIContainers);
            var lsw = UIController.LoadWindow<LoadingScreenWindow>();
            lsw.ShowImmediate();

            BinarySaveSystem.Initialize();
            SceneLoader.Initialize();
            PlayerLoader.Initialize(_PlayerPrefab);
            LightingController.Initialize(_LightingDataList.List);
            await lsw.SetProgress(0.25f);

            await SceneLoader.Load(_MenuSceneName);
            await lsw.SetProgress(0.5f);
            
            var slw = UIController.LoadWindow<SelectLevelWindow>();
            await lsw.SetProgress(1f);

            await lsw.Hide();
            await slw.Show();
            UIController.UnloadWindow(lsw);
            Destroy(gameObject);
        }
    }
}