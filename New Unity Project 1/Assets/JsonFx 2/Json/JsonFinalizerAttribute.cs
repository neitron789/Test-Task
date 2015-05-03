using System;

namespace JsonFx.Json
{
    /// <summary>
    /// Designates a method to be called when object deserialization is finished.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class JsonFinalizerAttribute : Attribute
    {
    }
}