
using TMPro;
using UnityEngine;

namespace QFSW.QC.FloatButtons
{
    public class FpsCounter : MonoBehaviour
    {
        private TextMeshProUGUI _textCom;
        private FPSMode _mode;
        private float _value;

        private int _frameCount = 0;
        private float _elapsedTime = 0f;

        public float Value => _value;

        private void Start()
        {
            QuantumRegistry.RegisterObject(this);
            _textCom = GetComponentInChildren<TextMeshProUGUI>();
            SetFps(_mode);
        }

        [Command("fps-mode", MonoTargetType.Registry)]
        public void SetFps(FPSMode mode)
        {
            _mode = mode;
            switch (_mode)
            {
                case FPSMode.None:
                    gameObject.SetActive(false);
                    break;

                case FPSMode.Avarage:
                    gameObject.SetActive(true);
                    _frameCount = 0;
                    _elapsedTime = 0;
                    break;

                case FPSMode.Immediately:
                    gameObject.SetActive(true);
                    break;
            }
        }

        public void Update()
        {
            switch (_mode)
            {
                case FPSMode.Immediately:
                    _value = 1f / Time.unscaledDeltaTime;
                    RefreshValue();
                    break;

                case FPSMode.Avarage:
                    _frameCount++;
                    _elapsedTime += Time.unscaledDeltaTime;

                    if (_elapsedTime >= 0.5f)
                    {
                        _value = _frameCount / _elapsedTime;
                        _frameCount = 0;
                        _elapsedTime = 0f;
                        RefreshValue();
                    }
                    break;
            }
        }

        private void RefreshValue()
        {
            _textCom.text = $"FPS {_value:F0}";
        }
    }
}
