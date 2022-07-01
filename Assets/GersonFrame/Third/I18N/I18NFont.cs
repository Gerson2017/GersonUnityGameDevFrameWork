using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Localization
{
    public class I18NFont : MonoBehaviour
    {
        private Text _text;
        private TextMeshProUGUI _meshText;

        private bool _initialized = false;
        private Font _defaultFont;
        private float _defaultLineSpacing;
        private int _defaultFontSize;
        void OnEnable()
        {
            if (!_initialized)
                _init();
        }

        void OnDestroy()
        {
            if (_initialized)
            {
                LocalizationManager.OnFontChanged -= _onFontChanged;
            }
        }

        private void _init()
        {
            _text = GetComponent<Text>();
            _meshText = GetComponent<TextMeshProUGUI>();
            if (_meshText != null || _text != null)
            {
                LocalizationManager.OnFontChanged += _onFontChanged;
            }

            if (_text == null && _meshText == null)
            {
                Debug.LogWarning("I18NFont:" + gameObject.name);
            }

            if(_text != null)
            {
                _defaultFont = _text.font;
                _defaultLineSpacing = _text.lineSpacing;
                _defaultFontSize = _text.fontSize;
            }

            if (LocalizationManager.Instance.useCustomFonts)
            {
                _changeFont(LocalizationManager.Instance.customFont);
            }

            _initialized = true;
            
        }
        private void _onFontChanged(I18NFonts newFont)
        {
            _changeFont(newFont);
        }

        private void _changeFont(I18NFonts f)
        {
            if(_text != null)
            {
                if (f != null)
                {
                    if (f.font)
                    {
                        _text.font = f.font;
                    }
                    else
                    {
                        _text.font = _defaultFont;
                    }
                    if (f.customLineSpacing)
                    {
                        _text.lineSpacing = f.lineSpacing;
                    }
                    if (f.customFontSizeOffset)
                    {
                        _text.fontSize = (int)(_defaultFontSize + (_defaultFontSize * f.fontSizeOffsetPercent / 100));
                    }
                }
                else
                {
                    _text.font = _defaultFont;
                    _text.lineSpacing = _defaultLineSpacing;
                    _text.fontSize = _defaultFontSize;
                }
            }
            

            if(_meshText != null && LocalizationManager.Instance.customTMPFont.font != null)
            {
                _meshText.font = LocalizationManager.Instance.customTMPFont.font;
            }
        }
    }
}