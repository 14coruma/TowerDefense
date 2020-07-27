using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target {get; set;}
    public float speed = 200f;
    public float damage = 10f; // How much damage enemy applies to Shield
    public float maxHealth = 100;
    public float health {get; set;}
    public int prize = 5;
    public Rigidbody2D rb {get; set;}
    public Player player {get; set;}
    public HealthBar healthBar {get; set;}
    public float distance {get; set;} = float.MaxValue;
    public bool reachedEndOfPath {get; set;} = false;

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

    public void SetStats(float setMaxHealth, int setPrize) {
        maxHealth = setMaxHealth;
        prize = setPrize;
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
        if (rb.velocity != Vector2.zero) {
            transform.Find("EnemySprite").right = rb.velocity.normalized;
        }
    }
}
