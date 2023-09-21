using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private bool resetPlayerPrefs;
    private RectTransform rectTransform;
    private Vector2 rectTransfromVector = new Vector2(0f, 50);

    void Awake()
    {
        rectTransform = GameObject.Find("InputField_Password").GetComponent<RectTransform>();

        if (resetPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("NewUser", 0);
        }
    }

    public void RegisterButton()
    {
        rectTransform.anchoredPosition -= rectTransfromVector;
    }

    public void ReturnButton()
    {
        rectTransform.anchoredPosition += rectTransfromVector;
    }

}
