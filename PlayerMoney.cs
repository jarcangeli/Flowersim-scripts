using TMPro;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int money = 4;
    public TextMeshProUGUI text;

    public void AddMoney(int pay)
    {
        money += pay;
        UpdateText();
    }
    public void TakeMoney(int cost)
    {
        money -= cost;
        UpdateText();
    }

    void UpdateText() 
    {
        text.text = money.ToString() + " $";
    }

    private void Start()
    {
        UpdateText();
    }
}
