using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AIAgentConfig : ScriptableObject
{
    //stores public versions of variables hidden away in AIState
    //public float maxDistance = 2.0f;
    //public float maxSightDistance = 5.0f;
    //public float maxChaseDistance = 100.0f; // distance the enemy can chase player, if payer goes out of range, enemy goes back to idle state
    public float dieForce = 10.0f;
    public float speed = 2f;

    public Vector2 shootSprayRadius;
}
