using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] private TMP_Text uRScoreLabel;
    [SerializeField] private TMP_Text[] usernameLabel;
    [SerializeField] private TMP_Text[] scoresLabel;
    [SerializeField] private Button topScoreLabel;
    private DataUser[] dataUsers = new DataUser[5];
    private Dictionary<string, int> usersDic = new Dictionary<string, int>();
    [DoNotSerialize] public int score;
    private DatabaseReference mDatabase;
    private int iterations = 5;

    private void Awake()
    {
        Instance = this;
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        var auth = FirebaseAuth.DefaultInstance;
        GetScore(auth);

    }
    void Start()
    {
        topScoreLabel.onClick.AddListener(GetUsersHighestScores);
        UIElementsManager.Instance.DisableUI();
    }

    public void WriteNewScore(FirebaseAuth auth, int newScore)
    {
        mDatabase.Child("users").Child(auth.CurrentUser.UserId).Child("score").SetValueAsync(newScore);
    }

    private void GetScore(FirebaseAuth auth)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/score")
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                uRScoreLabel.text = "Your Best Score: " + snapshot.Value.ToString();
                score = int.Parse(snapshot.Value.ToString());
            }
        });
    }


    public void GetUsersHighestScores() 
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByValue().GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    foreach (var userDoc in (Dictionary<string, object>)snapshot.Value)
                    {
                        var userObject = (Dictionary<string, object>)userDoc.Value;
                        string username = (string)userObject["username"];
                        int score = Convert.ToInt32(userObject["score"]);

                        if (usersDic.ContainsKey(username))
                        {
                            if (usersDic[username] < score)
                                usersDic[username] = score;
                        }
                        else
                            usersDic.Add(username, score);
                    }

                    var sortedUsers = usersDic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    var list_Users_Name = sortedUsers.Keys.ToList();
                    var list_Users_Score = sortedUsers.Values.ToList();

                    for (int i = 0; i < iterations; i++)
                    {
                        DataUser user = new DataUser();
                        user.username = list_Users_Name[i];
                        user.score = list_Users_Score[i];
                        dataUsers[i] = user;
                        usernameLabel[i].text = (i + 1) + ". " + dataUsers[i].username;
                        scoresLabel[i].text = dataUsers[i].score.ToString();

                    }
                }
            });
    }
}

[System.Serializable]
public class DataUser
{
    public string username;
    public int score;
}