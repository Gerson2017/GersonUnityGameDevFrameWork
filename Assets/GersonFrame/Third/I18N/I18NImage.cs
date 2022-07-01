using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GersonFrame.ABFrame;

namespace Localization
{
    public class I18NImage : MonoBehaviour
    {
        private bool _initialized = false;
        private Image _img;

        void OnEnable()
        {
            if (!_initialized)
                _init();

            _updateTranslation(LocalizationManager.Instance.currentLanguage);
        }

        void OnDestroy()
        {
            if (_initialized)
            {
                LocalizationManager.OnLanguageChanged -= _onLanguageChanged;
            }
        }

        private void _init()
        {
            _img = GetComponent<Image>();
            _initialized = true;

            LocalizationManager.OnLanguageChanged += _onLanguageChanged;
        }

        private void _onLanguageChanged(SystemLanguage newLang)
        {
            _updateTranslation(newLang);
        }

        private void _updateTranslation(SystemLanguage newLang)
        {
            if(_img != null && _img.sprite != null)
            {

                //string path = "Assets/LocalizationUI/" + newLang.ToString() + "/" + originName + ".png";
                //   Debug.Log(path);
                //ResourceManager.Instance.LoadResource<Sprite>(path);
                var originName = _img.sprite.name;
                Sprite newSprite = LocalizationManager.Instance.GetSpriteFromKey(originName);

                if(newSprite != null)
                    _img.sprite = newSprite;
            }
        }
    }
}