using UnityEngine;

public class TriggerActive : MonoBehaviour
{
    public GameObject[] ObjectsToActive;
    public bool isActive = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            for (int i = 0; i < ObjectsToActive.Length; i++)
            {
                ObjectsToActive[i].SetActive(isActive);
            }
        }
    }
}
