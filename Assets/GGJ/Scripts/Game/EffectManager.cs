using Cysharp.Threading.Tasks;
using GGJ.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Game
{
    public class EffectManager : SingletonMonoBehaviour<EffectManager>
    {
        protected override bool dontDestroyOnLoad { get { return false; } }

        [SerializeField] CrashEffectPool crashEffectPool = default;

        public void PlayCrashEffect(Vector2 position)
        {
            crashEffectPool.SpawnEffect(position).Forget();
        }
    }
}