using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public abstract class GameControllerBase : MonoBehaviour
{
    [SerializeField]
    private float playTime = 60F;
    [SerializeField] TMP_Text uRScoreLabel;
    public float RemainingPlayTime { get; private set; }
    private int score;
    protected abstract PlayerControllerBase PlayerController { get; }
    protected abstract UIManagerBase UiManager { get; }

    protected abstract ObstacleSpawnerBase Spawner { get; }

    protected abstract void OnScoreChanged(int scoreAdd);

    protected void OnObstacleDestroyed(int hp)
    {
        OnScoreChanged(hp);
    }

    protected virtual void SetGameOver()
    {
        enabled = false;
    }

    private void Start()
    {
        RemainingPlayTime = playTime;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        RemainingPlayTime -= Time.deltaTime;

        if (RemainingPlayTime <= 0F)
        {
            RemainingPlayTime = 0F;
            SetGameOver();
            uRScoreLabel.text = "Your Score: " + PlayerController?.Score.ToString();
            var auth = FirebaseAuth.DefaultInstance;
            SetScore(auth);
        }
    }

    private void SetScore(FirebaseAuth auth)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + auth.CurrentUser.UserId + "/score")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
                Debug.Log(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string _score = "" + snapshot.Value;
                score = int.Parse(_score);
                if ((int)PlayerController?.Score > score)
                       ScoreManager.Instance.WriteNewScore(auth, (int)PlayerController?.Score);
            }
        });
    }


}