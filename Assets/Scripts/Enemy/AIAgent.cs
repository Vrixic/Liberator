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
public class AIAgent : ISpawnable
{
    public AIStateMachine stateMachine;
    public AIStateID initialState;
    public AIStateID currentState;
    [HideInInspector]public NavMeshAgent navMeshAgent;
    public AIAgentConfig config;
    [HideInInspector]public Transform playerTransform;
    [HideInInspector] public AiSensor sensor;
    [HideInInspector] public Animator animator;

    private Coroutine lookCouroutine;

    public bool isFlashed = false;

    private Health enemyHealth;

    private EnemyGun enemyGun;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //creates a new state machine for this agent type. 
        stateMachine = new AIStateMachine(this);
        
        sensor = GetComponent<AiSensor>();
        //adds the chase player to the enum for AIState
        stateMachine.RegisterState(new AIChasePlayerScript());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.RegisterState(new AIIdleState());
        stateMachine.RegisterState(new AIFlashState());
        stateMachine.RegisterState(new AIAttackPlayerState());
        stateMachine.RegisterState(new AIReturnState());
        enemyGun = GetComponent<EnemyGun>();
        enemyHealth = GetComponent<Health>();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        //sets the player transform to look for the player tagged object
        playerTransform = GameManager.Instance.player.transform;
        //constantly updates the machine
        stateMachine.Update();

        //animator.SetFloat("Speed", navMeshAgent.speed);
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    public override void Spawn()
    {
        base.Spawn();

        gameObject.SetActive(true);

        currentState = initialState;
        //sets state to initial state.
        stateMachine.ChangeState(initialState);
        enemyHealth.currentHealth = enemyHealth.maxHealth;        
    }

    public override void Despawn()
    {
        base.Despawn();

        gameObject.SetActive(false);
    }

    public override void Respawn()
    {
        base.Respawn();

        animator.SetBool("isDead", false);
        enemyGun.ResetGun();
        Spawn();
    }

    public void Rotating()
    {
        if(lookCouroutine != null)
        {
            StopCoroutine(lookCouroutine);
        }

        lookCouroutine = StartCoroutine(LookAt());
    }

    public IEnumerator LookAt()
    {
        Vector3 lookRotation = GameManager.Instance.playerTransform.position - transform.position;
        lookRotation.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookRotation);

        float time = 0;

        while(time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, time);

            time += Time.deltaTime * config.speed;

            yield return null;
        }

    }

    public EnemyGun GetGun()
    {
        return enemyGun;
    }
}
