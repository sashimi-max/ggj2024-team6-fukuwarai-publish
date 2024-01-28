using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CrashEffectPool : MonoBehaviour
{
    [SerializeField] private CrashEffect goalEffectPrefab = default;

    ObjectPool<CrashEffect> pool;

    void Awake()
    {
        pool = new ObjectPool<CrashEffect>(OnCreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
    }

    CrashEffect OnCreatePooledObject()
    {
        return Instantiate(goalEffectPrefab, transform);
    }

    void OnGetFromPool(CrashEffect effect)
    {
        effect.gameObject.SetActive(true);
    }

    void OnReleaseToPool(CrashEffect effect)
    {
        effect.gameObject.SetActive(false);
    }

    void OnDestroyPooledObject(CrashEffect effect)
    {
        Destroy(effect.gameObject);
    }

    public async UniTask SpawnEffect(Vector2 position)
    {
        CrashEffect effect = pool.Get();
        effect.transform.position = position;

        await effect.Play();
        ReleaseGameObject(effect);
    }

    public void ReleaseGameObject(CrashEffect obj)
    {
        pool.Release(obj);
    }
}
