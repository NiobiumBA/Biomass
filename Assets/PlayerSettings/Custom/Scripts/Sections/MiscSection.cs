using Localization;
using PlayerSettings;
using System.Collections.Generic;
using System.Linq;

public class MiscSection : Section
{
    private SwitcherOption m_languageOption;

    public override void Apply()
    {
        Translation.Language = XmlTranslationParser.LoadLanguage(m_languageOption.NamedValue);
    }

    public override void Create()
    {
        m_languageOption = CreateOption<SwitcherOption, int>("Language", 0);
        m_languageOption.TranslateValueName = false;
        m_languageOption.ValueNames = GetAllLanguages();
        m_languageOption.IsTranslate = false;
    }

    private List<string> GetAllLanguages()
    {
        return Translation.Languages.ToList();
    }
}
