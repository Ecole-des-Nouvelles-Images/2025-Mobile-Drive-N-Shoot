using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Localisation
{
    public class LocaleSelector : MonoBehaviour
    {
        public void LocalisationSettings(int localId)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localId];
        }
    }
}
