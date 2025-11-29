using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]public class Magnification : MonoBehaviour
    {
        [Header("Greatî{ó¶"), SerializeField] protected float great = 2f;
        [Header("Goodî{ó¶"), SerializeField] protected float good = 1f;
        [Header("badî{ó¶"), SerializeField] protected float bad = 0.5f;
    }

public class PieceScript : Magnification
{
    [Header("êÅÇ´îÚÇŒÇ∑äÓëbà–óÕ"), SerializeField] Vector3 baseForce = new Vector3(1f, 1f, 0f);
    [Header("âÒì]"), SerializeField] Vector3 baseTorque = new Vector3(0f, 0f, 30f);
    [SerializeField] Rigidbody rb;

    float destroySecond = 4f;

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds());
    }

    public void AddForceByJudge(JudgeType judgeType, bool isRight)
    {
        if(!isRight)
        {
            baseForce.x *= -1;
        }

        baseForce.x += Random.Range(0, 3);
        baseForce.y += Random.Range(0, 2);

        switch(judgeType)
        {
            case JudgeType.great:
                rb.AddForce(baseForce * great, ForceMode.Impulse);
                rb.AddTorque(baseTorque * great, ForceMode.Impulse);
                break;
            case JudgeType.good:
                rb.AddForce(baseForce * good,ForceMode.Impulse);
                rb.AddTorque(baseTorque * good, ForceMode.Impulse);
                break;
            case JudgeType.bad:
                rb.AddForce(baseForce * bad, ForceMode.Impulse);
                rb.AddTorque(baseTorque * bad, ForceMode.Impulse);
                break;
            default:
                break;
        }
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(destroySecond);
        Destroy(gameObject);
    }
}
