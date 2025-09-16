using Binding.Runtime;
using JRPG.Services;
using Observable.Runtime;
using Observable.Runtime.MutableProperty;
using UnityEngine;

namespace Game.Runtime
{
    public class JrpgCharacterViewModel : IViewModel
    {
        private BattleViewModel _battleViewModel;
        public Property<string> Name;
        public Property<int> Hp;
        public Property<bool> Selected = new();

        public JRPGCharacter Model { get; private set; }

        public JrpgCharacterViewModel(BattleViewModel battleViewModel, JRPGCharacter model)
        {
            _battleViewModel = battleViewModel;
            Model = model;

            Name = new(Model.Name);

            Hp = new(Model.HP);
            Model.HpChangeEvent += OnHpChange;

            _battleViewModel.SelectedTarget.ChangeEvent += OnSelectedTargetChange;
        }

        public void ToggleSelected()
        {
            if (_battleViewModel.SelectedTarget.Value == this)
            {
                _battleViewModel.SelectTarget(null);
            }
            else
            {
                _battleViewModel.SelectTarget(this);
            }
        }

        private void OnHpChange(int value)
        {
            Hp.SetValue(value);
        }

        private void OnSelectedTargetChange()
        {
            Selected.SetValue(_battleViewModel.SelectedTarget.Value == this);
        }
    }
}