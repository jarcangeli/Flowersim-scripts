using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestTextGenerator
{
    static string[] names = new string[] 
    { 
        "Farmer Ted", "Lady Sybill", "Old Greg", "The Queen", "Barry", "Samwise", "Saint Catherine", "Baldrick", "A dog", "Xibanya", "Your mother"
    };

    static string[] situations = new string[]
    {
        "wedding", "baby shower", "birthday", "garden party", "graduation", "afternoon snack", "mantlepiece", "new flat", "sanity", "own reasons"
    };

    public static string Generate()
    {
        string name = names[Random.Range(0, names.Length)];
        string situation = situations[Random.Range(0, situations.Length)];

        return name + " needs a flower for their " + situation + ". Try to match the colour.";
    }
}
