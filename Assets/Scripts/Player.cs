using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int money = 100;
    int score = 0;
    public Transform gridPrefab;
    public Transform grid;
    Transform selectedTurret;
    public Transform turretPrefab; // TODO: Temporary, do in menu
    Enemies enemies;
    GUI gui;
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("Enemies").GetComponent<Enemies>();
        gui = GameObject.Find("GUI").GetComponent<GUI>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        UpdateMoney();
        UpdateScore();
    }

    /// <summary>
    /// Builds a turret at position of the "grid" object
    /// Called by TurretButton "Click()"
    /// </summary>
    public void BuildTurret() {
        int cost = turretPrefab.GetComponent<Turret>().cost;
        if (cost > money) {
            return;
        }
        Transform newTurret = Instantiate(turretPrefab, grid.position, Quaternion.identity);
        enemies.UpdateGridGraph(newTurret.GetComponent<Collider2D>());
        UpdateMoney(0 - cost);
        HandleGridCollisions();
    }

    /// <summary>
    /// Destroys "selectedTurret" at position of the "grid" object
    /// Called by TrashButton "Click()"
    /// </summary>
    public void DestroyTurret() {
        int refund = selectedTurret.GetComponent<Turret>().cost; // TODO: Different refund rate later
        Destroy(selectedTurret.gameObject);
        AstarPath.active.Scan();
        UpdateMoney(refund);
        HandleGridCollisions();
    }

    void HandleGridCollisions() {
        int maxColliders = 4; // Max number of colliders to sort through
        Collider2D[] colliders = new Collider2D[maxColliders];
        int numColliders = grid.GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), colliders);
        bool turret = false, wall = false;
        for (int i = 0; i < numColliders; i++) {
            GameObject go = colliders[i].gameObject;
            if (go.tag == "Turret") {
                turret = true;
                selectedTurret = go.transform;
            } else if (go.tag == "Wall") {
                wall = true;
            }
        }
        bool canBuildHere = !turret && !wall && enemies.NewObstaclePathPossible(grid.position);
        if (turret) {
            gui.DisableButton("Turret");
            gui.EnableButton("Trash");
        } else if (canBuildHere) {
            gui.DisableButton("Trash");
            gui.EnableButton("Turret");
        } else {
            gui.DisableButton("Turret");
            gui.DisableButton("Trash");
        }
    }

    void HandleMouseClick() {
        Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Check if click is inside gameplay
        if (!levelManager.PointInsideLevel(clickPos)) {
            return;
        }
        Vector3 clickPos3 = new Vector3(Mathf.Round(clickPos.x), Mathf.Round(clickPos.y), 0f);
        if (clickPos3 != grid.position) {
            Destroy(grid.gameObject);
            grid = Instantiate(gridPrefab, clickPos3, Quaternion.identity);
            HandleGridCollisions();
        }
    }

    public void UpdateMoney(int amount = 0) {
        money += amount;
        gui.DrawMoney(money);
    }
    public void UpdateScore(int amount = 0) {
        score += amount;
        gui.DrawScore(score);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0)) {
            HandleMouseClick();
        }
    }
}
