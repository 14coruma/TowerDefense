using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ListExtension
{
    public static T PopAt<T>(this List<T> list, int index)
    {
        T r = list[index];
        list.RemoveAt(index);
        return r;
    }
}

public class TextFileParser
{
    public static List<List<object>> ParseTextFile(string filename) {
        string text = Resources.Load<TextAsset>(filename).ToString();
        List<List<object>> rows = new List<List<object>>();
        List<string> textRows = new List<string>(text.Split(
            new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None));
        List<string> colTypes = new List<string>(textRows.PopAt(0).Split(','));
        foreach(string textRow in textRows) {
            string[] textCol = textRow.Split(',');
            List<object> row = new List<object>(colTypes.Count);
            for (int i = 0; i < colTypes.Count; i++) {
                switch (colTypes[i]) {
                    case "string":
                        row.Add(textCol[i]);
                        break;
                    case "int":
                        row.Add(int.Parse(textCol[i]));
                        break;
                    case "float":
                        row.Add(float.Parse(textCol[i]));
                        break;
                    default:
                        Debug.LogError("TextFileParser: Invalid col type");
                        break;
                }
            }
            rows.Add(row);
        }
        return rows;
    }
}