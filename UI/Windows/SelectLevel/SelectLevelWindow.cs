using Game.ScenesLoader;
using UnityEngine;

namespace Game.UI.Implementations.Windows.SelectLevel
{
    public class SelectLevelWindow : UIWindow
    {
        [SerializeField] private string StartSceneName = "Level1";

        public void StartGame() => StartGameProcess();
        public void Exit() => Application.Quit();
        
        private async void StartGameProcess()
        {
            CanvasGroup.blocksRaycasts = false;
            var lsw = UIController.LoadWindow<LoadingScreenWindow>();
            await lsw.Show();

            HideImmediate();
            UIController.UnloadAllWindows(this, lsw);
            await lsw.SetProgress(0.25f);

            await SceneLoader.Load(StartSceneName);
            await lsw.SetProgress(1f);
            
            await lsw.Hide();
            UIController.UnloadWindow(lsw);
            UIController.UnloadWindow(this);
            await SceneLoader.CurrentScene.AbstractInstance.Enter();
        }
    }
}