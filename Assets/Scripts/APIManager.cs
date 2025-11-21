using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    [SerializeField] private string _gasURL;
    public string SessionId;

    public InputField UserInput;
    public TMP_Text ChatDisplay;
    public TMP_Text ResultPost;

    public InputField UserInput_1;
    public GameObject ResultBoard;

    public TMP_Text UserResponse;
    private bool _isProcessing;

    public List<GameObject> Panel = new List<GameObject>();
    public List<Vector3> Panel_Pos = new List<Vector3>();
    public List<AudioClip> Tones= new List<AudioClip>();
    public AudioSource AITONE;


    private void Awake()
    {
        if (Panel.Count != 0)
        {
            for (int i = 0; i <= Panel.Count - 1; i++)
            {
                Panel_Pos.Add(Panel[i].transform.position);
                print("what");
            }
        }
        Panel[0].transform.position = Vector3.zero;
        ResultBoard.gameObject.SetActive(false);
    }
    void Start()
    {
        SessionId = System.Guid.NewGuid().ToString();
        Debug.Log("Session ID: " + SessionId);

    }

    void Update()
    {
        //  Trigger send on Enter/Return
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    OnSendPressed();
        //}

        if (Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void OnSendPressed()
    {
        if (_isProcessing) return;   // prevents spam
        UserResponse.text = "";
        string message = UserInput.text.Trim();
        if(UserInput_1.text.Trim().Length > 0) { message = UserInput_1.text.Trim(); }
        UserResponse.text= message;
        if (string.IsNullOrEmpty(message)) return;

        _isProcessing = true;        // lock until done
        StartCoroutine(SendDataToGAS(message));

        UserInput.text = "";
        UserInput_1.text = "";
    }

    private IEnumerator SendDataToGAS(string message)
    {
        ChatDisplay.text = "";
        WWWForm form = new WWWForm();
        form.AddField("parameter", message);
        form.AddField("session", SessionId);

        UnityWebRequest req = UnityWebRequest.Post(_gasURL, form);
        yield return req.SendWebRequest();

        string reply = req.result == UnityWebRequest.Result.Success ? req.downloadHandler.text : "Error: " + req.error;

        // Detect end of conversation
        if (reply.Contains("<END_CONVERSATION>"))
        {
            string cleaned = reply.Replace("<END_CONVERSATION>", "").Trim();

            ChatDisplay.text += $"\n\n<color=#000000><b>Manager:</b></color> {cleaned}";

            EvaluateConversation(cleaned);

            ChatDisplay.text += "\n\n<color=#000000><b>(Conversation Ended)</b></color>";

            UserInput.interactable = false;
            UserInput_1.interactable = false;
            Panel[2].transform.position = Panel_Pos[2];
            Panel[3].transform.position = Vector3.zero;
            yield return new WaitForSeconds(5);
            ResultBoard.gameObject.SetActive(true);

            yield break;
        }

        Panel[2].transform.position = Panel_Pos[2];
        Panel[3].transform.position = Vector3.zero;

        // Show only manager's reply
        yield return StartCoroutine(TypeText(reply));
        _isProcessing = false;
    }
    private IEnumerator TypeText(string fullText)
    {
        ChatDisplay.text += "\n\n<b><color=#000000>Manager:</color></b> ";
        int baseLength = ChatDisplay.text.Length;

        foreach (char c in fullText)
        {
            ChatDisplay.text = ChatDisplay.text.Insert(baseLength, c.ToString());
            baseLength++;
            AITONE.clip=Tones[Random.Range(0, Tones.Count - 1)];
            //AITONE.Play();
            yield return new WaitForSeconds(0.02f); // typing speed
        }
    }

    public void EnterGame()
    {
        Panel[0].transform.position = Panel_Pos[0];
        Panel[1].transform.position = Vector3.zero;
    }
    public void SendMessege()
    {
        OnSendPressed();
        Panel[2].transform.position = Vector3.zero;
        Panel[1].transform.position = Panel_Pos[1];
        Panel[3].transform.position = Panel_Pos[3];

    }

    public void FollowingSendMessege()
    {
        Panel[3].transform.position = Panel_Pos[3];
        OnSendPressed();
        Panel[2].transform.position = Vector3.zero;
    }

    private void EvaluateConversation(string summary)
    {
        summary = summary.ToLower();

        bool good =
            summary.Contains("agreement") ||
            summary.Contains("agreed") ||
            summary.Contains("solution") ||
            summary.Contains("resolved") ||
            summary.Contains("good job") ||
            summary.Contains("you handled well") ||
            summary.Contains("effective");

        bool bad =
            summary.Contains("no compromise") ||
            summary.Contains("cannot agree") ||
            summary.Contains("disagree") ||
            summary.Contains("failed") ||
            summary.Contains("refuse") ||
            summary.Contains("breakdown");

        if (good)
        {
            ResultPost.text += "\n\n<b><color=#00AA00> Conversation Result: SUCCESS</color></b>";
            ResultPost.text += "\n<color=#00AA00>You handled the conversation well and reached an agreement.</color>";
        }
        else if (bad)
        {
            ResultPost.text += "\n\n<b><color=#AA0000> Conversation Result: FAILED</color></b>";
            ResultPost.text += "\n<color=#AA0000>You were unable to reach agreement. Consider improving your approach.</color>";
        }
        else
        {
            ResultPost.text += "\n\n<b><color=#888888>Conversation Result: Neutral</color></b>";
            ResultPost.text += "\n<color=#888888>The outcome is unclear, but the conversation ended.</color>";
        }
    }


    public void CheckResult()
    {
        Panel[3].transform.position = Panel_Pos[3];
        Panel[4].transform.position = Vector3.zero;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}



