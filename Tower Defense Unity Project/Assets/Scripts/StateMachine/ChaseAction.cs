using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : Action
{
    public override void Act(StateController controller)
    {
        Chase(controller);
    }

    public void Chase(StateController controller)
    {
        if (controller.chaseTarget != null)
        {
         
            if (controller.enemyStats.lookRange < 
                Vector3.Distance(
                    controller.Tower.transform.position, 
                    controller.Tower.targetEnemy.transform.position))
            {
                controller.chaseTarget = null;
                return;
            }
            ////controller.navMeshAgent.destination = controller.chaseTarget.position;
            //Vector3 dir = controller.chaseTarget.transform.position - controller.Tower.transform.position;

            //Quaternion lookRotation = Quaternion.LookRotation(dir * controller.enemyStats.searchingTurnSpeed * Time.deltaTime);

            //Vector3 turn = Quaternion.Lerp(controller.Tower.rig.rotation, lookRotation, Time.deltaTime * controller.enemyStats.searchingTurnSpeed).eulerAngles;

            //controller.Tower.rig.rotation = Quaternion.Euler(0f, turn.y, 0f);

            controller.Tower.LockOnTarget();
            // controller.Tower.UpdateLaserDirection();
            //Debug.Log("chaser");
            //controller.navMeshAgent.Resume();
            // Debug.Log("ChaseAction");
        }
    }
}
