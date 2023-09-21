using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] private GameObject[] signUpErrorList;
    [SerializeField] private GameObject[] loginErrorList;
    [SerializeField] private GameObject[] restoreErrorList;

    private Button loginButton;
    private Button signUpButton;
    private Button restoreButton;
    private Button returnPasswordButton;

    private Coroutine loginCoroutine;
    private Coroutine signOutCoroutine;
    private Coroutine getUsernameCoroutine;
    private Coroutine restorePasswordCoroutine;

    DatabaseReference database;
    private string username;

    private void Awake()
    {
        loginButton = GameObject.Find("Button_Login").GetComponent<Button>();
        signUpButton = GameObject.Find("Button_SignUp").GetComponent<Button>();
        restoreButton = GameObject.Find("Button_Restore").GetComponent<Button>();
        DisableUIErrors();
        
    }

    void Start()
    {
        UIElementsManager.Instance.DisableUI();
        loginButton.onClick.AddListener(HandleLoginButtonClicked);
        signUpButton.onClick.AddListener(HandleRegisterButtonClicked);
        restoreButton.onClick.AddListener(HandleRestoreButtonClicked);
        database = FirebaseDatabase.DefaultInstance.RootReference;
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            SceneManager.LoadScene((int)AppScene.HOME);
    }

    public void DisableUIErrors()
    {
        foreach (var item in loginErrorList){
            item.gameObject.SetActive(false);
        }
        foreach (var item in signUpErrorList){
            item.gameObject.SetActive(false);
        }
        foreach ( var item in restoreErrorList) { 
            item.gameObject.SetActive(false);
        }
    }

    private void HandleRestoreButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;

        restorePasswordCoroutine = StartCoroutine(RestorePassword(email));
    }

    private void HandleLoginButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        loginCoroutine = StartCoroutine(LoginUser(email, password));
    }

    private void HandleRegisterButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string username = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        signOutCoroutine = StartCoroutine(RegisterUser(email, username, password));
    }

    private IEnumerator RestorePassword(string email)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var restoreTask = auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => restoreTask.IsCompleted);
        if (restoreTask.IsCanceled)
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        else if (restoreTask.IsFaulted)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + restoreTask.Exception);

            if (restoreTask.Exception.Message == "One or more errors occurred. (The email address is badly formatted.)")
            {
                DisableUIErrors();
                restoreErrorList[0].gameObject.SetActive(true);
            }
            else if (restoreTask.Exception.Message == "One or more errors occurred. (An email address must be provided.)")
            {
                DisableUIErrors();
                restoreErrorList[1].gameObject.SetActive(true);
            }
            else if (restoreTask.Exception.Message == "One or more errors occurred. (There is no user record corresponding to this identifier. The user may have been deleted.)")
            {
                DisableUIErrors();
                restoreErrorList[2].SetActive(true);
            }
        }
        else
        {
            DisableUIErrors();
            restoreErrorList[3].SetActive(true);
            Debug.Log("Password reset email sent successfully.");
        }
    }

    private IEnumerator LoginUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.IsCanceled)
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception.Message);

            if (registerTask.Exception.Message == "One or more errors occurred. (The email address is badly formatted.)")
            {
                DisableUIErrors();
                loginErrorList[1].gameObject.SetActive(true);
            }
            else if (registerTask.Exception.Message == "One or more errors occurred. (An email address must be provided.)")
            {
                DisableUIErrors();
                loginErrorList[2].gameObject.SetActive(true);
            }
            else if (registerTask.Exception.Message == "One or more errors occurred. (A password must be provided.)")
            {
                DisableUIErrors();
                loginErrorList[3].gameObject.SetActive(true);
            }
            else
            {
                DisableUIErrors();
                loginErrorList[0].gameObject.SetActive(true);
            }
        }
        else
        {
            AuthResult result = registerTask.Result;
            SceneManagement.Instance.ChangeScene((int)AppScene.HOME);
        }

    }

    public void GetUsername(FirebaseAuth auth)
    {
        string username = "";
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/username")
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.LogFormat(
                "User signed in successfully: {0} ({1} : {2})",
                snapshot.Value,
                auth.CurrentUser.Email,
                auth.CurrentUser.UserId);

                username = (string)snapshot.Value;

            }
        });
    }

    private IEnumerator RegisterUser(string email, string username, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.IsCanceled)
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception.Message);

            if (registerTask.Exception.Message == "One or more errors occurred. (The email address is already in use by another account.)")
            {
                DisableUIErrors();
                signUpErrorList[1].gameObject.SetActive(true);
            }
            else if (registerTask.Exception.Message == "One or more errors occurred. (The email address is badly formatted.)")
            {
                DisableUIErrors();
                loginErrorList[2].gameObject.SetActive(true);
            }
            else if (registerTask.Exception.Message == "One or more errors occurred. (An email address must be provided.)")
            {
                DisableUIErrors();
                signUpErrorList[3].gameObject.SetActive(true);
            }
            else if (registerTask.Exception.Message == "One or more errors occurred. (The given password is invalid.)")
            {
                DisableUIErrors();
                signUpErrorList[4].gameObject.SetActive(true);
            }
            else if (registerTask.Exception.Message == "One or more errors occurred. (A password must be provided.)")
            {
                DisableUIErrors();
                signUpErrorList[5].gameObject.SetActive(true);
            }
        }
        else
        {
            AuthResult result = registerTask.Result;

            if (ValidUsername(username))
            {
                Debug.LogFormat(
                        "Firebase user created successfully: {0} ({1} : {2})",
                        username,
                        result.User.Email,
                        result.User.UserId);

                PlayerPrefs.SetString("UserID", result.User.UserId);
                database.Child("users").Child(result.User.UserId).Child("score").SetValueAsync(0);
                database.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);
                

                SceneManagement.Instance.ChangeScene((int)AppScene.HOME);
            }
            else
            {
                FirebaseUser user = auth.CurrentUser;
                user.DeleteAsync();
                if (registerTask.IsCanceled)
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                else if (registerTask.IsFaulted)
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception.Message);
                else
                    Debug.Log("User deleted successfully.");
            }
        }
    }

    private bool ValidUsername(string username)
    {
        this.username = username;
        if (String.IsNullOrEmpty(username))
        {
            DisableUIErrors();
            signUpErrorList[6].gameObject.SetActive(true);
            return false;
        }
        if (String.IsNullOrWhiteSpace(username))
        {
            DisableUIErrors();
            signUpErrorList[6].gameObject.SetActive(true);
            return false;
        }
        else 
            return true;
    }
}