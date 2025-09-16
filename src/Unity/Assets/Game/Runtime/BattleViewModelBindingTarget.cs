using DependencyInjection.Runtime;
using Game.Runtime;
using JRPG.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleViewModelBindingTarget", menuName = "Game/Runtime/Binding/BattleViewModel")]
public class BattleViewModelBindingTarget : ScriptableObject
{
    private Dependency<BattleViewModel> _viewModel;

    public void AddParticipant(TMPro.TMP_InputField inputField)
    {
        var name = inputField.text;

        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning($"{nameof(BattleViewModelBindingTarget)}|{nameof(AddParticipant)}|input name is empty");
            return;
        }

        _viewModel.Value.AddParticipants(new JRPGCharacter(name, 35, 17, 8, 3, 4, 2));
    }
}
