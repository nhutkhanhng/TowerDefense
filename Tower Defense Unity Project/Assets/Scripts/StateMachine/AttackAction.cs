using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    private void Attack(StateController controller)
    {
        if (controller.chaseTarget != null)
        {
            if (controller.enemyStats.attackRange < Vector3.Distance(
                                                    controller.Tower.transform.position,
                                                        controller.Tower.targetEnemy.transform.position))
                return;
            RaycastHit hit;
            

            //Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.attackRange, Color.red);

            //if (Physics.BoxCast(controller.eyes.position + Vector3.down * controller.enemyStats.lookRange / 2, 
            //    Vector3.one * controller.enemyStats.attackRange, controller.eyes.forward, out hit, controller.Tower.rig.rotation, controller.enemyStats.lookRange)
            //    && hit.collider.CompareTag("Enemy"))
            //{
            //    //if (controller.CheckIfCountDownElapsed(controller.enemyStats.attackRate))
            //    //{
            //    // controller.Tower.targetEnemy = controller.chaseTarget;
            //    Debug.Log("Acctack");
            //    controller.Tower.Attack();
            //    controller.Tower.UpdateLaserDirection();
            //    //}
            //}

            Vector3 dir = controller.chaseTarget.transform.position - controller.eyes.position;

            Quaternion lookRotation = Quaternion.LookRotation(dir);

            if (15f >= Quaternion.Angle(lookRotation, Quaternion.LookRotation(controller.eyes.forward))
                || 10f > Vector3.Distance(controller.chaseTarget.transform.position, controller.Tower.partToRotate.transform.position))
            {
                // Debug.Log("Acctack");
                controller.Tower.Attack();
                controller.Tower.UpdateLaserDirection();
            }
            
            //Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.lookRange, Color.blue);
            //Debug.DrawRay(controller.Tower.partToRotate.transform.position, controller.eyes.forward, Color.black);
        }
    }
}
