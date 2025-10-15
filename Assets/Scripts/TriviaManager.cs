using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TMPro;

public class TriviaManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private string[] questions = {
        "What is the capital of France?",
        "Who wrote 'Romeo and Juliet'?",
        "What is the largest planet in our solar system?",
        "How many continents are there on Earth?"
    };

    private string[] answers = {
        "Paris",
        "William Shakespeare",
        "Jupiter",
        "Seven"
    };

    private int currentQuestionIndex;

    void Start()
    {
        ShowRandomQuestion();
        AirConsole.instance.onMessage += OnMessageReceived;
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
            AirConsole.instance.onMessage -= OnMessageReceived;
    }

    public void ShowRandomQuestion()
    {
        int randomIndex = Random.Range(0, questions.Length);
        currentQuestionIndex = randomIndex;
        questionText.text = questions[randomIndex];
        feedbackText.text = ""; // Clear previous feedback
    }

    void OnMessageReceived(int fromDeviceId, JToken data)
    {
        if (data["answer"] != null)
        {
            string playerAnswer = data["answer"].ToString().Trim();

            if (string.Equals(playerAnswer, answers[currentQuestionIndex], System.StringComparison.OrdinalIgnoreCase))
            {
                feedbackText.text = $"Player {fromDeviceId}: Correct!";
                feedbackText.color = Color.green;
            }
            else
            {
                feedbackText.text = $"Player {fromDeviceId}: Wrong! Correct answer: {answers[currentQuestionIndex]}";
                feedbackText.color = Color.red;
            }

            // Optionally move to next question automatically
            Invoke("ShowRandomQuestion", 3f); // 3-second delay before next question
        }
    }
}
