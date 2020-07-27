using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret :  Buildable
{
    public float range = 3f;
    public float damage = 10f;
    public float reload = 0.5f;
    bool canAttack = true;
    Enemies enemies;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("Enemies").GetComponent<Enemies>();
    }

    Transform GetTarget() {
        Transform target = enemies.ClosestEnemy(transform.position, range);
        // Look at "Start" when no target
        if (target == null) {
            target = GameObject.Find("Start").transform;
        }
        return target;
    }

    IEnumerator Reload() {
        yield return new WaitForSeconds(reload);
        canAttack = true;
    }

    void Attack(Transform target) {
        if (canAttack && target.tag == "Enemy") {
            target.GetComponent<EnemyAI>().Damage(damage);
            canAttack = false;
            StartCoroutine(Reload());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Transform target = GetTarget();
        // Face the target
        transform.right = target.position - transform.position;
        Attack(target);
    }
}
