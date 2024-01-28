using Cysharp.Threading.Tasks;
using GGJ.Common;
using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GGJ.Game
{
    public class FacePartsSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<FacePartsHolder> facePartsHolders = new List<FacePartsHolder>();

        [SerializeField] FacePartsModel FacePartsPrefab = default;

        [SerializeField] Image wholeFaceImage = default;

        [SerializeField] FacePartsAsset facePartsAsset = default;

        [SerializeField] GameObject nomalBg = default;
        [SerializeField] GameObject resultBg = default;
        [SerializeField] GameObject buttons = default;

        private List<FacePartsMover> movers;
        private IEnumerable<PlayerInputManager> inputManagers;
        bool isGameOver = false;
        private FaceParts faceParts;

        private void Start()
        {
            BGMManager.Instance.Play(BGMPath.BGM_GAME, isLoop: true);

            var index = Random.Range(0, facePartsAsset.facePartsSet.Count);
            faceParts = facePartsAsset.facePartsSet[index];
            SpawnFaceParts();

            foreach (var input in inputManagers)
            {
                input.OnCanceledFireButton
                    .Where(_ => !input.isFired)
                    .Subscribe(async _ =>
                    {
                        await UniTask.WaitForSeconds(2.0f);
                        SpawnFacePartsBySide(input.playerType);
                    }).AddTo(this);
            }
        }

        public void SpawnFaceParts()
        {
            var firstEjectedParts = new List<FacePartsData>() {
                faceParts.downEjectedFacePartsData.First(),
                faceParts.rightEjectedFacePartsData.First(),
                faceParts.upEjectedFacePartsData.First(),
                faceParts.leftEjectedFacePartsData.First(),
            };
            wholeFaceImage.sprite = faceParts.wholeFaceSprite;
            for (var i = 0; i < facePartsHolders.Count; i++)
            {
                var obj = Instantiate(FacePartsPrefab, facePartsHolders[i].transform);
                obj.Init(firstEjectedParts[i]);
            }

            movers = facePartsHolders.Select(obj => obj.GetComponentInChildren<FacePartsMover>()).ToList();
            inputManagers = facePartsHolders.Select(obj => obj.GetComponentInParent<PlayerInputManager>());
        }

        private void SpawnFacePartsBySide(PlayerType playerType)
        {
            var holder = facePartsHolders[(int)playerType];
            var obj = Instantiate(FacePartsPrefab, holder.transform);
            FacePartsData secondEjectedPart;
            switch (playerType)
            {
                case PlayerType.Player1:
                    secondEjectedPart = faceParts.downEjectedFacePartsData.Last();
                    break;
                case PlayerType.Player2:
                    secondEjectedPart = faceParts.rightEjectedFacePartsData.Last();
                    break;
                case PlayerType.Player3:
                    secondEjectedPart = faceParts.upEjectedFacePartsData.Last();
                    break;
                default:
                    secondEjectedPart = faceParts.leftEjectedFacePartsData.Last();
                    break;
            }

            obj.Init(secondEjectedPart);
            movers.Add(holder.GetComponentInChildren<FacePartsMover>());
        }

        private void FixedUpdate()
        {
            if (isGameOver || movers == null || movers.Count() == 0 || inputManagers == null || inputManagers.Count() == 0) return;

            var allFireds = inputManagers.All(inputs => inputs.isFired);

            if (!allFireds) return;

            var moveEnds = movers.All(mover => mover.rb.velocity.magnitude < 0.1f);

            if (moveEnds)
            {
                gameOver();
            }
        }

        private void gameOver()
        {
            movers.ForEach(mover =>
            {
                mover.rb.velocity = Vector2.zero;
                mover.rb.angularVelocity = 0;
            });
            BGMManager.Instance.Stop();
            Debug.Log("gameover!");
            isGameOver = true;

            buttons.SetActive(false);
            nomalBg.SetActive(false);
            resultBg.SetActive(true);
            SceneManager.LoadScene("Result", LoadSceneMode.Additive);
        }
    }
}
