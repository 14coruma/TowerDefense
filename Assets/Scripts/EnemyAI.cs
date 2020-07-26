using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    Transform target;
    public float speed = 200f;
    public float damage = 10f; // How much damage enemy applies to PlayerShield
    public float nextWaypointDistance = .5f;
    public float maxHealth = 100;
    public float health {get; set;}
    public float distance {get; private set;} = float.MaxValue;
    public int prize = 5;
    Path path;
    int currentWaypoint = 0;
    public bool reachedEndOfPath {get; private set;} = false;
    Seeker seeker;
    Rigidbody2D rb;
    Player player;
    HealthBar healthBar;

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

    public void SetStats(float setMaxHealth, int setPrize) {
        maxHealth = setMaxHealth;
        prize = setPrize;
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

    public void Damage(float damage) {
        health -= damage;
        healthBar.UpdateValue(health);
        if (health <= 0) {
            player.UpdateMoney(prize);
            player.UpdateScore(prize);
            prize = 0; // Just make sure enemy isn't accounted for more than once
            Destroy(gameObject);
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
