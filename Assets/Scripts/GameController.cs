using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject playerTextPrefab; // Assign a Text prefab in Unity
    public Transform playerTextContainer; // Parent object for text boxes

    private Dictionary<int, TextMeshProUGUI> playerTexts = new Dictionary<int, TextMeshProUGUI>();

    void Awake()
    {
        AirConsole.instance.onConnect += OnPlayerConnect;
        AirConsole.instance.onDisconnect += OnPlayerDisconnect;
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnPlayerConnect(int device_id)
    {
        GameObject newTextObj = Instantiate(playerTextPrefab);
        newTextObj.transform.SetParent(playerTextContainer, false);

        TextMeshProUGUI textComp = newTextObj.GetComponent<TextMeshProUGUI>();
        if (textComp == null) return;

        textComp.text = "Player " + device_id;
        textComp.color = Color.white;
        textComp.fontSize = 30;

        // Position each player vertically so they don’t overlap
        RectTransform rt = newTextObj.GetComponent<RectTransform>();
        float yOffset = -60f * playerTexts.Count; // 60 pixels apart
        rt.anchoredPosition = new Vector2(0, yOffset);
        rt.sizeDelta = new Vector2(200, 50);

        playerTexts[device_id] = textComp;
    }


    void OnPlayerDisconnect(int device_id)
    {
        // Remove the player's text box
        if (playerTexts.ContainsKey(device_id))
        {
            Destroy(playerTexts[device_id].gameObject);
            playerTexts.Remove(device_id);
        }
    }

    void OnMessage(int from, JToken data)
    {
        Debug.Log("Message received from " + from + ": " + data.ToString());

        if (data["name"] != null && playerTexts.ContainsKey(from))
        {
            playerTexts[from].text = data["name"].ToString();
            Debug.Log("Updated text for player " + from + " to " + data["name"]);
        }
    }

}
