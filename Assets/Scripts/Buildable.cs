using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    public int cost = 10;
    public int level {get; set;} = 0;
    public List<List<object>> stats {get; set;}
}
