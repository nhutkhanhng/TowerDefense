using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        #region Rotation partRoTation
        //Vector3 dir = target.position - transform.position;
        //Quaternion lookRotation = Quaternion.LookRotation(dir);
        //Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        //partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // controller.Tower.partToRotate.rotation =  Quaternion.Euler(10f, 0f, 0f);

        #endregion
        // Debug.Log("Lookdecision");

        // Apply this rotation to the rigidbody's rotation.
        // controller.Tower.rig.MoveRotation(turnRotation);

        RaycastHit hit;
        // Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.lookRange, Color.black);


        //if (Physics.SphereCast(controller.Tower.transform.position, controller.enemyStats.lookSphereCastRadius, controller.eyes.forward, out hit, controller.enemyStats.lookRange)
        //    && hit.collider.CompareTag("Enemy"))
        //{
        //    controller.chaseTarget =  hit.collider.gameObject.GetComponent<Enemy>();
        //    controller.Tower.target = controller.chaseTarget.transform;
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        if ( controller.chaseTarget == null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            float shortestDistance = Mathf.Infinity;

            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(controller.Tower.transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null && shortestDistance <= controller.enemyStats.lookRange)
            {
                controller.chaseTarget = nearestEnemy.GetComponent<Enemy>();
                controller.Tower.targetEnemy = controller.chaseTarget;
                controller.Tower.target = nearestEnemy.transform;

                return true;
            }
            else
            {
                controller.Tower.target = null;
                return false;
            }
        }

        return false;
    }
}
