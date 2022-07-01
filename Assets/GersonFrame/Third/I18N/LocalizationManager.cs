using GersonFrame.ABFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Localization
{
    public class LocalizationManager
    {
        public List<SystemLanguage> languages = new List<SystemLanguage>()
        {
            SystemLanguage.English,
            SystemLanguage.ChineseSimplified,
            SystemLanguage.ChineseTraditional
        };

        public SystemLanguage currentLanguage;
        public int languageIndex = 0;

        private const string GAME_LANG = "game_language";

        private Dictionary<SystemLanguage, LocalizationData> localizationDatas = new Dictionary<SystemLanguage, LocalizationData>();

        private ACLocalizationData ACLocalizationData = null;

        public delegate void LanguageChange(SystemLanguage newLanguage);
        public static event LanguageChange OnLanguageChanged;
        public delegate void FontChange(I18NFonts newFont);
        public static event FontChange OnFontChanged;

        private List<I18NFonts> _langFonts;
        private List<I18NTMPFonts> _langTMPFonts;



        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                }
                return _instance;
            }
        }

        public bool useCustomFonts = false;
        public I18NFonts customFont = null;
        public I18NTMPFonts customTMPFont = null;

        LocalizationManager()
        {
            InitGameLang();

            localizationDatas.Clear();
            LoadLanguages();
            //if(Application.isPlaying)
            //{
            //    var fontGo = GameObject.Instantiate(ResourceManager.Instance.LoadResource<GameObject>("Assets/Prefabs/UI/LocalizationFonts.prefab"));
            //    fontGo.name = "LocalizationFonts";
            //    GameObject.DontDestroyOnLoad(fontGo);
            //    LocalizationFonts localizationFonts = fontGo.GetComponent<LocalizationFonts>();

            //    useCustomFonts = true;
            //    SetFonts(localizationFonts.langFonts, localizationFonts.langTMPFonts);

            //    SwitchFonts();
            //}
        }

        private void InitGameLang()
        {
            if (!PlayerPrefs.HasKey(GAME_LANG))
            {
                if (languages.Contains(Application.systemLanguage))
                {
                    if (Application.systemLanguage == SystemLanguage.Chinese)
                        currentLanguage = SystemLanguage.ChineseSimplified;
                    else
                        currentLanguage = Application.systemLanguage;
                }
                else
                    currentLanguage = SystemLanguage.English;
            }
            else
            {
                currentLanguage = (SystemLanguage)PlayerPrefs.GetInt(GAME_LANG);
                if (!languages.Contains(currentLanguage))
                    currentLanguage = SystemLanguage.English;
            }
            languageIndex = languages.IndexOf(currentLanguage);
        }

        public void LoadACLocalizationData(string text)
        {
            if(ACLocalizationData == null)
                ACLocalizationData = new ACLocalizationData(text);
        }

        /// <summary>
        /// 设置语言列表
        /// </summary>
        /// <param name="languages"></param>
        public void SetLanguages(List<SystemLanguage> languages)
        {
            this.languages = languages;
        }

        public void SetFonts(List<I18NFonts> fonts, List<I18NTMPFonts> langTMPFonts)
        {
            this._langFonts = fonts;
            this._langTMPFonts = langTMPFonts;
        }

#if UNITY_EDITOR
        [MenuItem("Localization/Reload")]
#endif
        /// <summary>
        /// 重新加载语言包
        /// </summary>
        public static void ReLoad()
        {
            Instance.InitGameLang();
            Instance.localizationDatas.Clear();
            Instance.LoadLanguages();
        }

        /// <summary>
        /// 加载语言配置
        /// </summary>
        private void LoadLanguages()
        {
            localizationDatas = new Dictionary<SystemLanguage, LocalizationData>();
            for (int i = 0; i < languages.Count; i++)
            {
                localizationDatas.Add(languages[i], new LocalizationData(languages[i]));
            }
        }

        public LocalizationData GetCurrentLocalizationData()
        {
            return localizationDatas[currentLanguage];
        }

        public void SwitchLanguage(SystemLanguage language)
        {
            mCurLanguagePath = "";
            languageIndex = languages.IndexOf(language);
            if (languageIndex == -1)
            {
                languageIndex = 0;
            }

            currentLanguage = languages[languageIndex];
            OnLanguageChanged(currentLanguage);

            SwitchFonts();

            PlayerPrefs.SetInt(GAME_LANG, (int)currentLanguage);
        }

        public void SwitchFonts()
        {
            if (useCustomFonts)
            {
                I18NFonts newFont = null;
                customFont = null;
                if (_langFonts != null && _langFonts.Count > 0)
                {
                    foreach (I18NFonts f in _langFonts)
                    {
                        if (f.lang == currentLanguage)
                        {
                            newFont = f;
                            customFont = f;
                            break;
                        }
                    }
                }

                customTMPFont = null;
                if (_langTMPFonts != null && _langTMPFonts.Count > 0)
                {
                    foreach (I18NTMPFonts f in _langTMPFonts)
                    {
                        if (f.lang == currentLanguage)
                        {
                            customTMPFont = f;
                            break;
                        }
                    }
                }

                if (OnFontChanged != null)
                    OnFontChanged(newFont);
            }
            else
            {
                customFont = null;
                customTMPFont = null;
            }
        }


        /// <summary>
        /// 获取 Key 所有语言
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> GetStringListFromKey(string key)
        {
            List<string> lanList = new List<string>();
            foreach(var lan in localizationDatas.Keys)
            {
                var localizationData = localizationDatas[lan];
                lanList.Add(localizationData.GetValueByKey(key, key));
            }

            return lanList;
        }

        public string GetStringFromKey(string key, string[] parameters)
        {
            string val = GetTextFromKeyAndColumnName(key, key, "VALUE");

            if (parameters != null && parameters.Length > 0)
            {
                return string.Format(val.Replace("\\n", Environment.NewLine), parameters);
            }
            return val.Replace("\\n", Environment.NewLine);
        }
        public string GetStringFromKey(string key, string defaultString = "")
        {
            if(string.IsNullOrEmpty(defaultString))
            {
                return GetTextFromKeyAndColumnName(key, key, "VALUE");
            }
            return GetTextFromKeyAndColumnName(key, defaultString, "VALUE");
        }

        private string GetTextFromKeyAndColumnName(string key, string defaultString = "", string columnName = "VALUE")
        {
            return GetCurrentLocalizationData().GetValueByKey(key, defaultString, columnName);
        }






        private string mCurLanguagePath = "";
        public Sprite GetSpriteFromKey(string key)
        {
            Sprite newSprite = null;
            try
            {
                if (string.IsNullOrEmpty(mCurLanguagePath))
                    mCurLanguagePath = "Assets/LocalizationUI/" + currentLanguage.ToString() + "/";
                newSprite = ResourceManager.Instance.LoadResource<Sprite>(mCurLanguagePath + key + ".png");
                if (newSprite == null)
                {
                    if (currentLanguage != SystemLanguage.English)
                    {
                        newSprite = ResourceManager.Instance.LoadResource<Sprite>("Assets/LocalizationUI/English/" + key + ".png");
                    }
                }
                if (newSprite == null)
                    MyDebuger.LogError(" not found sprite assets " + mCurLanguagePath + key + ".png");
            }
            catch (Exception e)
            {
                MyDebuger.LogError(" not found sprite assets " + mCurLanguagePath + key + ".png");
            }
            return newSprite;
        }


        public string GetACStringFromKey(string key, string defaultString = "")
        {
            if(ACLocalizationData != null)
            {
                if (string.IsNullOrEmpty(defaultString))
                {
                    return ACLocalizationData.GetValueByKey(key, key, currentLanguage.ToString());
                }
                return ACLocalizationData.GetValueByKey(key, defaultString, currentLanguage.ToString());
            }
            else            
                return key;            
        }
    }

    

#region HELPER CLASSES

    /// <summary>
    /// Helper class, containing font parameters.
    /// </summary>
    [Serializable]
    public class I18NFonts
    {
        /// <summary>
        /// Font language code.
        /// </summary>
        public SystemLanguage lang;
        /// <summary>
        /// Font
        /// </summary>
        public Font font;
        /// <summary>
        /// True, when components should use custom line spacing.
        /// </summary>
        public bool customLineSpacing = false;
        /// <summary>
        /// Custom line spacing value.
        /// </summary>
        public float lineSpacing = 1.0f;
        /// <summary>
        /// True, when components should use custom font size.
        /// </summary>
        public bool customFontSizeOffset = false;
        /// <summary>
        /// Custom font size offset in percents.
        /// e.g. 55, -10
        /// </summary>
        public int fontSizeOffsetPercent = 0;        
    }


    [Serializable]
    public class I18NTMPFonts
    {
        /// <summary>
        /// Font language code.
        /// </summary>
        public SystemLanguage lang;

        /// <summary>
        /// Font
        /// </summary>
        public TMP_FontAsset font;
    }

    /// <summary>
    /// Helper class, containing sprite parameters.
    /// </summary>
    [Serializable]
    public class I18NSprites
    {

        /// <summary>
        /// Sprite lang code.
        /// </summary>
        public SystemLanguage language;
        /// <summary>
        /// Sprite.
        /// </summary>
        public Sprite image;
    }

#endregion
}
