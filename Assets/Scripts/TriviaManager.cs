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
            string correctAnswer = answers[currentQuestionIndex];
            string feedbackMessage;

            bool isCorrect = string.Equals(
                playerAnswer,
                correctAnswer,
                System.StringComparison.OrdinalIgnoreCase
            );

            if (isCorrect)
            {
                feedbackMessage = "Correct!";
                Debug.Log($"Player {fromDeviceId}: Correct!");
            }
            else
            {
                feedbackMessage = $"Wrong! Correct answer: {correctAnswer}";
                Debug.Log($"Player {fromDeviceId}: Wrong! Answer was {correctAnswer}");
            }

            // Send feedback directly to that player's controller
            AirConsole.instance.Message(fromDeviceId, new
            {
                feedback = feedbackMessage
            });

            // Optional: show something minimal on the main screen
            feedbackText.text = $"Player {fromDeviceId} answered!";
            feedbackText.color = Color.white;

            // Move to next question after a delay
            Invoke("ShowRandomQuestion", 3f);
        }
    }
}
