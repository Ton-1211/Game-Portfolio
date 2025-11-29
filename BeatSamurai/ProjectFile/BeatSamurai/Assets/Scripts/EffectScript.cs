using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    
    public void OnAnimatorEnd()
    {
        Destroy(gameObject);
    }
}
