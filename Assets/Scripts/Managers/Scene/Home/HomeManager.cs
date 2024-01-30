using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameLabel;
    private string mUsername;

    private FirebaseAuth mAuth;
    private Button signOutButton;
    private Coroutine getUserCoroutine;
    private DatabaseReference mDatabase;

    private void Awake()
    {
        mAuth = FirebaseAuth.DefaultInstance;
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        getUserCoroutine = StartCoroutine(GetUsername(mAuth)); 
        signOutButton = GameObject.Find("Button_SignOut").GetComponent<Button>();
    }

    void Start()
    {
        UIElementsManager.Instance.DisableUI();
        signOutButton.onClick.AddListener(SignOut);
    }

    private IEnumerator GetUsername(FirebaseAuth auth)
    {
        var taskGetUser = FirebaseDatabase.DefaultInstance.GetReference("users/" + auth.CurrentUser.UserId + "/username").GetValueAsync();

        yield return new WaitUntil(() => taskGetUser.IsCompleted);

        if (taskGetUser.IsFaulted)
            Debug.Log(taskGetUser.Exception);
        else if (taskGetUser.IsCompleted)
        {
            DataSnapshot snapshot = taskGetUser.Result;
            usernameLabel.text = "Welcome " + (string)snapshot.Value;
            mUsername = (string)snapshot.Value;
        }

    }

    private void SignOut()
    {
        if (mAuth.CurrentUser.IsAnonymous)
        {
            mDatabase.Child("users").Child(mAuth.CurrentUser.UserId).RemoveValueAsync();

            FirebaseUser user = mAuth.CurrentUser;
            if (user != null)
            {
                user.DeleteAsync().ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("DeleteAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("User deleted successfully.");
                });
            }
        }

        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene((int)AppScene.LOGIN);
        PlayerPrefs.DeleteKey("UserID");
    }
}
