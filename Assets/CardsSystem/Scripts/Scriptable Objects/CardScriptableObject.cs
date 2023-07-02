using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Cards")]

[System.Serializable]
public class CardScriptableObject : ScriptableObject
{
    public string Name;
    public string Description;
    public int OrderNumber;
    public GameObject CardPrefab;
}
