using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using GGJ.Common;
using UniRx;
using KanKikuchi.AudioManager;
using UnityEngine.SceneManagement;

namespace GGJ.Game
{
    [RequireComponent(typeof(RectTransform))]
    public class FacePartsHolder : MonoBehaviour
    {
        [SerializeField] private GamePlayParameterAsset gamePlayParameter = default;

        private PlayerInputManager playerInputManager;

        RectTransform rectTransform;
        Sequence sequence;
        float width;

        // Start is called before the first frame update
        void Start()
        {
            playerInputManager = GetComponentInParent<PlayerInputManager>();
            var parentRectTransform = transform.parent.GetComponent<RectTransform>();
            width = parentRectTransform.sizeDelta.x;
            rectTransform = GetComponent<RectTransform>();

            if (SceneManager.GetActiveScene().name == "Game2")
            {
                sequence = DOTween.Sequence();
                sequence.Append(
                    parentRectTransform.DOLocalRotate(new Vector3(0, 0, 360), 3f, RotateMode.FastBeyond360)
                        .SetEase(Ease.Linear)
                ).SetLoops(-1).Play();
            }
            else
            {
                DoYoYo();
            }

            playerInputManager
                .OnPressedFireButton
                .Subscribe(_ =>
                {
                    SEManager.Instance.Play(AudioRandomContainer.Instance.RandomSE(SEPath.SE_FACE_SELECT1, SEPath.SE_FACE_SELECT2, SEPath.SE_FACE_SELECT3));
                    sequence.Pause();
                })
                .AddTo(this);

            playerInputManager
                .OnCanceledFireButton
                .Subscribe(_ => sequence.Play())
                .AddTo(this);
        }

        public void DoYoYo()
        {
            sequence = DOTween.Sequence();

            sequence
                .Append(rectTransform.DOAnchorPos(new Vector3(width, 0, 0), gamePlayParameter.playerBarMoveTime).SetEase(Ease.Linear))
                .Append(rectTransform.DOAnchorPos(new Vector3(0, 0, 0), gamePlayParameter.playerBarMoveTime).SetEase(Ease.Linear))
                .SetLoops(-1)
                .Play();
        }
    }
}
