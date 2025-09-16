using System;

namespace Binding.Runtime
{
    [AttributeUsage(validOn:AttributeTargets.Class|AttributeTargets.Field|AttributeTargets.Property)]
    public class BindingSourceAttribute : Attribute
    {

    }
}