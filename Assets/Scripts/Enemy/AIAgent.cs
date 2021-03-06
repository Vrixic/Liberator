using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(AIStateMachine))]
[RequireComponent(typeof(Transform))]
//[RequireComponent(typeof(AIAgentConfig))]
[RequireComponent(typeof(AiSensor))]
[RequireComponent(typeof(Animator))]
public class AIAgent : MonoBehaviour
{
    public AIStateMachine stateMachine;
    public Vector3 initialPosition;
    public Vector3 initialRotation;
    public AIStateID initialState;
    public AIStateID currentState;
    public Ragdoll ragdoll;
    [HideInInspector]public NavMeshAgent navMeshAgent;
    public AIAgentConfig config;
    [HideInInspector]public Transform playerTransform;
    [HideInInspector] public AiSensor sensor;
    [HideInInspector] public Animator animator;
    [SerializeField] Collider enemyTagCollider;
    [SerializeField] bool isMelee = false;
    public float sqrDistance;

    public string aiName = "";

    [HideInInspector] public Vector3 aimDirection;

    [SerializeField] [Tooltip("How long should it wait to clean up the enemy's body after death")]float disableEnemyInterval = 2f;

    private Coroutine lookCouroutine;

    public bool alreadyNavigatingToPlayer = false;

    public bool isFlashed = false;
    public bool isStunned = false;

    public bool isInHitReaction = false;

    public bool bFirstChase = true;

    private Health enemyHealth;

    private EnemyGun enemyGun;
    private EnemyMelee enemyMelee;

    [HideInInspector] public MiniMapScanable miniMapLocator;

    // Colliders 
    SphereCollider headShotCollider;
    public Headshot_Hitbox headshot;
    CapsuleCollider bodyCollider;
    BoxCollider boxCollider;

    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();
        //creates a new state machine for this agent type. 
        stateMachine = new AIStateMachine(this);

        miniMapLocator = GetComponent<MiniMapScanable>();

        sensor = GetComponent<AiSensor>();
        //adds the chase player to the enum for AIState
        stateMachine.RegisterState(new AIChasePlayerScript());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.RegisterState(new AIIdleState());
        stateMachine.RegisterState(new AIFlashState());
        stateMachine.RegisterState(new AIAttackPlayerState());
        stateMachine.RegisterState(new AIReturnState());
        stateMachine.RegisterState(new AIAlertedState());
        enemyMelee = GetComponent<EnemyMelee>();
        if (!isMelee)
        {
            enemyGun = GetComponent<EnemyGun>();
        }
        enemyHealth = GetComponent<Health>();

        currentState = initialState;
        //sets state to initial state.
        stateMachine.ChangeState(initialState);

        if (!(transform.parent.CompareTag("Juggernaut")))
        {
            headShotCollider = GetComponentInChildren<SphereCollider>();
            headshot = GetComponentInChildren<Headshot_Hitbox>();
        }
        bodyCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponentInChildren<BoxCollider>();

        isDead = false;
        enemyHealth.currentHealth = enemyHealth.maxHealth;

        EnableColliders();

        //store where the enemy is located initially in the scene
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //constantly updates the machine
        stateMachine.Update();
        sqrDistance = (GameManager.Instance.playerTransform.position - transform.position).sqrMagnitude;
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    public void CS_ReturnToPost(Vector3 aimPosition)
    {
        aimDirection = aimPosition;
        stateMachine.ChangeState(AIStateID.Returning);
    }

    public void Rotating()
    {
        if(lookCouroutine != null)
        {
            StopCoroutine(lookCouroutine);
        }

        lookCouroutine = StartCoroutine(LookAt());
    }

    public IEnumerator WaitForReactionTime()
    {
        yield return new WaitForSeconds(0.2f);
        stateMachine.ChangeState(AIStateID.AttackPlayer);
    }

    //not implemented correctly yet
    //public void StartNavigatingToPlayer()
    //{
    //    if(!alreadyNavigatingToPlayer)
    //        StartCoroutine(NavigateToPlayer());
    //}

    //public void StopNavigatingToPlayer()
    //{
    //    StopCoroutine(NavigateToPlayer());

    //    alreadyNavigatingToPlayer = false;
    //}

    //private IEnumerator NavigateToPlayer()
    //{
    //    alreadyNavigatingToPlayer = true;

    //    navMeshAgent.destination = GameManager.Instance.playerTransform.position;
    //    yield return new WaitForSeconds(0.1f);
    //}

    void EnableColliders()
    {
        bodyCollider.enabled = true;
        if (!(transform.parent.CompareTag("Juggernaut")))
        {
            headShotCollider.enabled = true;
        }
        boxCollider.enabled = true;

        enemyTagCollider.enabled = true;
    }

    public void DisableColliders()
    {
        bodyCollider.enabled = false;
        if (!(transform.parent.CompareTag("Juggernaut")))
        {
            headShotCollider.enabled = false;
        }

        enemyTagCollider.enabled = false;
    }

    //public IEnumerator LookAt()
    //{
    //    Vector3 lookRotation = GameManager.Instance.playerTransform.position - transform.position;
    //    lookRotation.y = transform.position.y;
    //    Quaternion rot = Quaternion.LookRotation(lookRotation);

    //    float time = 0;

    //    while(time < 1)
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, rot, time);

    //        time += Time.deltaTime * config.speed;

    //        yield return null;
    //    }

    //}

    public IEnumerator LookAt()
    {
        Vector3 lookRotation = (GameManager.Instance.playerTransform.position - transform.position).normalized; // the direction vector, means it has to be normalized
        lookRotation.y = 0f; // we don't want enemy to look up or down
        //Debug.DrawLine(transform.position, transform.position + lookRotation * 10f, Color.red, 0.5f);
        Quaternion rot = Quaternion.LookRotation(lookRotation);

        float time = 1; // start time 

        while (time > 0) // end time
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, time);

            time -= Time.deltaTime * config.speed; // increases the rotation per frame depending on the speed

            yield return null;
        }
    }

    public void HitReacted()
    {
        isInHitReaction = true;
        Invoke("ExitHitReaction", 0.94f);

        navMeshAgent.isStopped = true;
    }

    public void ExitHitReaction()
    {
        isInHitReaction = false;

        if(!IsDead())
            navMeshAgent.isStopped = false;
    }

    public EnemyGun GetGun()
    {
        return enemyGun;
    }

    public EnemyMelee GetMeleeWeapon()
    {
        return enemyMelee;
    }

    public bool IsMelee()
    {
        return isMelee;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void SetIsDead(bool dead)
    {
        isDead = dead;
    }

    public float GetDisableEnemyInterval()
    {
        return disableEnemyInterval;
    }
}
