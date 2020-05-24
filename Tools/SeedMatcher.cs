using UnityEngine;
using SG = SeedGenerator;
using System.Collections.Generic;

public class SeedMatcher : MonoBehaviour, IClickable
{
    public PlantSeed seedA;
    public PlantSeed seedB;
    PlayerInputs player;
    int nProperties;
    public bool[] masterToggle;
    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
        nProperties = SG.floatRangeManagers.Length + SG.intRangeManagers.Length
                        + SG.floatRanges.Length + SG.intRanges.Length;
        masterToggle = new bool[nProperties];
        ToggleAll(true);
    }

    public void ToggleAll(bool toggle)
    {
        for (int i = 0; i < masterToggle.Length; ++i)
        {
            masterToggle[i] = toggle;
        }
    }

    public void ToggleInd(int ind)
    {
        masterToggle[ind] = !masterToggle[ind];
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.extract)
        {
            PollenExtractor extractor = player.heldObject.GetComponent<PollenExtractor>();
            if (extractor.seedA != null && extractor.seedB != null)
            {
                PrintMatch(extractor.seedA, extractor.seedB);
                extractor.EmptyExtractor();
            }
        }

    }

    public void PrintMatch(PlantSeed seedA, PlantSeed seedB)
    {
        float match = Match(seedA, seedB);
        Debug.Log(seedA.name + "and " + seedB.name + " are a " + (match * 100) + " % match (not implemented)");
    }
    /*
    public float Match(PlantSeed seed, PlantSeed target)
    {
        // for debugging track individual weights/matches
        float[] weights = new float[nProperties];
        float[] matches = new float[nProperties];

        int iProp = 0; // current property index out of total

        float totalW = 0;
        float totalM = 0;

        float currW;
        float currM;

        for (int i = 0; i < SG.floatRangeManagers.Length; ++i)
        {
            currM = seed.floatRanges[i].Match(target.floatRanges[i]);
            currW = masterToggle[iProp] ? SG.floatRangeWeights[i] : 0;
            totalW += currW; totalM += currM * currW;
            weights[iProp] = currW; matches[iProp] = currM;
            ++iProp;
        }
        for (int i = 0; i < SG.intRangeManagers.Length; ++i)
        {
            currM = seed.intRanges[i].Match(target.intRanges[i]);
            currW = masterToggle[iProp] ? SG.intRangeWeights[i] : 0;
            totalW += currW; totalM += currM * currW;
            weights[iProp] = currW; matches[iProp] = currM;
            ++iProp;
        }
        for (int i = 0; i < SG.floatRanges.Length; ++i)
        {
            currM = SG.floatRanges[i].Match(seed.floats[i], target.floats[i]);
            currW = masterToggle[iProp] ? SG.floatWeights[i] : 0;
            totalW += currW; totalM += currM * currW;
            weights[iProp] = currW; matches[iProp] = currM;
            ++iProp;
        }
        for (int i = 0; i < SG.intRanges.Length; ++i)
        {
            currM = SG.intRanges[i].Match(seed.ints[i], target.ints[i]);
            currW = masterToggle[iProp] ? SG.intWeights[i] : 0;
            totalW += currW; totalM += currM * currW;
            weights[iProp] = currW; matches[iProp] = currM;
            ++iProp;
        }
        for (int i = 0; i < SG.colorRanges.Length; ++i)
        {
            currM = ColorRange.Match(seed.colors[i], target.colors[i]);
            currW = masterToggle[iProp] ? SG.colorWeights[i] : 0;
            totalW += currW; totalM += currM * currW;
            weights[iProp] = currW; matches[iProp] = currM;
            ++iProp;
        }
        //Debug.Log("match " + i + " = " + matches[i] + " with weight " + weights[i]);

        if (totalW != 0)
        {
            return totalM / totalW;
        }
        else
        {
            return 0f;
        }
    }
    */

    public float Match(PlantSeed a, PlantSeed b)
    {
        return 1f;
    }
}
