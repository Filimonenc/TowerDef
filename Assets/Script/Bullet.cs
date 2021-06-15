using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    
    public float speed = 70f;
    public float explosionRadius = 0f;

    public int damage = 50;
    
    public GameObject BulletEffect;
    public GameObject impactEffect;
   
    public void Seek(Transform _target)
    {
        target = _target;

    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude < distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);


    }
    void HitTarget()
    {
        GameObject effectIns = (GameObject) Instantiate(BulletEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        if (explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            Damage(target);
        }
        
        
        //Destroy(target.gameObject);
        Destroy(gameObject);
    }

    void Damage(Transform enemy)
    {
       Enemy e= enemy.GetComponent<Enemy>();

       if (e != null)
       {
           e.TakeDamage(damage);
       }

      
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }

        void onDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}