using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
namespace BusinessLayer.Services
{
    public class LocalizationService<T> : IStringLocalizer<T>
    {
        private readonly Dictionary<string, Dictionary<string, string>> _localizationData;
        private readonly ILogger<LocalizationService<T>> _logger;

        public LocalizationService(ILogger<LocalizationService<T>> logger)
        {
            _localizationData = new Dictionary<string, Dictionary<string, string>>();
            _logger = logger;

            LoadLocalizationData("messages.en.json", "en");
            LoadLocalizationData("messages.ur.json", "ur");
        }

        private void LoadLocalizationData(string filePath, string culture)
        {
            try
            {
                var jsonData = File.ReadAllText(Path.Combine("Resources", filePath));
                var localization = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

                if (localization != null)
                {
                    _localizationData[culture] = localization;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading localization file {filePath}: {ex.Message}");
            }
        }

        public LocalizedString this[string name]
        {
            get
            {
                var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                if (_localizationData.ContainsKey(culture) && _localizationData[culture].ContainsKey(name))
                {
                    return new LocalizedString(name, _localizationData[culture][name]);
                }
                return new LocalizedString(name, name, true);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
            => new LocalizedString(name, string.Format(this[name], arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (_localizationData.ContainsKey(culture))
            {
                foreach (var kvp in _localizationData[culture])
                {
                    yield return new LocalizedString(kvp.Key, kvp.Value);
                }
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }
}
