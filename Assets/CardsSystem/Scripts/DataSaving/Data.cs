using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Data : MonoBehaviour
{
    string saveFilePath;

    public GameData gameData;

    private void Awake()
    {
        gameData = new GameData();
        saveFilePath = Application.persistentDataPath + "/gameData.Json";
    }

    public void WriteToJson()
    {
        gameData.chosenCards = DataSaver.Instance.chosenCards.Select(card => card.cardStats).ToList();
        string JsonString = JsonUtility.ToJson(gameData);
        File.WriteAllText(saveFilePath, JsonString);
    }

    public void ReadFromJson()
    {
        if (File.Exists(saveFilePath))
        {
            string contentFromSaveFile = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(contentFromSaveFile);


            List<Card> cardsToDestroy = new List<Card>(DataSaver.Instance.chosenCards);

            foreach (Card card in cardsToDestroy)
            {
                Destroy(card.gameObject);
            }

            DataSaver.Instance.chosenCards.Clear();

            DataSaver.Instance.chosenCards = new List<Card>();


            foreach (CardScriptableObject cardStats in gameData.chosenCards)
            {
                Vector3 randomSpawnPosition = new Vector3(Random.Range(-300, 400), Random.Range(-300, 400), 0);
                GameObject prefab = Instantiate(cardStats.CardPrefab, randomSpawnPosition, Quaternion.identity);
                DataSaver.Instance.chosenCards.Add(prefab.GetComponent<Card>());
            }
        }
    }

    public void RestoreJson()
    {
        File.Delete(saveFilePath = Application.persistentDataPath + "/gameData.Json");
    }


    [System.Serializable]
    public class GameData
    {
        public List<CardScriptableObject> chosenCards;
    }


}
