
using Mochineko.VoiceActivityDetection.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Birdtracks.Game.ONS
{


    public class VADSettingsUI : MonoBehaviour
    {
        [SerializeField] private VADParameters _settingsData;
        [SerializeField] private GameObject _settingsUIPrefab;
        [SerializeField] private Transform _gridParent;
        [SerializeField] private ONS_Hint _hint;
        [SerializeField] private GameObject _hintContent;

        private void Start()
        {
            var index = 0;
            var settings = _settingsData.GetSettings();
            foreach (var s in settings)
            {
                var newSetting = Instantiate(_settingsUIPrefab, _gridParent);

                var slider = newSetting.GetComponentInChildren<Slider>();
                slider.minValue = s.SliderValueRange.x;
                slider.maxValue = s.SliderValueRange.y;
                slider.value = s.Value;
                slider.onValueChanged.AddListener(s.UpdateValue);

                var settingName = newSetting.GetComponentInChildren<TextMeshProUGUI>();
                settingName.text = s.UIName;

                var hintButton = newSetting.GetComponentInChildren<ONS_HintButton>();
                hintButton.SetID(index);
                hintButton.SetHintUI(this);

                index++;
            }
            ExitHint();
            SubscribeToHintExitButton();
        }

        public void CloseSettings()
        {
            GetComponentInParent<Canvas>().enabled = false;
        }

        public void OpenSettings()
        {
            GetComponentInParent<Canvas>().enabled = true;
        }

        public void OpenHint(int id)
        {
            var settings = _settingsData.GetSettings();
            _hint.SetHintText(settings[id].SettingHint);
            _hintContent.SetActive(true);
        }

        private void SubscribeToHintExitButton()
        {
            var exitButton = _hint.GetHintExitButton();
            
            exitButton.onClick.AddListener(ExitHint);
            
        }

        private void ExitHint()
        {
            _hintContent.SetActive(false);
        }
    }
}