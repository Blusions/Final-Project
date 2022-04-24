using UnityEngine;
using UnityEngine.UI;

public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 4.0f;
    public GameObject dialogBox;
    float timerDisplay;

    public AudioSource audioSource;
    public AudioClip talk;
    
    void Start()
    {
        dialogBox.SetActive(false);
        timerDisplay = -1.0f;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }
    
    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
        audioSource.PlayOneShot(talk);
    }
    
}
