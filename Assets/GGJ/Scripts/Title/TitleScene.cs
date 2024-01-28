using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TransitionsPlus;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;

namespace GGJ
{
    /// <summary>
    /// タイトルシーン
    /// </summary>
    public class TitleScene : MonoBehaviour
    {
        public static List<bool> PlayerStateList = new List<bool>(4);

        [SerializeField, Tooltip("ゲーム開始ボタン")]
        private Button _startButton;

        [SerializeField, Tooltip("オプションボタン")]
        private Button _optionButton;

        [SerializeField, Tooltip("トランジションプロファイル")]
        private TransitionProfile _starTransitionProfile;

        [SerializeField]
        private GameObject _howTo;

        [SerializeField]
        private GameObject _howToW;
        [SerializeField]
        private GameObject _p1;
        [SerializeField]
        private GameObject _p2;
        [SerializeField]
        private GameObject _p3;
        [SerializeField]
        private GameObject _p4;
        [SerializeField]
        private GameObject _wolfN;
        [SerializeField]
        private GameObject _wolfW;
        [SerializeField]
        private GameObject _wolfS;
        [SerializeField, Tooltip("NEXTボタン")]
        private Button _nextButton;

        [SerializeField, Tooltip("タイトルレイヤー")]
        private GameObject _titleLayer;
        [SerializeField, Tooltip("ウルフレイヤー")]
        private GameObject _worfLayer;

        // InputSystem
        private FukuwaraiControls _fukuwaraiControls;

        private bool _startInputBlock = false;

        private bool _isHowTo = false;

        private void Awake()
        {
            _fukuwaraiControls = new FukuwaraiControls();
            _fukuwaraiControls.UI.Enter.canceled += (x) =>
            {
                _startButton.onClick.Invoke();
            };
            _fukuwaraiControls.UI.FireW.canceled += (x) =>
            {
                // Wolf Mode
                _optionButton.onClick.Invoke();
            };
            _fukuwaraiControls.UI.FireI.canceled += (x) =>
            {
                StartGame2();
            };
            _fukuwaraiControls.Enable();

            PlayerStateList = new List<bool>(4);
            for (int i = 0, count = 4; i < count; i++)
            {
                PlayerStateList.Add(false);
            }

            _titleLayer.SetActive(true);
            _worfLayer.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            // BGM再生
            BGMManager.Instance.Play(BGMPath.BGM_TITLE, delay: 3.0f, isLoop: true);

            SEManager.Instance.Play(SEPath.SE_TITLE);

            // スタートボタン選択状態
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);

            // 開始ボタン
            _startButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (!_isHowTo)
                    {
                        SEManager.Instance.Play(SEPath.SE_TITLE_START_BUTTON);
                        _howTo.SetActive(true);
                        _isHowTo = true;
                        return;
                    }

                    StartGame();
                })
                .AddTo(gameObject);
            // オプションボタン
            _optionButton.OnClickAsAsyncEnumerable()
                .SubscribeAwait(async (unit, token) =>
                {
                    if (_isHowTo)
                    {
                        return;
                    }

                    await WolfCheckAsync(token);
                })
                .AddTo(gameObject);
        }

        private void StartGame()
        {
            if (_startInputBlock)
            {
                return;
            }
            _startInputBlock = true;
            SEManager.Instance.Play(SEPath.SE_CLOSE_RULE);
            TransitionAnimator.Start(_starTransitionProfile, sceneNameToLoad: "Game");
        }

        private void StartGame2()
        {
            if (_startInputBlock)
            {
                return;
            }
            _startInputBlock = true;
            SEManager.Instance.Play(SEPath.SE_CLOSE_RULE);
            TransitionAnimator.Start(_starTransitionProfile, sceneNameToLoad: "Game2");
        }

        private async UniTask WolfCheckAsync(CancellationToken cancellationToken)
        {
            if (_startInputBlock)
            {
                return;
            }

            SEManager.Instance.Play(SEPath.SE_WOLF_START);
            _startInputBlock = true;
            cancellationToken.ThrowIfCancellationRequested();

            _titleLayer.SetActive(false);
            _worfLayer.SetActive(true);

            var rindex = UnityEngine.Random.Range(0, 4);
            PlayerStateList[rindex] = true;

            EventSystem.current.SetSelectedGameObject(_nextButton.gameObject);

            _howToW.SetActive(true);

            var step0Event = _nextButton.onClick.GetAsyncEventHandler(cancellationToken);
            await UniTask.WhenAny(step0Event.OnInvokeAsync());

            _howToW.SetActive(false);

            await OneCheckAsync(cancellationToken, "Player1", 0, _p1);
            await OneCheckAsync(cancellationToken, "Player2", 1, _p2);
            await OneCheckAsync(cancellationToken, "Player3", 2, _p3);
            await OneCheckAsync(cancellationToken, "Player4", 3, _p4);

            async UniTask OneCheckAsync(CancellationToken cancellationToken, string playerName, int index, GameObject playerInfo)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _p1.SetActive(false);
                _p2.SetActive(false);
                _p3.SetActive(false);
                _p4.SetActive(false);
                _wolfN.SetActive(false);
                _wolfW.SetActive(false);

                playerInfo.SetActive(true);

                var step1Event = _nextButton.onClick.GetAsyncEventHandler(cancellationToken);
                await UniTask.WhenAny(step1Event.OnInvokeAsync());
                playerInfo.SetActive(false);
                if (PlayerStateList[index])
                {
                    _wolfW.SetActive(true);
                }
                else
                {
                    _wolfN.SetActive(true);
                }


                var step2Event = _nextButton.onClick.GetAsyncEventHandler(cancellationToken);
                await UniTask.WhenAny(step2Event.OnInvokeAsync());
                _wolfN.SetActive(false);
                _wolfW.SetActive(false);
            }

            _wolfS.SetActive(true);

            var stepEndEvent = _nextButton.onClick.GetAsyncEventHandler(cancellationToken);
            await UniTask.WhenAny(stepEndEvent.OnInvokeAsync());

            _startInputBlock = false;

            StartGame();
        }
    }
}