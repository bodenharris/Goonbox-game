using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject playerTextPrefab;           // Assign a prefab with TextMeshProUGUI
    public Transform playerTextContainer;         // Parent object under a Canvas

    private Dictionary<int, TextMeshProUGUI> playerTexts = new Dictionary<int, TextMeshProUGUI>();

    void Awake()
    {
        AirConsole.instance.onConnect += OnPlayerConnect;
        AirConsole.instance.onDisconnect += OnPlayerDisconnect;
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnDestroy()
    {
        // Clean up event listeners to avoid memory leaks
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onConnect -= OnPlayerConnect;
            AirConsole.instance.onDisconnect -= OnPlayerDisconnect;
            AirConsole.instance.onMessage -= OnMessage;
        }
    }

    void OnPlayerConnect(int deviceId)
    {
        if (playerTextPrefab == null || playerTextContainer == null)
        {
            Debug.LogWarning("Missing playerTextPrefab or playerTextContainer.");
            return;
        }

        GameObject newTextObj = Instantiate(playerTextPrefab, playerTextContainer);
        newTextObj.transform.localScale = Vector3.one; // Ensure correct scaling

        TextMeshProUGUI textComp = newTextObj.GetComponent<TextMeshProUGUI>();
        if (textComp == null)
        {
            Debug.LogError("playerTextPrefab is missing a TextMeshProUGUI component.");
            Destroy(newTextObj);
            return;
        }

        textComp.text = "Player " + deviceId;
        textComp.color = Color.white;
        textComp.fontSize = 30;

        RectTransform rt = newTextObj.GetComponent<RectTransform>();
        if (rt != null)
        {
            float yOffset = -60f * playerTexts.Count;
            rt.anchoredPosition = new Vector2(0, yOffset);
            rt.sizeDelta = new Vector2(200, 50);
        }

        playerTexts[deviceId] = textComp;
    }

    void OnPlayerDisconnect(int deviceId)
    {
        if (playerTexts.TryGetValue(deviceId, out TextMeshProUGUI textComp))
        {
            Destroy(textComp.gameObject);
            playerTexts.Remove(deviceId);
        }
    }

    void OnMessage(int from, JToken data)
    {
        Debug.Log($"Message received from {from}: {data}");

        if (data["name"] != null && playerTexts.TryGetValue(from, out TextMeshProUGUI textComp))
        {
            string playerName = data["name"].ToString();
            textComp.text = playerName;
            Debug.Log($"Updated text for player {from} to {playerName}");
        }
    }
}
