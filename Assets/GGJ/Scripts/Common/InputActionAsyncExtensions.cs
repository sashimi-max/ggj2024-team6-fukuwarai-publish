using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GGJ
{
    public static class InputActionAsyncExtensions
    {
        #region 戻り値あり

        /// <summary>
        /// InputActionのstartedコールバックが実行されるまで待機する
        /// </summary>
        public static UniTask<T> OnStartedAsync<T>(
            this InputAction inputAction,
            CancellationToken ct = default
        ) where T : struct
        {
            return OnCallbackAsync<T>(
                ct,
                callback => inputAction.started += callback,
                callback => inputAction.started -= callback
            );
        }

        /// <summary>
        /// InputActionのperformedコールバックが実行されるまで待機する
        /// </summary>
        public static UniTask<T> OnPerformedAsync<T>(
            this InputAction inputAction,
            CancellationToken ct = default
        ) where T : struct
        {
            return OnCallbackAsync<T>(
                ct,
                callback => inputAction.performed += callback,
                callback => inputAction.performed -= callback
            );
        }

        /// <summary>
        /// InputActionのcanceledコールバックが実行されるまで待機する
        /// </summary>
        public static UniTask<T> OnCanceledAsync<T>(
            this InputAction inputAction,
            CancellationToken ct = default
        ) where T : struct
        {
            return OnCallbackAsync<T>(
                ct,
                callback => inputAction.canceled += callback,
                callback => inputAction.canceled -= callback
            );
        }

        #endregion

        #region 戻り値なし

        /// <summary>
        /// InputActionのstartedコールバックが実行されるまで待機する
        /// </summary>
        public static UniTask OnStartedAsync(
            this InputAction inputAction,
            CancellationToken ct = default
        )
        {
            return OnCallbackAsync(
                ct,
                callback => inputAction.started += callback,
                callback => inputAction.started -= callback
            );
        }

        /// <summary>
        /// InputActionのperformedコールバックが実行されるまで待機する
        /// </summary>
        public static UniTask OnPerformedAsync(
            this InputAction inputAction,
            CancellationToken ct = default
        )
        {
            return OnCallbackAsync(
                ct,
                callback => inputAction.performed += callback,
                callback => inputAction.performed -= callback
            );
        }

        /// <summary>
        /// InputActionのcanceledコールバックが実行されるまで待機する
        /// </summary>
        public static UniTask OnCanceledAsync(
            this InputAction inputAction,
            CancellationToken ct = default
        )
        {
            return OnCallbackAsync(
                ct,
                callback => inputAction.canceled += callback,
                callback => inputAction.canceled -= callback
            );
        }

        #endregion

        #region 共通ロジック

        private static UniTask<T> OnCallbackAsync<T>(
            CancellationToken ct,
            UnityAction<System.Action<InputAction.CallbackContext>> onAdd,
            UnityAction<System.Action<InputAction.CallbackContext>> onRemove
        ) where T : struct
        {
            // performedが通知されたら結果を確定させるUniTaskCompletionSource
            var tcs = new UniTaskCompletionSource<T>();

            // コールバックを受け取るローカル関数
            void OnCallback(InputAction.CallbackContext context)
            {
                // コールバックを解除
                onRemove?.Invoke(OnCallback);
                // UniTaskCompletionSourceに結果を設定
                tcs.TrySetResult(context.ReadValue<T>());
            }

            // コールバックを登録
            onAdd?.Invoke(OnCallback);

            // CancellationTokenがキャンセルされたときの処理
            ct.Register(() =>
            {
                // コールバックを解除
                onRemove?.Invoke(OnCallback);
                // UniTaskCompletionSourceをキャンセル
                tcs.TrySetCanceled();
            });

            // await可能なUniTaskを返す
            return tcs.Task;
        }

        private static UniTask OnCallbackAsync(
            CancellationToken ct,
            UnityAction<System.Action<InputAction.CallbackContext>> onAdd,
            UnityAction<System.Action<InputAction.CallbackContext>> onRemove
        )
        {
            // performedが通知されたら結果を確定させるUniTaskCompletionSource
            var tcs = new UniTaskCompletionSource();

            // コールバックを受け取るローカル関数
            void OnCallback(InputAction.CallbackContext context)
            {
                // コールバックを解除
                onRemove?.Invoke(OnCallback);
                // UniTaskCompletionSourceに結果を設定
                tcs.TrySetResult();
            }

            // コールバックを登録
            onAdd?.Invoke(OnCallback);

            // CancellationTokenがキャンセルされたときの処理
            ct.Register(() =>
            {
                // コールバックを解除
                onRemove?.Invoke(OnCallback);
                // UniTaskCompletionSourceをキャンセル
                tcs.TrySetCanceled();
            });

            // await可能なUniTaskを返す
            return tcs.Task;
        }

        #endregion
    }
}
