using Microsoft.Extensions.Localization;
namespace CommonDataLayer.Helpers
{
    public static class LocalizationHelper
    {
        public static string GetLocalizedMessage<TEnum>(IStringLocalizer localizer, TEnum code) where TEnum : Enum
        {
            var key = code.ToString();
            return localizer[key];
        }
    }
}
