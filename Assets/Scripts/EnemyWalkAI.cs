using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyWalkAI : EnemyAI
{
    public float nextWaypointDistance = .5f;
    Path path;
    int currentWaypoint = 0;
    Seeker seeker;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
        target = GameObject.Find("Stop").transform;
        healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    
        health = maxHealth;
        healthBar.UpdateMax(maxHealth);
        healthBar.UpdateValue(health);
    }

    void UpdatePath() {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
            distance = path.GetTotalLength();
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (path == null)
            return;
        
        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        if (rb.velocity != Vector2.zero) {
            transform.Find("EnemySprite").right = rb.velocity.normalized;
        }
    }
}
