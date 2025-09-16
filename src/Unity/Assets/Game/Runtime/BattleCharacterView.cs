using Binding.Runtime;
using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class BattleCharacterView : BaseDynamicReactiveView<JrpgCharacterViewModel>
    {
        [SerializeField] private TextMeshProUGUI _hpLabel;
        [SerializeField] private TextMeshProUGUI _nameLabel;

        protected override void Subscribe()
        {
            _viewModel.Hp.ChangeEvent += OnHpChange;
            _viewModel.Name.ChangeEvent += OnNameChange;
        }

        protected override void Unsubscribe()
        {
            _viewModel.Hp.ChangeEvent -= OnHpChange;
            _viewModel.Name.ChangeEvent -= OnNameChange;
        }

        private void OnHpChange()
        {
            _hpLabel.text = _viewModel.Hp.Value.ToString();
        }

        private void OnNameChange()
        {
            _nameLabel.text = _viewModel.Name.Value;
        }
    }
}