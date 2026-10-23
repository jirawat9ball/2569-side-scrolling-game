using UnityEngine;
using UnityEngine.SceneManagement;
public class TriggerWin : MonoBehaviour
{
    public GameObject Effect;
    public float delayTime = 1f;
    public string nextSceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            Rigidbody2D rb2d = collision.GetComponent<Rigidbody2D>();
            if (player != null)
            {
                player.enabled = false;
                rb2d.Sleep();
                GameObject g = Instantiate(Effect, transform.position, Quaternion.identity);
                Invoke(nameof(LoadNextScene), delayTime);
                Destroy(g, 1f);
            }
            Gamemanager.instance.ShowWinUI(true);
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
        Gamemanager.instance.ShowWinUI(false);
    }
}
