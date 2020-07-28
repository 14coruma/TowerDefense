using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buildable
{
    public int maxHealth = 5;
    public int health {get; set;} = 5;
    HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();

        health = maxHealth;
        healthBar.UpdateMax(maxHealth);
        healthBar.UpdateValue(health);
    }

    public void SetStats(int setMaxHealth) {
        maxHealth = setMaxHealth;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Transform other = collision.collider.transform;
        if(other.tag == "Enemy") {
            Damage();
            Destroy(other.gameObject);
        }
    }
    
    public void Damage() {
        health--;
        healthBar.UpdateValue(health);
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
