using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CrashEffect : MonoBehaviour
{
    ParticleSystem particle;
    // Start is called before the first frame update
    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public async UniTask Play()
    {
        particle.Play();

        await UniTask.WaitForSeconds(particle.main.startLifetimeMultiplier);

        particle.Stop();
    }

}
