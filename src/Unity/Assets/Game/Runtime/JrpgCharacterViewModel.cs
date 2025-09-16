using Binding.Runtime;
using JRPG.Services;
using Observable.Runtime;
using Observable.Runtime.MutableProperty;

namespace Game.Runtime
{
    public class JrpgCharacterViewModel : IViewModel
    {
        public Property<string> Name;
        public Property<int> Hp;

        private readonly JRPGCharacter _model;

        public JrpgCharacterViewModel(JRPGCharacter model)
        {
            _model = model;

            Name = new(_model.Name);

            Hp = new(_model.HP);
            _model.HpChangeEvent += OnHpChange;
        }

        private void OnHpChange(int value)
        {
            Hp.SetValue(value);
        }
    }
}