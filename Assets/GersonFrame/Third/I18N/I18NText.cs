using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Localization
{
    public class I18NText : MonoBehaviour
    {
        public string lanKey = "";
        public List<string> valueList = new List<string>();

        private Text _text;
        private bool _initialized = false;
        private Font _defaultFont;
        private float _defaultLineSpacing;
        private int _defaultFontSize;
        void OnEnable()
        {
            if (!_initialized)
                _init();

            updateTranslation();
        }

        void OnDestroy()
        {
            if (_initialized)
            {
                LocalizationManager.OnLanguageChanged -= _onLanguageChanged;
                LocalizationManager.OnFontChanged -= _onFontChanged;
            }
        }

        private void _updateTranslation()
        {
            if (_text)
            {
                _text.text = LocalizationManager.Instance.GetStringFromKey(lanKey);
            }
        }

        public void updateTranslation()
        {
            _updateTranslation();
        }

        public void SetText(string str)
        {
            if (_text == null)
            {
                _text = GetComponent<Text>();
            }
            if (_text)
            {
                _text.text = str;
                _text.enabled = false;
                _text.enabled = true;
            }
        }

        private void _init()
        {
            _text = GetComponent<Text>();
            if(_text == null)
            {
                Debug.LogWarning("I18NText:" + gameObject.name);
                return;
            }
            else
            {
                _defaultFont = _text.font;
                _defaultLineSpacing = _text.lineSpacing;
                _defaultFontSize = _text.fontSize;
            }
            
            _initialized = true;

            if (LocalizationManager.Instance.useCustomFonts)
            {
                _changeFont(LocalizationManager.Instance.customFont);
            }

            LocalizationManager.OnLanguageChanged += _onLanguageChanged;
            LocalizationManager.OnFontChanged += _onFontChanged;

            if (!_text)
            {
                Debug.LogWarning(string.Format("{0}: Text component was not found!", this));
            }
        }

        private void _onLanguageChanged(SystemLanguage gameLang)
        {
            _updateTranslation();
        }

        private void _onFontChanged(I18NFonts newFont)
        {
            _changeFont(newFont);
        }

        private void _changeFont(I18NFonts f)
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
    }
}