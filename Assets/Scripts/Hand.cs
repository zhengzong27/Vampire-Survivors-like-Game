using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;
    SpriteRenderer player;
    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);

    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance 为空！");
            return;
        }

        if (GameManager.instance.player == null)
        {
            Debug.LogError("GameManager.instance.player 为空！");
            return;
        }

        player = GameManager.instance.player.GetComponent<SpriteRenderer>();
        if (player == null)
        {
            Debug.LogError("无法获取玩家的 SpriteRenderer 组件！");
        }
        else
        {
            Debug.Log($"成功获取到玩家的 SpriteRenderer 组件，当前 flipX: {player.flipX}");
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        bool isReverse = player.flipX;
        Debug.Log($"当前 flipX: {player.flipX}, isReverse: {isReverse}, isLeft: {isLeft}");

        if (isLeft)
        {
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.sortingOrder = isReverse ? 4 : 6;
        }
        else
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 6 : 4;
        }
    }
}
