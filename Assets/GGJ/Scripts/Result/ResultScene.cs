using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using TMPro;
using TransitionsPlus;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks.Linq;

namespace GGJ
{
    /// <summary>
    /// リザルトシーン
    /// </summary>
    public class ResultScene : MonoBehaviour
    {
        [SerializeField, Tooltip("タイトルに戻るボタン")]
        private Button _returnButton;

        [SerializeField, Tooltip("もう一回ボタン")]
        private Button _retryButton;

        [SerializeField, Tooltip("ノーマルリザルト")]
        private GameObject _nomalLayer;

        [SerializeField, Tooltip("ウルフレイヤー")]
        private GameObject _wolfLayer;

        [SerializeField]
        private GameObject _credit;

        [SerializeField, Tooltip("NEXTボタン")]
        private Button _nextButton;

        // [SerializeField, Tooltip("インフォボタン")]
        // private TextMeshProUGUI  _infoText;

        [SerializeField]
        private GameObject _mainBtnInfo;

        [SerializeField]
        private GameObject _wwin;

        [SerializeField]
        private GameObject _nwin;

        [SerializeField]
        private GameObject _p1w;

        [SerializeField]
        private GameObject _p2w;

        [SerializeField]
        private GameObject _p3w;

        [SerializeField]
        private GameObject _p4w;

        [SerializeField, Tooltip("P1ボタン")]
        private Button _p1Button;

        [SerializeField, Tooltip("P2ボタン")]
        private Button _p2Button;

        [SerializeField, Tooltip("P3ボタン")]
        private Button _p3Button;

        [SerializeField, Tooltip("P4ボタン")]
        private Button _p4Button;

        // InputSystem
        private FukuwaraiControls _fukuwaraiControls;
        private CancellationToken _cancellationToken;
        private bool isLoaded = false;
        private bool isMover = false;

        private void Awake()
        {
            _fukuwaraiControls = new FukuwaraiControls();
            _fukuwaraiControls.Enable();

            _wwin.SetActive(false);
            _nwin.SetActive(false);
            _p1w.SetActive(false);
            _p2w.SetActive(false);
            _p3w.SetActive(false);
            _p4w.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            SEManager.Instance.Play(SEPath.SE_RESULT);

            var isWolf = TitleScene.PlayerStateList.Any(x => x);
            Debug.Log($"Mode IsWolf:{isWolf}");

            _nomalLayer.SetActive(!isWolf);
            _wolfLayer.SetActive(false);

            if (!isWolf)
            {
                // ボタン選択状態
                EventSystem.current.SetSelectedGameObject(_returnButton.gameObject);

                // タイトル戻るボタン
                _returnButton.OnClickAsAsyncEnumerable()
                    .SubscribeAwait(async _ =>
                    {
                        isMover = true;
                        // SceneManager.LoadScene("Game");
                        SEManager.Instance.Play(SEPath.SE_RETURN_TITLE);
                        await UniTask.WaitForSeconds(2.1f);
                        SceneTitleLoad();
                    })
                    .AddTo(gameObject);
                // もう一回ボタン
                _retryButton.OnClickAsAsyncEnumerable()
                    .SubscribeAwait(async _ =>
                    {
                        isMover = true;
                        // SceneManager.LoadScene("Game");
                        SEManager.Instance.Play(SEPath.SE_RESTART);
                        await UniTask.WaitForSeconds(2.9f);
                        SceneReLoad();
                    })
                    .AddTo(gameObject);
                _fukuwaraiControls.UI.C.canceled += ChangeCredit;

                ResultAsync(_cancellationToken).Forget();
            }
            else
            {
                _cancellationToken = new CancellationToken();
                WolfResultAsync(_cancellationToken).Forget();
            }
        }

        private void OnDestroy()
        {
            _fukuwaraiControls.UI.C.canceled -= ChangeCredit;
        }

        private void ChangeCredit(InputAction.CallbackContext content)
        {
            if (isMover)
            {
                return;
            }
            _credit.SetActive(!_credit.activeSelf);
        }

        private void SceneReLoad()
        {
            if (isLoaded) return;
            isLoaded = true;
            SceneManager.LoadScene("Game");
        }

        private void SceneTitleLoad()
        {
            if (isLoaded) return;
            isLoaded = true;
            SceneManager.LoadScene("Title");
        }

        private async UniTask ResultAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _fukuwaraiControls.UI.Move.OnPerformedAsync<Vector2>(_cancellationToken);
            if (isLoaded || isMover) return;

            if (result.x > 0)
            {
                SEManager.Instance.Play(SEPath.SE_RESTART);
                await UniTask.WaitForSeconds(3.6f);
                SceneReLoad();
            }
            else
            {
                SEManager.Instance.Play(SEPath.SE_RETURN_TITLE);
                await UniTask.WaitForSeconds(2.3f);
                SceneTitleLoad();
            }
        }

        private async UniTask WolfResultAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _nomalLayer.SetActive(true);
            await UniTask.WaitForSeconds(3);
            _wolfLayer.SetActive(true);

            // _infoText.SetText("ウルフと思われる人を選択して下さい");
            _mainBtnInfo.SetActive(true);
            _nextButton.gameObject.SetActive(false);

            var bp1Event = _p1Button.onClick.GetAsyncEventHandler(cancellationToken);
            var bp2Event = _p2Button.onClick.GetAsyncEventHandler(cancellationToken);
            var bp3Event = _p3Button.onClick.GetAsyncEventHandler(cancellationToken);
            var bp4Event = _p4Button.onClick.GetAsyncEventHandler(cancellationToken);

            EventSystem.current.SetSelectedGameObject(_p1Button.gameObject);

            var selectIndex = await UniTask.WhenAny(
            bp1Event.OnInvokeAsync(),
            bp2Event.OnInvokeAsync(),
            bp3Event.OnInvokeAsync(),
            bp4Event.OnInvokeAsync());

            var isWolfLose = false;
            var wolfIndex = 0;
            for (var i = 0; i < 4; i++)
            {
                if (TitleScene.PlayerStateList[i])
                {
                    wolfIndex = i;
                }
                if (selectIndex == i && TitleScene.PlayerStateList[i])
                {
                    isWolfLose = true;
                }
            }

            _mainBtnInfo.SetActive(false);

            EventSystem.current.SetSelectedGameObject(_nextButton.gameObject);
            _nextButton.gameObject.SetActive(true);

            // var winner = isWolfLose? "ウルフ": "市民";
            // _infoText.SetText($"{winner}側の勝利です！\n\nウルフはPlayer{wolfIndex + 1}の方です。");

            _wwin.SetActive(!isWolfLose);
            _nwin.SetActive(isWolfLose);
            _p1w.SetActive(wolfIndex == 0);
            _p2w.SetActive(wolfIndex == 1);
            _p3w.SetActive(wolfIndex == 2);
            _p4w.SetActive(wolfIndex == 3);

            var nextEvent = _nextButton.onClick.GetAsyncEventHandler(cancellationToken);
            await UniTask.WhenAny(nextEvent.OnInvokeAsync());

            SceneTitleLoad();
        }
    }
}