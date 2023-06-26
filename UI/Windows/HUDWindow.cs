using Game.ScenesLoader;
using Game.UI.Implementations.Windows.Inventory;
using UnityEngine;

namespace Game.UI.Implementations.Windows.SelectLevel
{
    public class HUDWindow : UIWindow
    {
        [SerializeField] private string MenuSceneName = "Menu";

        public void BackToMenu() => BackToMenuProcess();
        public void ShowInventory() => ShowInventoryProcess();
        
        private async void BackToMenuProcess()
        {
            CanvasGroup.blocksRaycasts = false;
            var lsw = UIController.LoadWindow<LoadingScreenWindow>();
            await lsw.Show();

            HideImmediate();
            await SceneLoader.CurrentScene.AbstractInstance.Exit();
            await lsw.SetProgress(0.25f);

            await SceneLoader.UnloadCurrentLevel();
            await lsw.SetProgress(0.25f);

            await SceneLoader.Load(MenuSceneName);
            await lsw.SetProgress(1f);

            await lsw.Hide();
            UIController.UnloadWindow(lsw);
            await SceneLoader.CurrentScene.AbstractInstance.Enter();
        }
        private async void ShowInventoryProcess()
        {
            var window = UIController.GetWindow<PlayerInventoryWindow>();
            await window.Cancellation();
            await window.Show();
        }
    }
}