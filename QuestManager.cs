using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour, IClickable
{
    public Quest currQuest;
    AudioManager audioManager;
    [SerializeField]
    PlantSpot questSpot = null;
    [SerializeField]
    Quest quest = null;
    [SerializeField]
    TextMeshPro questText = null;
    PlayerInputs player;
    PlayerMoney money;
    public int postageCost = 0;
    public float newQuestDelay = 0.5f;
    int flowerTypeMax = 1;

    int nQuests = 0;
    FloatRange[] firstColors = new FloatRange[]
    {
        new FloatRange(280f/360, 300f/300),
        new FloatRange(100f/360, 150f/360)
    };
    float petalColor = -1f;

    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
        money = FindObjectOfType<PlayerMoney>();
        audioManager = FindObjectOfType<AudioManager>();
        NewQuest();
    }

    public void NewQuest()
    {
        FlowerType newQuestType = FlowerType.normal;
        if (money.money > 10 && flowerTypeMax == 1) 
        { 
            flowerTypeMax += 1;
            newQuestType = FlowerType.tulip;
        }
        else if (money.money > 50 && flowerTypeMax == 2) 
        { 
            flowerTypeMax += 1;
            newQuestType = FlowerType.rose;
        }
        else
        {
            newQuestType = (FlowerType)Random.Range(0, flowerTypeMax);
        }



        if (nQuests < firstColors.Length)
        {
            petalColor = firstColors[nQuests].GetValue();
        }
        else petalColor = -1f;

        questSpot.EmptySpot();
        quest.GenerateQuestSeed(petalColor, newQuestType);
        questSpot.FillSpot(quest.questSeed);
        currQuest = quest;
        SetQuestText();
        ++nQuests;
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold)
        {
            if (player.heldObject != null && currQuest != null 
                && player.heldObject.GetComponent<PlantSeed>() is PlantSeed seed
                && (seed.growthStage >= seed.nGrowthStages || seed.isCutting))
            {
                audioManager.Play("QuestComplete");
                float match = currQuest.SubmitMatch(seed);
                int reward = currQuest.GetReward(match);
                money.TakeMoney(postageCost);
                money.AddMoney(reward);
                SetMatchQuestText((int)(100*match), reward);
                Invoke("NewQuest", newQuestDelay);
                if (seed.plantSpot != null) { seed.plantSpot.EmptySpot(); }
                Destroy(seed.gameObject);
                player.heldObject = null;
            }
        }
    }

    void SetQuestText()
    {
        questText.text = QuestTextGenerator.Generate();
    }
    void SetMatchQuestText(int match, int reward)
    {
        questText.text = match + "% match degree. Earned " + reward + "$";
    }
}
