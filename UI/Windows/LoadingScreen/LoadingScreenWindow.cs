using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Implementations.Windows.SelectLevel
{
    public class LoadingScreenWindow : UIWindow
    {
        [SerializeField] private TMP_Text _ProgressText;
        [SerializeField] private Image _ProgressImage;

        public override void Initialize()
        { 
            base.Initialize();
            ResetProgress();
        }
        public void ResetProgress()
        {
            _ProgressText.text = $"0%";
            _ProgressImage.fillAmount = 0;
        }
        public async UniTask SetProgress(float value)
        {
            value = Mathf.Clamp01(value);
            _ProgressText.text = $"{(value * 100f):0.#}%";
            _ProgressImage.fillAmount = value;
            await UniTask.Yield();
        }
    }
}