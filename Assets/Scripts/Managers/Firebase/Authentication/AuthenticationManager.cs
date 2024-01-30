using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour
{
    private GameObject errorUI;
    
    private TMP_Text tMPErrorUI;
    private Image imageErrorUI;
    private Color colorErrorUI;

    private Button loginButton;
    private Button signUpButton;
    private Button restoreButton;
    private Button loginGuestButton;
    
    private Coroutine loginCoroutine;
    private Coroutine signInCoroutine;
    private Coroutine loginGuestCoroutine;
    private Coroutine getUsernameCoroutine;
    private Coroutine restorePasswordCoroutine;

    DatabaseReference mDatabase;

    private void Awake()
    {
        loginButton = GameObject.Find("Button_Login").GetComponent<Button>();
        signUpButton = GameObject.Find("Button_SignIn").GetComponent<Button>();
        restoreButton = GameObject.Find("Button_Restore").GetComponent<Button>();
        loginGuestButton = GameObject.Find("Button_LoginGuest").GetComponent<Button>();
        
        errorUI = GameObject.Find("Error_Window_UI");
        errorUI.SetActive(false);

        imageErrorUI = errorUI.GetComponent<Image>();
        tMPErrorUI = errorUI.GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        UIElementsManager.Instance.DisableUI();
        loginButton.onClick.AddListener(HandleLoginButtonClicked);
        signUpButton.onClick.AddListener(HandleRegisterButtonClicked);
        restoreButton.onClick.AddListener(HandleRestoreButtonClicked);
        loginGuestButton.onClick.AddListener(HandleLoginGuestButtonClicked);
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            SceneManager.LoadScene((int)AppScene.HOME);
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

        loginCoroutine = StartCoroutine(SignInUser(email, password));
    }

    private void HandleRegisterButtonClicked()
    {
        string email = GameObject.Find("InputField_Email").GetComponent<TMP_InputField>().text;
        string username = GameObject.Find("InputField_Username").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputField_Password").GetComponent<TMP_InputField>().text;

        signInCoroutine = StartCoroutine(RegisterUser(email, username, password));
    }

    private void HandleLoginGuestButtonClicked()
    {
        loginGuestCoroutine = StartCoroutine(SingInWithAnonymous());
    }

    private IEnumerator SingInWithAnonymous()
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SignInAnonymouslyAsync();

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.IsCanceled)
            Debug.LogError("SignInAnonymouslyAsync was canceled.");
        else if (registerTask.IsFaulted)
            Debug.LogError("SendPasswordResetEmailAsync encountered an error:" + registerTask.Exception);

        AuthResult result = registerTask.Result;
        PlayerPrefs.SetString("UserID", result.User.UserId);
        mDatabase.Child("users").Child(result.User.UserId).Child("score").SetValueAsync(0);
        mDatabase.Child("users").Child(result.User.UserId).Child("username").SetValueAsync("#" + UnityEngine.Random.Range(1000, 9999));
        SceneManagement.Instance.ChangeScene((int)AppScene.HOME);
        PlayerPrefs.Save();
    }

    private IEnumerator RestorePassword(string email)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var restoreTask = auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => restoreTask.IsCompleted);

        errorUI.SetActive(false);

        if (restoreTask.IsCanceled)
            Debug.LogError("SendPasswordResetEmailAsync was canceled.");
        else if (restoreTask.IsFaulted)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + restoreTask.Exception);
            errorUI.SetActive(true);
            colorErrorUI = Color.HSVToRGB(0, 80, 100);
            colorErrorUI.a = 0.9f;
            imageErrorUI.color = colorErrorUI;
            tMPErrorUI.text = restoreTask.Exception.Message;
        }
        else
        {
            errorUI.SetActive(true);
            Color colorErrorUI = Color.green;
            colorErrorUI.a = 0.8f;
            imageErrorUI.color = colorErrorUI;
            tMPErrorUI.text = "Password reset email sent successfully.";
            Debug.Log("Password reset email sent successfully.");
        }
    }

    private IEnumerator SignInUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        errorUI.SetActive(false);

        if (registerTask.IsCanceled)
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception.Message);
            errorUI.SetActive(true);
            colorErrorUI = Color.HSVToRGB(0, 80, 100);
            colorErrorUI.a = 0.9f;
            imageErrorUI.color = colorErrorUI;
            tMPErrorUI.text = registerTask.Exception.Message;
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
        .GetValueAsync().ContinueWith(task => {
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

        errorUI.SetActive(false);

        if (registerTask.IsCanceled)
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception.Message);
            errorUI.SetActive(true);
            colorErrorUI = Color.HSVToRGB(0, 80, 100);
            colorErrorUI.a = 0.9f;
            imageErrorUI.color = colorErrorUI;
            tMPErrorUI.text = registerTask.Exception.Message;
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
                mDatabase.Child("users").Child(result.User.UserId).Child("score").SetValueAsync(0);
                mDatabase.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);
                

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
        errorUI.SetActive(false);

        if (String.IsNullOrEmpty(username))
        {
            errorUI.SetActive(true);
            colorErrorUI = Color.HSVToRGB(0, 80, 100);
            colorErrorUI.a = 0.9f;
            imageErrorUI.color = colorErrorUI;
            tMPErrorUI.text = "One or more errors occurred. (An username must be provided.)";
            
            return false;
        }
        if (String.IsNullOrWhiteSpace(username))
        {
            errorUI.SetActive(true);
            colorErrorUI = Color.HSVToRGB(0, 80, 100);
            colorErrorUI.a = 0.9f;
            imageErrorUI.color = colorErrorUI;
            tMPErrorUI.text = "One or more errors occurred. (Username has white Space.)";
            
            return false;
        }
        else 
            return true;
    }
}