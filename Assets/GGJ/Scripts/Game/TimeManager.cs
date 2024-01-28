using GGJ.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace GGJ.Game
{
    public class TimeManager : SingletonMonoBehaviour<TimeManager>
    {
        protected override bool dontDestroyOnLoad { get { return false; } }

        [SerializeField] GamePlayParameterAsset gamePlayParameter = default;
        
        public IReadOnlyReactiveProperty<bool> IsTimeUp => _isTimeUp;
        private BoolReactiveProperty _isTimeUp = new BoolReactiveProperty(false);

        public float remainingTime;

        // Start is called before the first frame update
        void Start()
        {
            remainingTime = gamePlayParameter.remainingTime;
        }

        void Update()
        {
            if (_isTimeUp.Value) return;
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                _isTimeUp.Value = true;
            }
        }
    }
}
