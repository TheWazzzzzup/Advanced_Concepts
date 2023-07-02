using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour
{
    public CardScriptableObject cardStats;

    public void Initialize(CardScriptableObject cardStats)
    {
        this.cardStats = cardStats;
    }
}
