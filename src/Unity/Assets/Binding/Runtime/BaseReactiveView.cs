using UnityEngine;

namespace Binding.Runtime
{
    /// <summary>
    /// Handles the basic Unity lifecycle boilerplate for subscribe and unsubscribe.
    /// </summary>
    /// <remarks>
    /// Use this when your view takes care of getting its own view model.
    /// </remarks>
    public abstract class BaseReactiveView : MonoBehaviour
    {
        protected bool _didCallStart;
        protected bool _didSubscribe;

        protected virtual void Awake()
        {
            _didCallStart = false;
            _didSubscribe = false;
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

        protected abstract void Subscribe();

        protected abstract void Unsubscribe();

        protected virtual void TrySubscribe()
        {
            if (!_didSubscribe && _didCallStart)
            {
                _didSubscribe = true;
                Subscribe();
            }
        }

        protected virtual void TryUnsubscribe()
        {
            if (_didSubscribe)
            {
                _didSubscribe = false;
                Unsubscribe();
            }
        }
    }
}