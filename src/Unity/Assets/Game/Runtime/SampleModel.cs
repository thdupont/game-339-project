using Binding.Runtime;
using Observable.Runtime;
using Observable.Runtime.MutableProperty;

public class SampleModel : IViewModel
{
    public Property<int> Gold = new();

    public void BumpGold()
    {
        Gold.SetValue(Gold.Value + 1);
    }

    public void SetGold(int value)
    {
        Gold.SetValue(value);
    }
}
