using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelToggle : MonoBehaviour, IClickable
{
    [SerializeField]
    Animator qpAnimator = null;
    [SerializeField]
    SpriteRenderer arrowSprite = null;
    bool hidden = false;

    public void OnClick(CursorTool tool)
    {
        if (hidden)
        {
            ShowPanel();
        }
        else
        {
            HidePanel();
        }
    }

    void HidePanel()
    {
        if (!hidden)
        {
            hidden = true;
            qpAnimator.SetTrigger("HideTrigger");
            qpAnimator.ResetTrigger("ShowTrigger");
            arrowSprite.transform.localRotation *= Quaternion.Euler(0f, 0f, 180f);
        }
    }

    void ShowPanel()
    {
        if (hidden)
        {
            hidden = false;
            qpAnimator.SetTrigger("ShowTrigger");
            qpAnimator.ResetTrigger("HideTrigger");
            arrowSprite.transform.localRotation *= Quaternion.Euler(0f, 0f, 180f);
        }
    }
    
}
