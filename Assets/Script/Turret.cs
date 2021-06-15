using UnityEngine;

namespace Script
{
    public class Turret : MonoBehaviour
    {
        private Transform target;
        private Enemy targetEnemy;

    
        [Header("using Bullets")]
        public GameObject bulletPrefab;
        public float range = 15f;
        public float fireRate = 1f;
        private float fireCountdown = 0f;

        [Header("LaserSetting")] 
        public float slowPct = 0.5f;
        
        public bool useLaser = false;
        public LineRenderer lineRenderer;
        public int damageOverTime = 30;
        public ParticleSystem impactEffect;
    
        [Header("Unity Setup Fields")]

        public string enemyTag = "Enemy";
        public Transform Tower;
        public float turnSpeed = 20;
        
    
        public Transform firePoint;
    



        void Start()
        {
            InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        }

        public void UpdateTarget ()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;


            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null && shortestDistance <= range)
            {
                target = nearestEnemy.transform;
                targetEnemy = nearestEnemy.GetComponent<Enemy>();
            }
            else
            {
                target = null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
            {
                if (useLaser)
                {
                    if (lineRenderer.enabled)
                    {
                        lineRenderer.enabled = false;
                        impactEffect.Stop();
                    }
                        
                }
                return;
            }

            LockOnTarget();
            if (useLaser)
            {
                Laser();
            }
            else
            {
                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
            
                fireCountdown -= Time.deltaTime;
            }
        
        }

        void LockOnTarget()
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(Tower.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            Tower.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        void Laser()
        {
          targetEnemy.TakeDamage(damageOverTime* Time.deltaTime);
          targetEnemy.Slow(slowPct);
          

            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
                impactEffect.Play();
            }
                
        
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, target.position);

            Vector3 dir = firePoint.position - target.position;
            
            impactEffect.transform.position = target.position + dir.normalized;
            
            impactEffect.transform.rotation = Quaternion.LookRotation(dir);
            
            
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
        void Shoot()
        {
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
                bullet.Seek(target);
        }

    }
}
