using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataSaver
{
    public List<Card> chosenCards = new List<Card>();

    private static DataSaver instance;
    public static DataSaver Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataSaver();
            }
            return instance;
        }
    }

    public void SaveChosenCards(List<Card> cards)
    {
        chosenCards = cards;
    }

}
