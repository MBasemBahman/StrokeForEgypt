using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StrokeForEgypt.API.Services
{
    public class EntityLocalizationService
    {
        private readonly IStringLocalizer localizer;
        public EntityLocalizationService(IStringLocalizerFactory factory)
        {
            AssemblyName assemblyName = new(typeof(EntityResources).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(EntityResources), assemblyName.Name);
        }

        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
