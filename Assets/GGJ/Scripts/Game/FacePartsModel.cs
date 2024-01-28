using GGJ.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GGJ.Game
{
    [RequireComponent(typeof(RectTransform), typeof(Rigidbody2D))]
    public class FacePartsModel : MonoBehaviour
    {
        private PolygonCollider2D polygonCollider2D;
        private RectTransform rectTransform;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            rb = GetComponent<Rigidbody2D>();
            var playerType = GetComponentInParent<PlayerInputManager>().playerType;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (SceneManager.GetActiveScene().name != "Game2")
            {
                switch (playerType)
                {
                    case PlayerType.Player1:
                        break;
                    case PlayerType.Player2:
                        spriteRenderer.gameObject.transform.LookAt(Vector3.left);
                        break;
                    case PlayerType.Player3:
                        spriteRenderer.gameObject.transform.LookAt(Vector3.down);
                        break;
                    default:
                        spriteRenderer.gameObject.transform.LookAt(Vector3.right);
                        break;
                }
            }
            polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        public void Init(FacePartsData facePartsData)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            spriteRenderer.sprite = facePartsData.sprite;
            rb.drag = 5;

            gameObject.layer = LayerMask.NameToLayer(collidableObjectTypeName(facePartsData.collidableObjectType));
            if (facePartsData.collidableObjectType == CollidableObjectType.BlackEyeObject)
            {
                spriteRenderer.sortingOrder = 2;
            }

            var sprite = spriteRenderer.sprite;
            var physicsShapeCount = sprite.GetPhysicsShapeCount();

            polygonCollider2D.pathCount = physicsShapeCount;

            var physicsShape = new List<Vector2>();

            for (var i = 0; i < physicsShapeCount; i++)
            {
                physicsShape.Clear();
                sprite.GetPhysicsShape(i, physicsShape);
                var points = physicsShape.ToArray();
                polygonCollider2D.SetPath(i, points);
            }

            polygonCollider2D.enabled = false;
        }

        private string collidableObjectTypeName(CollidableObjectType collidableObjectType)
        {
            switch (collidableObjectType)
            {
                case CollidableObjectType.WhiteEyeObject:
                    return "WhiteEyeObject";
                case CollidableObjectType.BlackEyeObject:
                    return "BlackEyeObject";
                default:
                    return "EverythingCollidableObject";
            }
        }
    }
}
