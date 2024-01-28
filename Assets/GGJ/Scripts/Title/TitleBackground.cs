using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GGJ
{
    public class TitleBackground : MonoBehaviour
    {
        
        [SerializeField, Tooltip("BGFrame1")] private Transform _titleBgFrame;

        [SerializeField] private float _rote = 720f;
        [SerializeField] private float _roteTime = 90f;
        
        // Start is called before the first frame update
        void Start()
        {
            _titleBgFrame.DOLocalRotate(new Vector3(0, 0, 720f), 90f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}