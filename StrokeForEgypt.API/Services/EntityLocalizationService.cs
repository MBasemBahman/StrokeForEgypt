﻿using Microsoft.Extensions.Localization;
using StrokeForEgypt.API.Resources;
using System.Reflection;

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
