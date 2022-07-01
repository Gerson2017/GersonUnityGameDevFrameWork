using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Localization
{
    public class I18NSprite : MonoBehaviour
    {
        private bool _initialized = false;
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Sprite _defaultSprite;
        [SerializeField]
        private List<I18NSprites> _sprites = new List<I18NSprites>();

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
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_defaultSprite == null)
            {
                _defaultSprite = _spriteRenderer.sprite;
            }
            _initialized = true;

            LocalizationManager.OnLanguageChanged += _onLanguageChanged;
        }

        private void _onLanguageChanged(SystemLanguage newLang)
        {
            _updateTranslation(newLang);
        }

        private void _updateTranslation(SystemLanguage newLang)
        {
            if (_sprites == null || (_sprites != null && _sprites.Count == 0))
            {
                return;
            }

            Sprite newSprite = _defaultSprite;

            for (int i=0; i<_sprites.Count; i++)
            {
                if (_sprites[i].language == newLang)
                {
                    newSprite = _sprites[i].image;
                    break;
                }
            }

            _spriteRenderer.sprite = newSprite;
        }
    }
}