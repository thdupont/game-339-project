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

        public BattleViewModel()
        {
            _model = new BattleContext(this);
        }

        public void AddParticipants(params JRPGCharacter[] characters)
        {
            var list = Participants.Value.ToList();
            list.AddRange(characters.Select(it => new JrpgCharacterViewModel(it)));
            Participants.SetValue(list);
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