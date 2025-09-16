using System;
using DependencyInjection.Runtime;
using Observable.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Binding.Runtime
{
    /// <summary>
    /// Describes a reference to a <see cref="IViewModel"/> member in the inspector.
    /// </summary>
    /// <remarks>
    /// This type is a wrapper around the stuff we need to store on a component so we can look up the view model and field at runtime.
    /// Using this wrapper lets us write a nice picker for view models in the inspector.
    /// </remarks>
    [Serializable]
    public class InspectorBindingDataSource
    {
        [SerializeField] internal string _typeAssemblyQualifiedName;
        [SerializeField] internal string _memberName;
    }

    /// <summary>
    /// Derive from this class to create inspector bindings to properties on your view models.
    /// </summary>
    /// <typeparam name="T">The type of the property to bind to.</typeparam>
    public abstract class InspectorBinding<T> : MonoBehaviour, IViewModelBinding
    {
        #region Inspector Fields
        [SerializeField]
        [Tooltip("Should this component be bound dynamically at runtime? e.g. for binding scroll list items that we generate on the fly.")]
        private bool _isRuntimeBinding;

        [SerializeField]
        [Tooltip("Set the data this component should bind to.")]
        private InspectorBindingDataSource _dataSource;

        [SerializeField]
        [Tooltip("Use this to have other GameObjects react when the bound data changes.")]
        private UnityEvent<T> _targetAction;
        #endregion

        #region Private fields
        private IViewModel _viewModel;
        private Property<T> _viewModelProperty;
        private bool _didCallStart = false;
        private bool _didSubscribe = false;
        #endregion

        #region Private properties
        private IViewModel ViewModel
        {
            get
            {
                if (_viewModel != null)
                {
                    return _viewModel;
                }

                if (!_isRuntimeBinding)
                {
                    var viewModelType = Type.GetType(_dataSource._typeAssemblyQualifiedName);
                    _viewModel = (IViewModel)Scope.Current.Pull(viewModelType);
                }

                return _viewModel;
            }
            set
            {
                if (_viewModel == value)
                {
                    return;
                }

                if (gameObject.activeInHierarchy)
                {
                    TryUnsubscribe();
                }

                _viewModel = value;

                // We need to clear this out, so the next subscribe will find it on the new view model.
                _viewModelProperty = null;

                if (gameObject.activeInHierarchy)
                {
                    TrySubscribe();
                }
            }
        }

        protected Property<T> ViewModelProperty
        {
            get
            {
                if (_viewModelProperty == null)
                {
                    _viewModelProperty = ViewModel.FindObservableProperty<T>(_dataSource._memberName);
                }

                return _viewModelProperty;
            }
        }
        #endregion

        #region IViewModelBinding
        void IViewModelBinding.TrySetViewModel(IViewModel viewModel, Type viewModelType, string serializedViewModelTypeName)
        {
            if (_isRuntimeBinding && _dataSource._typeAssemblyQualifiedName == serializedViewModelTypeName)
            {
                ViewModel = viewModel;
            }
        }
        #endregion

        #region Unity events
        protected virtual void Awake()
        {

        }
        protected virtual void Start()
        {
            _didCallStart = true;
            TrySubscribe();
        }

        protected virtual void OnEnable()
        {
            TrySubscribe();
        }

        protected virtual void OnDisable()
        {
            TryUnsubscribe();
        }
        #endregion

        #region Virtual methods
        protected virtual void OnChangeEvent()
        {
            var value = ViewModelProperty.Value;
            _targetAction?.Invoke(value);
        }
        #endregion

        #region Private methods
        private void TrySubscribe()
        {
            if (!_didSubscribe && _didCallStart)
            {
                _didSubscribe = true;
                ViewModelProperty.ChangeEvent += OnChangeEvent;
            }
        }

        private void TryUnsubscribe()
        {
            if (_didSubscribe)
            {
                _didSubscribe = false;
                ViewModelProperty.ChangeEvent -= OnChangeEvent;
            }
        }
        #endregion
    }

    /// <summary>
    /// Derive from this class to provide bindings that convert types for target actions.
    /// </summary>
    /// <remarks>
    /// Your derived type should override <see cref="OnChangeEvent"/> and convert the value from <see cref="ViewModelProperty"/> to the desired type.
    /// </remarks>
    /// <typeparam name="TFrom">The type of the data source value.</typeparam>
    /// <typeparam name="TTo">The type of the target action value.</typeparam>
    public abstract class InspectorBinding<TFrom, TTo> : InspectorBinding<TFrom>
    {
        #region Inspector fields
        [SerializeField]
        [Tooltip("Use this to have other GameObjects react when the bound data changes.")]
        private UnityEvent<TTo> _convertedTargetAction;
        #endregion

        #region Inherited methods
        protected override void OnChangeEvent()
        {
            base.OnChangeEvent();
            var convertedValue = ConvertValue();
            _convertedTargetAction?.Invoke(convertedValue);
        }
        #endregion

        #region Virtual methods
        /// <summary>
        /// Convert the <see cref="ViewModelProperty"/> value from <see cref="TFrom"/> to <see cref="TTo"/>.
        /// </summary>
        protected abstract TTo ConvertValue();
        #endregion
    }
}