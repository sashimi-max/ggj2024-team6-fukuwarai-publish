using Cysharp.Threading.Tasks;
using GGJ.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerType playerType;

    public IObservable<Unit> OnPressedFireButton => _onPressedFireButton;
    private Subject<Unit> _onPressedFireButton = new Subject<Unit>();

    public IObservable<Unit> OnCanceledFireButton => _onCanceledFireButton;
    private Subject<Unit> _onCanceledFireButton = new Subject<Unit>();

    public bool isFired { get; private set; } = false;
    private bool canPressedButton = true;
    private float reenabledTimer = 0.0f;

    private const int ABLE_FIRE_COUNT = 2;
    private int currentFireCount = 0;

    private void Awake()
    {
        var inputActions = new FukuwaraiControls();

        switch (playerType)
        {
            case PlayerType.Player1:
                inputActions.Game.Fire.performed += OnFireButtonDown;
                inputActions.Game.Fire.canceled += OnFireButtonUp;
                break;
            case PlayerType.Player2:
                inputActions.Game.Fire2.performed += OnFireButtonDown;
                inputActions.Game.Fire2.canceled += OnFireButtonUp;
                break;
            case PlayerType.Player3:
                inputActions.Game.Fire3.performed += OnFireButtonDown;
                inputActions.Game.Fire3.canceled += OnFireButtonUp;
                break;
            default:
                inputActions.Game.Fire4.performed += OnFireButtonDown;
                inputActions.Game.Fire4.canceled += OnFireButtonUp;
                break;
        }
        inputActions.Enable();
    }

    private void Update()
    {
        if (!canPressedButton)
        {
            reenabledTimer += Time.deltaTime;
            if (reenabledTimer > 2.0f)
            {
                canPressedButton = true;
            }
        }
    }


    private void OnFireButtonUp(InputAction.CallbackContext context)
    {
        if (isFired || !canPressedButton) return;
        canPressedButton = false;
        reenabledTimer = 0.0f;
        currentFireCount++;
        if (currentFireCount == ABLE_FIRE_COUNT)
        {
            isFired = true;
        }

        _onCanceledFireButton.OnNext(default);
    }

    private void OnFireButtonDown(InputAction.CallbackContext context)
    {
        if (isFired || !canPressedButton) return;
        _onPressedFireButton.OnNext(default);
    }
}
