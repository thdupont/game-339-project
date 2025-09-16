using System.Collections.Generic;
using System.Linq;
using Binding.Runtime;
using JRPG.Core;
using JRPG.Services;
using Observable.Runtime;
using Observable.Runtime.MutableProperty;
using UnityEngine;

namespace Game.Runtime
{
    public class BattleViewModel : ILog, IViewModel
    {
        private BattleContext _model;

        public Property<List<JrpgCharacterViewModel>> Participants = new(new List<JrpgCharacterViewModel>());

        public Property<JrpgCharacterViewModel> SelectedTarget = new();

        public BattleViewModel()
        {
            _model = new BattleContext(this);

            Participants.ChangeEvent += OnParticipantsChange;
        }

        public void AddParticipants(params JRPGCharacter[] characters)
        {
            var list = Participants.Value.ToList();
            list.AddRange(characters.Select(it => new JrpgCharacterViewModel(this, it)));
            Participants.SetValue(list);

            _model.Participants.Clear();
            _model.Participants.AddRange(characters);
        }

        public void SelectTarget(JrpgCharacterViewModel target)
        {
            SelectedTarget.SetValue(target);
        }

        public void AttackSelectedTarget()
        {
            if (SelectedTarget.Value == null)
            {
                Debug.LogWarning($"{nameof(BattleViewModel)}|{nameof(AttackSelectedTarget)}|target is null");
                return;
            }

            _model.Participants[0].AttackTarget(SelectedTarget.Value.Model);
        }

        private void OnParticipantsChange()
        {
            _model.Participants.Clear();
            _model.Participants.AddRange(Participants.Value.Select(it => it.Model));
        }

        void ILog.Critical(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        void ILog.Debug(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        void ILog.Error(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        void ILog.Information(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        void ILog.Trace(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        void ILog.Warning(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }
    }
}