using Microsoft.Extensions.Localization;
using StrokeForEgypt.AdminApp.Resources;
using System.Reflection;

namespace StrokeForEgypt.AdminApp.Services
{
    public class CommonLocalizationService
    {
        private readonly IStringLocalizer localizer;
        public CommonLocalizationService(IStringLocalizerFactory factory)
        {
            AssemblyName assemblyName = new AssemblyName(typeof(CommonResources).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(CommonResources), assemblyName.Name);
        }

        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
