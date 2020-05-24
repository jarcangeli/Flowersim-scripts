using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedDispenser : MonoBehaviour, IClickable
{
    [SerializeField]
    PlantSpot seedPos = null;
    public GameObject plantPrefab = null;
    PlayerMoney money;

    public bool nonNegative = true;

    int nDispensed = 0;
    FloatRange[] firstColors = new FloatRange[] 
    { 
        new FloatRange(60f/360, 80f/360), // yellow
        new FloatRange(180f/360, 210f/360), // blue
        new FloatRange(345f/360, 360f/360), // red
    };
    float petalColor = -1f;

    // Sold seed types
    [SerializeField]
    TextMeshPro soldSeedText = null;
    FlowerType[] seedTypes = new FlowerType[] { FlowerType.normal, FlowerType.tulip, FlowerType.rose };
    int[] seedPrices = new int[] { 1, 10, 50 };
    string[] seedNames = new string[] { "Regular", "Tulip", "Rose" };

    int typeInt = 0;
    FlowerType seedType;
    int seedPrice;
    string seedName;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSoldSeed();

        money = FindObjectOfType<PlayerMoney>();
        GenerateSeed();
    }

    void UpdateSoldSeed()
    {
        seedType = seedTypes[typeInt];
        seedPrice = seedPrices[typeInt];
        seedName = seedNames[typeInt];

        soldSeedText.text = seedName + " - " + seedPrice + "$";
    }

    public void GenerateSeed()
    {
        if (!seedPos.filled && (money.money >= seedPrice || !nonNegative))
        {
            if (nDispensed < firstColors.Length)
            {
                petalColor = firstColors[nDispensed].GetValue();
            }
            else { petalColor = -1f; }

            PlantSeed newSeed = SeedGenerator.GenerateSeed(plantPrefab, petalColor, seedType).GetComponent<PlantSeed>();
            seedPos.FillSpot(newSeed);
            money.TakeMoney(seedPrice);
            ++nDispensed;
        }
        else
        {
            Debug.Log("No room/money to generate seed");
        }
    }

    public void OnClick(CursorTool tool)
    {
        GenerateSeed();
    }

    public void UpArrow()
    {
        ++typeInt;
        if (typeInt >= seedTypes.Length)
        {
            typeInt = 0;
        }
        UpdateSoldSeed();
    }

    public void DownArrow()
    {
        --typeInt;
        if (typeInt < 0)
        {
            typeInt = seedTypes.Length - 1;
        }
        UpdateSoldSeed();
    }
}
