using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChargeGage : MonoBehaviour
{
    [SerializeField] private AnimationCurve widthAnimationCurve;
    [SerializeField] private AnimationCurve heightAnimationCurve;

    [SerializeField] private Sprite lowerImage;
    [SerializeField] private Sprite higherImage;

    private PlayerInputManager playerInputManager;
    PlayerCharge playerCharge;

    private Image viewImage;
    private RectTransform view;

    private bool prevHigher = false;
    private bool isHigher = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCharge = GetComponentInParent<PlayerCharge>();
        viewImage = GetComponentInChildren<Image>();
        view = viewImage.gameObject.GetComponent<RectTransform>();
        view.gameObject.SetActive(false);
        playerInputManager = GetComponentInParent<PlayerInputManager>();
        playerInputManager.OnPressedFireButton
            .Subscribe(_ =>
            {
                SEManager.Instance.Play(SEPath.SE_GAUGE, isLoop: true);
                view.gameObject.SetActive(true);
            })
            .AddTo(this);

        playerInputManager.OnCanceledFireButton
            .Subscribe(_ => view.gameObject.SetActive(false))
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.gameObject.activeSelf) return;
        var width = widthAnimationCurve.Evaluate(playerCharge.normalizedChargedTime);
        var height = heightAnimationCurve.Evaluate(playerCharge.normalizedChargedTime);
        view.sizeDelta = new Vector2(width, height);
        isHigher = playerCharge.normalizedChargedTime >= 0.5f;


        if (isHigher != prevHigher)
        {
            if (isHigher)
            {
                viewImage.sprite = higherImage;
            }
            else
            {
                viewImage.sprite = lowerImage;
            }
            prevHigher = isHigher;
        }
        //v.value = playerCharge.normalizedChargedTime;
    }
}
