using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplayPlayerProperity : MonoBehaviour
{

    public static DisplayPlayerProperity Instance
    {
        get; private set;
    }
    Text text;
    private void Awake()
    {
        text = GetComponent<Text>();
        Instance = this;
    }

    public void SetText(float sp, float maxSp, float mp, float maxMp)
    {
        string str = "SP : " + Mathf.RoundToInt(sp) + "/" + Mathf.RoundToInt(maxSp);
        str += "\nMP : " + Mathf.RoundToInt(mp) + "/" + Mathf.RoundToInt(maxMp);
        text.text = str;
    }
}
