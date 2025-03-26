using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private TextMeshProUGUI letterText;

    [Header("Events")] 
    public static Action<char> onKeyPressed;

    private void Start()
    {
        //GetComponent<Button>().onClick.AddListener(SendKeyPressedEvent);
    }

    private void SendKeyPressedEvent()
    {
        onKeyPressed?.Invoke(letterText.text[0]);
    }
}
