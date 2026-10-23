using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public TMP_Text scoreText;
    public GameObject WinUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
            heartImages[i].enabled = i < maxHealth;
        }
    }
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowWinUI(bool t)
    {
        WinUI.SetActive(t);
    }
}
