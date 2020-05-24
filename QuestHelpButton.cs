using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestHelpButton : MonoBehaviour, IClickable
{
    [SerializeField]
    GameObject text = null;

    void Start()
    {
        text.SetActive(false);
    }
    public void OnClick(CursorTool tool)
    {
        text.SetActive(!text.activeInHierarchy);
    }
}
