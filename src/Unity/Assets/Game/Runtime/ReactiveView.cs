using DependencyInjection.Runtime;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class ReactiveView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;

        private Dependency<SampleModel> _viewModel;
        private bool _didCallStart;
        private bool _didSubscribe;

        private void Awake()
        {
            _didCallStart = false;
            _didSubscribe = false;
        }

        private void Start()
        {
            _didCallStart = true;
            TrySubscribe();
        }

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void OnDisable()
        {
            TryUnsubscribe();
        }

        private void TrySubscribe()
        {
            if (!_didSubscribe && _didCallStart)
            {
                _didSubscribe = true;
                _viewModel.Value.Gold.ChangeEvent += UpdateLabel;
            }
        }

        private void TryUnsubscribe()
        {
            if (_didSubscribe)
            {
                _didSubscribe = false;
                _viewModel.Value.Gold.ChangeEvent -= UpdateLabel;
            }
        }

        private void UpdateLabel()
        {
            _label.text = _viewModel.Value.Gold.Value.ToString();
        }
    }
}