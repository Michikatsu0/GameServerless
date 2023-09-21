using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class UIManagerBase : MonoBehaviour
{
    [SerializeField]
    private Image[] bulletIcons;

    [SerializeField]
    private Text
        scoreLabel,
        timeLabel;

    
    [SerializeField] private TMP_Text scoreText;

    [SerializeField]
    private GameObject gameOverPanel;

    protected abstract PlayerControllerBase PlayerController { get; }
    protected abstract GameControllerBase GameController { get; }

    public void RestartLevel()
    {
        SceneManager.LoadScene((int)AppScene.GAME, LoadSceneMode.Single);
    }

    protected void OnGameOver()
    {
        enabled = false;
        gameOverPanel?.SetActive(true);
    }

    protected void UpdateScoreLabel()
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = PlayerController?.Score.ToString();
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (PlayerController == null)
        {
            Debug.LogError("Can't initialize without player");
            enabled = false;
            scoreLabel.text = "Invalid";
            timeLabel.text = "99:99:99";
        }
        else
        {
            UpdateScoreLabel();
        }

        gameOverPanel?.SetActive(false);
        enabled = true;
        EnableIcon(0);
    }

    // Update is called once per frame
    private void Update()
    {
        if (timeLabel != null)
        {
            float currentTime = GameController.RemainingPlayTime + 1;
            timeLabel.text = currentTime.ToTimeFormatString();
        }
    }

    protected void EnableIcon(int iconIndex)
    {
        switch (iconIndex)
        {
            case 1:

                ToggleUIControl(bulletIcons[1], true);
                ToggleUIControl(bulletIcons[0], false);
                ToggleUIControl(bulletIcons[2], false);
                break;

            case 2:
                ToggleUIControl(bulletIcons[2], true);
                ToggleUIControl(bulletIcons[0], false);
                ToggleUIControl(bulletIcons[1], false);
                break;

            default:
                ToggleUIControl(bulletIcons[0], true);
                ToggleUIControl(bulletIcons[1], false);
                ToggleUIControl(bulletIcons[2], false);
                break;
        }
    }

    private void ToggleUIControl(Graphic uiControl, bool val)
    {
        uiControl.enabled = val;
        uiControl.gameObject.SetActive(val);
    }
}