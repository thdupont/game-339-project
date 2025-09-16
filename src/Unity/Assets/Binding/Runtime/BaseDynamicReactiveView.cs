using UnityEngine;

namespace Binding.Runtime
{
    /// <summary>
    /// Handles the basic Unity lifecycle boilerplate for subscribe and unsubscribe.
    /// </summary>
    /// <remarks>
    /// Use this when your view expects someone else to supply its view model with <see cref="ViewModelBindingHelper.SetViewModelInChildren(GameObject, IViewModel)"/>.
    /// </remarks>
    public abstract class BaseDynamicReactiveView<T> : BaseReactiveView, IViewModelBinding<T> where T : IViewModel
    {
        protected T _viewModel;

        protected override void TrySubscribe()
        {
            if (!_didSubscribe && _didCallStart && _viewModel != null)
            {
                _didSubscribe = true;
                Subscribe();
            }
        }

        protected override void TryUnsubscribe()
        {
            if (_didSubscribe && _viewModel != null)
            {
                _didSubscribe = false;
                Unsubscribe();
            }
        }

        void IViewModelBinding<T>.OnSetViewModel(T viewModel)
        {
            _viewModel = viewModel;
        }
    }
}