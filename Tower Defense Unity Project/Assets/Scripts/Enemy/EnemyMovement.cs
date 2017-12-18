using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{

    private Transform target;    

    private Enemy enemy;
    Vector3[] path;
    int targetIndex;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        if (GameObject.FindGameObjectsWithTag("End").Length > 1)
            target = GameObject.FindGameObjectsWithTag("End")[Random.RandomRange(1, 19) / 10].transform;
        else
            target = GameObject.FindGameObjectWithTag("End").transform;

        PathRequestManager.RequestPath(enemy.transform.position, target.position, enemy.Hypotenuse, OnPathFound);
    }

    void Update()
    {
        if (path != null)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                if (Physics.Linecast(path[i], path[i + 1], 1 << LayerMask.NameToLayer("Unwalkable")) && enemy.Hypotenuse)
                {
                    PathRequestManager.RequestPath(enemy.transform.position, target.position, enemy.Hypotenuse, OnPathFound);
                    return;
                }
            }
        }
        

        if (enemy.transform.position.x == target.position.x && enemy.transform.position.z == target.position.z)
        {
            GameObject effect = (GameObject)Instantiate(enemy.deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f);
            DestroyObject(gameObject);
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            if (enemy == null)
                return;

            path = newPath;
            targetIndex = 0;
            
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    WaveSpawner.EnemiesAlive--;
                    PlayerStats.Lives--;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, enemy.speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}

