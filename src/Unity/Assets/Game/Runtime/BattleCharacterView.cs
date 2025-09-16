using Binding.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class BattleCharacterView : BaseDynamicReactiveView<JrpgCharacterViewModel>
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _hpLabel;
        [SerializeField] private TextMeshProUGUI _nameLabel;

        public void ToggleSelected()
        {
            _viewModel.ToggleSelected();
        }

        protected override void Subscribe()
        {
            _viewModel.Hp.ChangeEvent += OnHpChange;
            _viewModel.Name.ChangeEvent += OnNameChange;
            _viewModel.Selected.ChangeEvent += OnSelectedChange;
        }

        protected override void Unsubscribe()
        {
            _viewModel.Hp.ChangeEvent -= OnHpChange;
            _viewModel.Name.ChangeEvent -= OnNameChange;
            _viewModel.Selected.ChangeEvent -= OnSelectedChange;
        }

        private void OnHpChange()
        {
            _hpLabel.text = _viewModel.Hp.Value.ToString();
        }

        private void OnNameChange()
        {
            _nameLabel.text = _viewModel.Name.Value;
        }

        private void OnSelectedChange()
        {
            _backgroundImage.color = _viewModel.Selected.Value ? Color.lightGoldenRodYellow : Color.white;
        }
    }
}