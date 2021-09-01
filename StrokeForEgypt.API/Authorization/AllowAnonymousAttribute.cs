using System;

namespace StrokeForEgypt.API.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAllAttribute : Attribute
    { }
}