using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;

    [Header("Debug Menu")]
    public bool respawnCharacter;


    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("States")]
    public IdleState idle;
    public PursueTargetState pursueTarget;
    public CombatStanceState combatStance;
    public AttackState attack;
    public DeadState dead;
    public ToughnessBrokenState toughnessBrokenState;
    public ForeverSpinState foreverSpin;


    protected override void Awake()
    {
        base.Awake();

        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        // Make a copy so the original is not modified
        idle = Instantiate(idle);
        pursueTarget = Instantiate(pursueTarget);

        if (currentState as ForeverSpinState != null)
        {
            foreverSpin = Instantiate(foreverSpin);
            currentState = foreverSpin;
            return;
        }
        currentState = idle;
    }

    protected override void Update()
    {
        base.Update();

        aiCharacterCombatManager.HandleActionRecovery(this);

        DebugMenu();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        aiCharacterNetworkManager.currentHealth.OnValueChanged += aiCharacterNetworkManager.CheckHP;
        aiCharacterNetworkManager.currentToughness.OnValueChanged += aiCharacterNetworkManager.CheckToughness;

        aiCharacterNetworkManager.isLockedOn.OnValueChanged += aiCharacterNetworkManager.OnIsLockedOnChanged;
        aiCharacterNetworkManager.currentTargetNetworkObjectID.OnValueChanged += aiCharacterNetworkManager.OnLockOnTargetIDChange;
    }
    private void ProcessStateMachine()
    {
        
        AIState nextState = currentState?.Tick(this);
        if (nextState != null)
        {
            currentState = nextState;
        }

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (aiCharacterCombatManager.currentTarget != null && !aiCharacterCombatManager.currentTarget.isDead.Value) {
            aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            //aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                aiCharacterNetworkManager.isMoving.Value = true;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false;
            }
        }
        else
        {
            aiCharacterNetworkManager.isMoving.Value = false;
        }
    }

    // Provoked if attacked
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0 || other == null) {
            return;
        }
        Debug.Log("Calling provocation");
        DamageCollider attacker = other.GetComponent<DamageCollider>();

        if (attacker != null) {
            Debug.Log("Detected hitbox");
            CharacterManager possibleTarget = attacker.gameObject.GetComponentInParent<CharacterManager>();
            if (possibleTarget != null && possibleTarget.gameObject.layer != gameObject.layer)
            {
                aiCharacterCombatManager.currentTarget = possibleTarget;
            }
            else {
                Debug.Log("Detected hazard");
            }
        }
        
    }

    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            isDead.Value = false;
            aiCharacterNetworkManager.currentHealth.Value = aiCharacterNetworkManager.maxHealth.Value;
            // Play VFX and exit death state
            characterAnimatorManager.PlayTargetActionAnimation("Idle", false, false, true, true);

        }
    }
}
