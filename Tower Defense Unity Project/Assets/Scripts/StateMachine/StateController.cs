using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Complete;

public class StateController : MonoBehaviour
{

    public State currentState;

    public EnemyStats enemyStats;

    public Transform eyes;

    public State remainState;

    [HideInInspector]
    public Enemy chaseTarget;

    [HideInInspector]
    public float stateTimeElapsed;

    [HideInInspector]
    public Turret Tower;

    // public bool aiActive;


    void Awake()
    {
        Tower = GetComponent<Turret>();        
    }

    void Update()
    {

        currentState.UpdateState(this);
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, enemyStats.lookRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(eyes.position, enemyStats.attackRange);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
}
