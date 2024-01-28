using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Common;
using KanKikuchi.AudioManager;

namespace GGJ.Game
{
    public class AudioRandomContainer : Common.SingletonMonoBehaviour<AudioRandomContainer>
    {
        protected override bool dontDestroyOnLoad { get { return false; } }

        public string RandomSE(params string[] seNames)
        {
            var index = Random.Range(0, seNames.Length);
            return seNames[index];
        }
    }
}
