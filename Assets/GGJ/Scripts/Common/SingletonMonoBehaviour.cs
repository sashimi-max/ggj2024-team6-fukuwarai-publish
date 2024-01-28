using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Common
{
    /// <summary>
    /// シングルトンクラス
    /// </summary>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        //protected override bool dontDestroyOnLoad { get { return true; } }
        protected abstract bool dontDestroyOnLoad { get; }

        private static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    System.Type t = typeof(T);
                    instance = (T)FindObjectOfType(t);
                    if (!instance)
                    {
                        Debug.LogError(t + " is nothing.");
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}