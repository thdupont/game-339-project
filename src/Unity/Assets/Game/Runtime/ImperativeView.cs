using DependencyInjection.Runtime;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class ImperativeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;

        private Dependency<SampleModel> _viewModel;

        private void Start()
        {
            UpdateLabel();
        }

        public void OnClick()
        {
            _viewModel.Value.BumpGold();
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            _label.text = _viewModel.Value.Gold.Value.ToString();
        }
    }
}