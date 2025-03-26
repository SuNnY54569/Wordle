using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager Instance { get; private set; }
    
    private Dictionary<char, Button> keyButtons = new Dictionary<char, Button>();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        foreach (Button key in FindObjectsOfType<Button>())
        {
            char letter = key.GetComponentInChildren<TextMeshProUGUI>().text[0];
            RegisterKey(letter, key);
        }
    }
    
    public void RegisterKey(char letter, Button button)
    {
        if (!keyButtons.ContainsKey(letter))
        {
            keyButtons.Add(letter, button);
        }
    }
    
    public void UpdateKeyColor(char letter, Tile.State state)
    {
        if (keyButtons.TryGetValue(letter, out Button button))
        {
            Image keyImage = button.GetComponent<Image>();
            if (keyImage != null)
            {
                keyImage.color = state.fillColor;
            }
        }
    }
}
