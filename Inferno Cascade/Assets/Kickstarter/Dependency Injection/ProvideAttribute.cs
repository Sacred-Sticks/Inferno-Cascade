using System;

namespace Kickstarter.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class ProvideAttribute : Attribute { }
}
