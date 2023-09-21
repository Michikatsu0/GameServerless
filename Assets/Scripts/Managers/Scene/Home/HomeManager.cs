using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameLabel;

    private void Awake()
    {
        var auth = FirebaseAuth.DefaultInstance;
        GetUsername(auth); 
    }
    void Start()
    {
        UIElementsManager.Instance.DisableUI();
    }

    public void GetUsername(FirebaseAuth auth)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/username")
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                usernameLabel.text = "Welcome " + (string)snapshot.Value;
            }
        });
    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene((int)AppScene.LOGIN);
    }
}
