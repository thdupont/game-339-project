using DependencyInjection.Runtime;
using UnityEngine;

namespace Game.Runtime
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private float _bumpIntervalSeconds = 4.0f;

        private Dependency<SampleModel> _sampleModel;

        private float _elapsedTime;

        void Awake()
        {
            var scope = Scope.Current;
            scope.RegisterSingleton(() => new SampleModel());
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _elapsedTime = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _bumpIntervalSeconds)
            {
                _sampleModel.Value.BumpGold();
                _elapsedTime = 0.0f;
            } 
        }
    }
}