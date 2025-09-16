using System;
using System.Reflection;
using Observable.Runtime;
using UnityEngine;

namespace Binding.Runtime
{
    public interface IViewModelBinding
    {
        internal void TrySetViewModel(IViewModel viewModel, Type viewModelType, string serializedViewModelTypeName);
    }

    public interface IViewModelBinding<T> : IViewModelBinding where T : IViewModel
    {
        void IViewModelBinding.TrySetViewModel(IViewModel viewModel, Type viewModelType, string serializedViewModelTypeName)
        {
            if (typeof(T).IsAssignableFrom(viewModelType))
            {
                OnSetViewModel((T)viewModel);
            }
        }

        void OnSetViewModel(T viewModel);
    }

    public static class ViewModelBindingHelper
    {
        public static void SetViewModelInChildren(this GameObject gameObject, IViewModel viewModel)
        {
            if (viewModel == null)
            {
                Debug.LogError($"{nameof(SetViewModelInChildren)}|{nameof(viewModel)} is null!");
                return;
            }

            var bindingList = gameObject.GetComponentsInChildren<IViewModelBinding>(includeInactive: true);
            var bindingCount = bindingList.Length;
            var viewModelType = viewModel.GetType();
            var viewModelTypeAssemblyQualifiedName = viewModelType.AssemblyQualifiedName;

            // We use an index-based for loop here instead of a foreach,
            // because this code will be called a lot, so the overhead is worth removing,
            // even though it's insignificant in most cases.
            for (int i = 0; i < bindingCount; ++i)
            {
                bindingList[i].TrySetViewModel(viewModel, viewModelType, viewModelTypeAssemblyQualifiedName);
            }
        }

        public static Property<T> FindObservableProperty<T>(this IViewModel viewModel, string propertyName)
        {
            var viewModelType = viewModel.GetType();

            var propertyInfo = viewModelType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null && typeof(Property<T>).IsAssignableFrom(propertyInfo.PropertyType))
            {
                var property = (Property<T>)propertyInfo.GetValue(viewModel);
                if (property == null)
                {
                    Debug.LogWarning($"{viewModelType.Name}|{nameof(FindObservableProperty)}|{propertyInfo.Name} is null!");
                    return default;
                }

                return property;
            }

            var fieldInfo = viewModelType.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null && typeof(Property<T>).IsAssignableFrom(fieldInfo.FieldType))
            {
                var property = (Property<T>)fieldInfo.GetValue(viewModel);
                if (property == null)
                {
                    Debug.LogWarning($"{viewModelType.Name}|{nameof(FindObservableProperty)}|{fieldInfo.Name} is null!");
                    return default;
                }

                return property;
            }

            return default;
        }

        public static Action<T> FindTargetAction<T>(this IViewModel viewModel, string methodName)
        {
            var viewModelType = viewModel.GetType();

            var methodInfo = viewModelType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            if (methodInfo == null)
            {
                Debug.LogWarning($"{viewModelType.Name}|{nameof(FindTargetAction)}|{methodName} not found!");
                return default;
            }

            var methodParameters = methodInfo.GetParameters();
            if (methodParameters.Length == 0)
            {
                return value => methodInfo.Invoke(viewModel, Array.Empty<object>());
            }
            else
            {
                return value => methodInfo.Invoke(viewModel, new object[] { value });
            }
        }
    }
}
