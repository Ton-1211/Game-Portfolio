using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    PlayerBehaviorScript playerBehavior;
    // Start is called before the first frame update
    void Start()
    {
        playerBehavior = GetComponent<PlayerBehaviorScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
