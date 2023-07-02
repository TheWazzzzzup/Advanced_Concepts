using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [field: SerializeField] public List<CardScriptableObject> Cards { get; private set; }
    [field: SerializeField] public Data data { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        PlaceCards();

    }

    private void PlaceCards()
    {

        int cardCount = DataSaver.Instance.chosenCards.Count;

        if (cardCount >= 7)
        {
            Debug.Log("Already have enough cards placed.");
            return;
        }

        while (cardCount < 7)
        {
            int randomIndex = Random.Range(0, Cards.Count);
            CardScriptableObject chosenCardStats = Cards[randomIndex];

            if (!DataSaver.Instance.chosenCards.Exists(card => card == chosenCardStats))
            {
                Vector3 randomSpawnPosition = new Vector3(Random.Range(-300, 400), Random.Range(-300, 400), 0);
                GameObject newCardObject = Instantiate(chosenCardStats.CardPrefab, randomSpawnPosition, Quaternion.identity);
                Card newCard = newCardObject.GetComponent<Card>();
                newCard.cardStats = chosenCardStats;
                DataSaver.Instance.chosenCards.Add(newCard);
                cardCount++;
            }
        }
    }



}

