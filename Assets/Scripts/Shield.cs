using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buildable
{
    public float maxHealth = 100;
    public float health = 100;
    HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();

        health = maxHealth;
        healthBar.UpdateMax(maxHealth);
        healthBar.UpdateValue(health);
    }

    public void SetStats(float setMaxHealth) {
        maxHealth = setMaxHealth;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Transform other = collision.collider.transform;
        if(other.tag == "Enemy") {
            Damage(other.GetComponent<EnemyAI>().damage);
            Destroy(other.gameObject);
        }
    }
    
    public void Damage(float damage) {
        health -= damage;
        healthBar.UpdateValue(health);
        if (health <= 0) {
            // TODO: Player loses
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
