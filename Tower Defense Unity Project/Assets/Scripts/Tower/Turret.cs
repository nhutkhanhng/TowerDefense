using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    [HideInInspector]
	public Transform target;
    [HideInInspector]
	public Enemy targetEnemy;


    private float nextFireTime;

    // [Header("General")]
	// public float range = 15f;

	[Header("Use Bullets (default)")]
	public GameObject bulletPrefab;

	public float fireRate = 1f;

	[Header("Use Laser")]
	public bool useLaser = false;

	public int damageOverTime = 30;
	public float slowAmount = .5f;

	public LineRenderer lineRenderer;
	public ParticleSystem impactEffect;
	public Light impactLight;

	[Header("Unity Setup Fields")]

	public string enemyTag = "Enemy";

	public Transform partToRotate;
	// public float turnSpeed = 10f;

	public Transform firePoint;

    public Rigidbody rig;



    // public Rigidbody m_Shell;                   // Prefab of the shell.
    private void Awake()
    {        
        partToRotate = GetComponentsInChildren<Transform>()[1];
        //Debug.Log(partToRotate.ToString());
        rig = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
		InvokeRepeating("UpdateTarget", 0f, 0.5f);
	}

    #region Update Tower on frame
    //void UpdateTarget ()
    //{
    //       //// Determine the number of degrees to be turned based on the input, speed and time between frames.
    //       //float turn = 100f * turnSpeed * Time.deltaTime;

    //       //// Make this into a rotation in the y axis.
    //       //Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

    //       ////        partToRotate.Rotate(0, turn * 180 /  Mathf.PI, 0);
    //       //partToRotate.Rotate(Vector3.up * turn);
    //       // Determine the number of degrees to be turned based on the input, speed and time between frames.


    //       // Debug.Log(partToRotate.ToString());
    //       // partToRotate.rotation = Random.rotation;
    //       //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

    //       //float shortestDistance = Mathf.Infinity;

    //       //GameObject nearestEnemy = null;

    //       //foreach (GameObject enemy in enemies)
    //       //{
    //       //    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
    //       //    if (distanceToEnemy < shortestDistance)
    //       //    {
    //       //        shortestDistance = distanceToEnemy;
    //       //        nearestEnemy = enemy;
    //       //    }
    //       //}

    //       //if (nearestEnemy != null && shortestDistance <= range)
    //       //{
    //       //    target = nearestEnemy.transform;
    //       //    targetEnemy = nearestEnemy.GetComponent<Enemy>();
    //       //}
    //       //else
    //       //{
    //       //    target = null;
    //       //}

    //   }
    #endregion

    // Update is called once per frame
    public void Update () {

        if (target == null)
        {
            if (useLaser)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }

            return;
        }

    }

    public void LockOnTarget ()
	{
		Vector3 dir = targetEnemy.transform.position - transform.position;

		Quaternion lookRotation = Quaternion.LookRotation(dir);

        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * 2).eulerAngles;
        
		partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // Debug.Log("Rotation");
	}

	void Laser ()
	{
        //if (target == null)
        //{
        //    if (useLaser)
        //    {
        //        if (lineRenderer.enabled)
        //        {
        //            lineRenderer.enabled = false;
        //            impactEffect.Stop();
        //            impactLight.enabled = false;
        //        }
        //    }

        //    return;
        //}

        if (targetEnemy != null)
        {
            // UpdateLaserDirection();
            targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
            targetEnemy.Slow(slowAmount);

            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
                impactEffect.Play();
                impactLight.enabled = true;
            }
        }
	}

    public void UpdateLaserDirection()
    {
        if (targetEnemy != null)
            if (useLaser)
            {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, targetEnemy.transform.position);

                Vector3 dir = firePoint.position - targetEnemy.transform.position;

                impactEffect.transform.position = targetEnemy.transform.position + dir.normalized;

                impactEffect.transform.rotation = Quaternion.LookRotation(dir);                
            }
    }

    public void Shoot()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            // Set the fired flag so only Fire is only called once.
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();

            // Set the shell's velocity to the launch force in the fire position's forward direction.

            if (bullet != null)
            {
                bullet.Seek(target);
            }
        }
    }

    public void Attack()
    {
        if (target == null)
        {
            Debug.Log("Target == NULL");
            if (useLaser)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
            }

            return;
        }

        if (useLaser)
        {
  //          Debug.Log("Lazer");
            Laser();
        }
        else
        {
//            Debug.Log("Shoot");
                Shoot();
        }
    }
}
