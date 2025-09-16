using DependencyInjection.Runtime;
using UnityEngine;

namespace Game.Runtime
{
    public class BattleBootstrap : MonoBehaviour
    {
        void Awake()
        {
            var scope = Scope.Current;
            scope.RegisterSingleton(() => new BattleViewModel());
        }
    }
}