using DependencyInjection.Runtime;
using UnityEngine;

namespace Game.Runtime
{
    /// <summary>
    /// Simple wrapper to make calling view model methods easier from the inspector.
    /// </summary>
    /// <remarks>
    /// In a real project, you could use Roslyn to generate this code, to save manual effort!
    /// </remarks>
    [CreateAssetMenu(fileName = "SampleModelBindingTarget", menuName = "Game/Runtime/Binding/SampleModel")]
    public class SampleModelBindingTarget : ScriptableObject
    {
        private Dependency<SampleModel> _viewModel;

        public void BumpGold()
        {
            _viewModel.Value.BumpGold();
        }
    }
}