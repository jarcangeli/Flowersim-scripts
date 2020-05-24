using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserButton : MonoBehaviour, IClickable
{
    [SerializeField]
    bool up = true;

    SeedDispenser dispenser;

    void Start()
    {
        dispenser = GetComponentInParent<SeedDispenser>();
    }

    public void OnClick(CursorTool tool)
    {
        if (up) { dispenser.UpArrow(); }
        else { dispenser.DownArrow(); }
    }

}
