using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public int money = 100;
    int score = 0;
    public Transform gridPrefab;
    public Transform grid;
    Transform selected;
    public List<string> turretIds;
    Dictionary<string, Transform> turretPrefabs;
    public Transform shieldPrefab;
    Enemies enemies;
    GUI gui;
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("Enemies").GetComponent<Enemies>();
        gui = GameObject.Find("GUI").GetComponent<GUI>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        turretPrefabs = new Dictionary<string, Transform>();
        foreach (string turretId in turretIds) {
            turretPrefabs.Add(turretId, Resources.Load<GameObject>("Prefabs/Turret" + turretId).transform);
        }

        UpdateMoney();
        UpdateScore();
    }

    public void BuildShield() {
        int cost = shieldPrefab.GetComponent<Shield>().cost;
        if (cost > money) {
            return;
        }
        Transform newShield = Instantiate(shieldPrefab, grid.position, Quaternion.identity);
        enemies.UpdateGridGraph(newShield.GetComponent<Collider2D>());
        UpdateMoney(0 - cost);
        HandleGridCollisions();
    }

    /// <summary>
    /// Builds a turret at position of the "grid" object
    /// Called by TurretButton "Click()"
    /// </summary>
    public void BuildTurret(string typeId) {
        int cost = turretPrefabs[typeId].GetComponent<Turret>().cost;
        if (cost > money) {
            return;
        }
        Transform newTurret = Instantiate(turretPrefabs[typeId], grid.position, Quaternion.identity);
        enemies.UpdateGridGraph(newTurret.GetComponent<Collider2D>());
        UpdateMoney(0 - cost);
        HandleGridCollisions();
    }

    /// <summary>
    /// Destroys "selected" at position of the "grid" object
    /// Called by TrashButton "Click()"
    /// </summary>
    public void DestroyObject() {
        int refund = selected.GetComponent<Buildable>().cost; // TODO: Different refund rate later
        Destroy(selected.gameObject);
        AstarPath.active.Scan();
        UpdateMoney(refund);
        HandleGridCollisions();
    }

    public void UpgradeObject() {
        int cost = selected.GetComponent<Turret>().Upgrade();
        UpdateMoney(0-cost);
    }

    void HandleGridCollisions() {
        int maxColliders = 4; // Max number of colliders to sort through
        Collider2D[] colliders = new Collider2D[maxColliders];
        ContactFilter2D cf = new ContactFilter2D(); // Collide with triggers, like "Start" and "Stop"
        cf.useTriggers = true;
        int numColliders = grid.GetComponent<Collider2D>().OverlapCollider(cf, colliders);
        bool turret = false, wall = false, start = false, stop = false, shield = false;
        for (int i = 0; i < numColliders; i++) {
            GameObject go = colliders[i].gameObject;
            if (go.tag == "Turret") {
                turret = true;
                selected = go.transform;
            } else if (go.tag == "Wall") {
                wall = true;
            } else if (go.tag == "Start") {
                start = true;
            } else if (go.tag == "Stop") {
                stop = true;
            } else if (go.tag == "Shield") {
                shield = true;
                selected = go.transform;
            }
        }
        bool canBuildShield = !turret && !wall && !start && !stop && !shield;
        bool canBuildTurret = canBuildShield && enemies.NewObstaclePathPossible(grid.position);
        if (turret || shield) {
            gui.DisableButton("Turret");
            gui.EnableButton("Trash");
            gui.DisableButton("Shield");
            gui.EnableButton("Upgrade");
        } else if (canBuildShield) {
            gui.DisableButton("Trash");
            gui.EnableButton("Shield");
            gui.DisableButton("Upgrade");
            if (canBuildTurret) {
                gui.EnableButton("Turret");
            } else {
                gui.DisableButton("Turret");
            }
        } else {
            gui.DisableButton("Shield");
            gui.DisableButton("Turret");
            gui.DisableButton("Trash");
            gui.DisableButton("Upgrade");
        }
    }

    void HandleMouseClick() {
        Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Check if click is inside gameplay
        if (!levelManager.PointInsideLevel(clickPos)) {
            return;
        }

        gui.CloseAllPanels();

        // Move selector grid object
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
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            // Don't handle click if GUI elements are overlapping
            if (!EventSystem.current.IsPointerOverGameObject()) {
                HandleMouseClick();
            }
        }
    }
}
