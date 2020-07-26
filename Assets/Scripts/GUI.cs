using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class GUI : MonoBehaviour
{
    public Transform moneyIndicator;
    public Transform scoreIndicator;
    public List<Transform> buttonsArray; // For editor convenience; to be put in Dictionary 
    Dictionary<string, Transform> buttons;

    public void DrawScore(int score) {
        scoreIndicator.GetComponent<Text>().text = score.ToString();
    }
    public void DrawMoney(int money) {
        moneyIndicator.GetComponent<Text>().text = money.ToString();
    }

    public void DisableButton(string id) {
        buttons[id].GetComponent<Button>().interactable = false;
    }
    public void EnableButton(string id) {
        buttons[id].GetComponent<Button>().interactable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Put buttonArray transforms into dictionary (for convenience)
        buttons = new Dictionary<string, Transform>();
        // Naming convention for buttons: "(id)Button"
        string buttonNamePattern = @"(.+)Button";
        foreach (Transform button in buttonsArray) {
            Match match = Regex.Match(button.name, buttonNamePattern);
            buttons.Add(match.Groups[1].Value, button);
        }
    }
}
