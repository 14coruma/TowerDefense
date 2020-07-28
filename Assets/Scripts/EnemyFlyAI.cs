using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyAI : EnemyAI
{


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
        target = GameObject.Find("Stop").transform;
        healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();

        health = maxHealth;
        healthBar.UpdateMax(maxHealth);
        healthBar.UpdateValue(health);
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector2 direction = ((Vector2) target.position - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        distance = Vector2.Distance(rb.position, target.position);

        if (rb.velocity != Vector2.zero) {
            transform.Find("EnemySprite").right = rb.velocity.normalized;
        }
    }
}
