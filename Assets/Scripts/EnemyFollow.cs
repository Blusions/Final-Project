using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{

    public GameObject slime;

    private GameObject player;

    public float speed = 0.75f;

    Animator anim;

    Rigidbody2D rigidbody2d;
    
    private RubyController RubyController;

    AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("RubyController");

        anim = GetComponent<Animator>();

        rigidbody2d = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Swarm();
    }

    private void Swarm()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        RubyController controller = other.collider.GetComponent<RubyController>();
		
		if(controller != null)
			controller.ChangeHealth(-2);
    }

    public void Fix()
    {

        rigidbody2d.simulated = false;

        audioSource.Stop();
        audioSource.PlayOneShot(hitSound);
        audioSource.PlayOneShot(deathSound);

        speed = 0;

        if(RubyController != null)
        {
            RubyController.ScoreCount(1);
        }
    }
}
