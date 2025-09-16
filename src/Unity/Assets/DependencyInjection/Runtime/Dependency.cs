
namespace DependencyInjection.Runtime
{
    public struct Dependency<T>
    {
        private T _value;
        private int _version;

        public T Value
        {
            get
            {
                var scope = Scope.Current;

                if (_value == null || _version != scope.Version)
                {
                    scope.Pull(out _value);
                    _version = scope.Version;
                }

                return _value;
            }
        }
    }
}
