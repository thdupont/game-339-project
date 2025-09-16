using Binding.Runtime;

namespace View.Runtime
{
    public class IntToStringBinding : InspectorBinding<int, string>
    {
        protected override string ConvertValue()
        {
            return ViewModelProperty.Value.ToString();
        }
    }
}