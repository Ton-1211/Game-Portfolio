using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviorScript : MonoBehaviour
{

    enum AttackDirection
    {
        up = 0,
        down = 1
    }

    [SerializeField] CheckAreaScript rightCheckArea;
    [SerializeField] CheckAreaScript leftCheckArea;
    [SerializeField] Animator bodyAnimator;
    [SerializeField] JudgeScript missPrefab;
    [SerializeField] PauseScript pauseMenu;

    ScoreScript scoreScript;
    string hurtableTag = "Hurtable";
    void Start()
    {
        scoreScript = FindObjectOfType<ScoreScript>();
    }

    public void Damage()
    {
        // ここに被ダメージモーションを挟む予定
    }

    public void OnRightAttack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                SetAttackDirection();
                bodyAnimator.SetTrigger("Right");
                rightCheckArea.Hit();
                break;
            default:
                break;
        }
    }

    public void OnLeftAttack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                SetAttackDirection();
                bodyAnimator.SetTrigger("Left");
                leftCheckArea.Hit();
                break;
            default:
                break;
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(!pauseMenu.gameObject.activeSelf)
            {
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.Pause();
            }
            else
            {
                pauseMenu.Resume();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == hurtableTag)// 敵からダメージを受けたとき
        {
            Instantiate(missPrefab, transform);
            scoreScript.ResetCombo();
            Destroy(other.gameObject);
        }
    }

    void SetAttackDirection()
    {
        AttackDirection attackDirection = Random.Range(0, 2) == 0 ? AttackDirection.up : AttackDirection.down;
        if(attackDirection == AttackDirection.up)
        {
            bodyAnimator.SetTrigger("SlashUp");
        }
        else
        {
            bodyAnimator.SetTrigger("SlashDown");
        }
    }
}
