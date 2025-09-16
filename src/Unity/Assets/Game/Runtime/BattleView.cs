using Binding.Runtime;
using DependencyInjection.Runtime;
using UnityEngine;

namespace Game.Runtime
{
    public class BattleView : BaseReactiveView
    {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private BattleCharacterView _characterPrefab;

        private Dependency<BattleViewModel> _viewModel;

        protected override void Subscribe()
        {
            _viewModel.Value.Participants.ChangeEvent += OnParticipantsChange;
        }

        protected override void Unsubscribe()
        {
            _viewModel.Value.Participants.ChangeEvent -= OnParticipantsChange;
        }

        private void OnParticipantsChange()
        {
            foreach (Transform kid in _contentRoot)
            {
                Destroy(kid.gameObject);
            }

            _contentRoot.DetachChildren();

            foreach (var participant in _viewModel.Value.Participants.Value)
            {
                var participantView = Instantiate(_characterPrefab, _contentRoot);
                participantView.gameObject.SetViewModelInChildren(participant);
            }
        }
    }
}