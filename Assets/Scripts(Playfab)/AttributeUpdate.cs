using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AttributeUpdate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI attText;
    [SerializeField] Slider slider;
    [SerializeField] string text;

    private void FixedUpdate()
    {
        attText.text = $"{text}: {slider.value}";
    }
}
