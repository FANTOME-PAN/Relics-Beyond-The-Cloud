using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplaySkillName : MonoBehaviour {

    public static DisplaySkillName Instance
    {
        get; private set;
    }
    Text text;
    private void Awake()
    {
        text = GetComponent<Text>();
        Instance = this;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
