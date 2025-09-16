using System;
using System.Collections.Generic;
using ConstructorMap = System.Collections.Generic.Dictionary<System.Type, System.Func<object>>;

namespace DependencyInjection.Runtime
{
    public interface IScope
    {
        string Name { get; }
        IScope Parent { get; }
        int Version { get; }
        void IncrementVersion();
        event Action<IScope> ClearEvent;

        void RegisterSingleton<TInterface>(Func<TInterface> constructor);
        void Pull<TInterface>(out TInterface instance);
        object Pull(Type interfaceType);
        void Clear();
    }

    public class Scope : IScope
    {
        #region Static interface
        public delegate IScope ScopeConstructor(string name, IScope parent);

        private static IScope _root;
        private static IScope _current;
        private static ScopeConstructor _constructor;
        private static IScope DefaultScopeConstructor(string name, IScope parent)
        {
            return new Scope(name, parent);
        }

        public static IScope Current
        {
            get
            {
                if (_current == null)
                {
                    Push("root");
                }

                return _current;
            }
        }

        public static void SetConstructor(ScopeConstructor constructor)
        {
            _constructor = constructor;
        }

        private static IScope Push(string name)
        {
            var constructor = _constructor ?? DefaultScopeConstructor;

            var scope = constructor(name, _current);

            if (scope == null)
            {
                throw new InvalidOperationException($"{nameof(Scope)}|{nameof(Push)}|constructor returned null!");
            }

            _current = scope;

            _root ??= scope;

            IncrementVersions();

            return scope;
        }

        public static void Pop()
        {
            if (_current == null)
            {
                return;
            }

            _current = _current.Parent;

            IncrementVersions();
        }

        private static void IncrementVersions()
        {
            var node = _current;
            while (node != null)
            {
                node.IncrementVersion();
                node = node.Parent;
            }
        }
        #endregion

        #region IScope
        private ConstructorMap _constructorMap;
        private ConstructorMap ConstructorMap => _constructorMap ??= new ConstructorMap();

        private ConstructorMap _singletonConstructorMap;
        private ConstructorMap SingletonConstructorMap => _singletonConstructorMap ??= new ConstructorMap();

        private Dictionary<Type, object> _singletonMap;
        private Dictionary<Type, object> SingletonMap => _singletonMap ??= new Dictionary<Type, object>();

        private List<Type> _activePullList;
        private List<Type> ActivePullList => _activePullList ??= new List<Type>();

        public string Name { get; private set; }
        public IScope Parent { get; private set; }
        public int Version { get; private set; }
        public void IncrementVersion() => ++Version;
        public event Action<IScope> ClearEvent;

        internal Scope(string name, IScope parent)
        {
            Name = name;
            Parent = parent;
        }

        public void RegisterSingleton<TInterface>(Func<TInterface> constructor)
        {
            Register(SingletonConstructorMap, constructor);
        }

        public void Pull<TInterface>(out TInterface typedInstance)
        {
            Pull(this, typeof(TInterface), out var instance);
            typedInstance = (TInterface)instance;
        }

        public object Pull(Type interfaceType)
        {
            Pull(this, interfaceType, out var instance);
            return instance;
        }

        public void Clear()
        {
            ClearEvent?.Invoke(this);
            ClearEvent = null;

            _singletonMap?.Clear();
            _singletonMap = null;

            _singletonConstructorMap?.Clear();
            _singletonConstructorMap = null;

            _constructorMap?.Clear();
            _constructorMap = null;

            IncrementVersion();
        }
        #endregion

        private void Register<TInterface>(ConstructorMap constructorMap, Func<TInterface> constructor)
        {
            if (constructor == null)
            {
                throw new InvalidOperationException($"{nameof(Scope)}|{nameof(Register)}|scope={Name}|{nameof(constructor)} cannot be null.");
            }

            if (constructorMap.ContainsKey(typeof(TInterface)))
            {
                throw new InvalidOperationException($"{nameof(Scope)}|{nameof(Register)}|scope={Name}|type={typeof(TInterface).Name}|already registered!");
            }

            constructorMap.Add(typeof(TInterface), WrappedConstructor);

            object WrappedConstructor()
            {
                return constructor();
            }
        }

        private static void Pull(Scope node, Type interfaceType, out object instance, bool throwExceptions = true)
        {
            var currentScopeName = node.Name;

            instance = default;

            while (node != null)
            {
                if (node.ActivePullList.Contains(interfaceType))
                {
                    if (throwExceptions)
                    {
                        throw new InvalidOperationException($"{nameof(Scope)}|{nameof(Pull)}|scope={node.Name}|type={interfaceType.Name}|found cyclic dependency! chain: '{string.Join(',', node.ActivePullList)}'");
                    }

                    break;
                }

                if (node.SingletonMap.TryGetValue(interfaceType, out var cachedInstance))
                {
                    instance = cachedInstance;
                    return;
                }

                node.ActivePullList.Add(interfaceType);

                if (node.SingletonConstructorMap.TryGetValue(interfaceType, out var singletonConstructor))
                {
                    var entry = singletonConstructor();
                    node.ActivePullList.Remove(interfaceType);
                    node.SingletonMap.Add(interfaceType, entry);
                    instance = entry;
                    return;
                }

                if (node.ConstructorMap.TryGetValue(interfaceType, out var constructor))
                {
                    instance = constructor();
                    node.ActivePullList.Remove(interfaceType);
                    return;
                }

                node.ActivePullList.Remove(interfaceType);

                node = (Scope)node.Parent;
            }

            if (throwExceptions)
            {
                throw new InvalidOperationException($"{nameof(Scope)}|{nameof(Pull)}|scope={currentScopeName}|type={interfaceType.Name}|no registration found!");
            }
        }
    }
}
