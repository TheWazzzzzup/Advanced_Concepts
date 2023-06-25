using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using TMPro;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] TMP_InputField characterName;
    [SerializeField] Slider characterDamage;
    [SerializeField] Slider characterHealth;


    Character ReturnClass()
    {
        return new Character(characterName.text, characterDamage.value, characterHealth.value);
    }

    public string GetCharacterJson()
    {
        return JsonUtility.ToJson(ReturnClass());
    }

    public void LoadData(Character character)
    {
        characterName.text = character.Name;
        characterDamage.value = character.Damage;
        characterHealth.value = character.Health;
    }
}

[System.Serializable]
public class Character
{
    public string Name;
    public float Damage;
    public float Health;

    public Character(string name, float damage, float health)
    {
        Name = name;
        Damage = damage;
        Health = health;
    }
}