using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RubyController : MonoBehaviour
{
    // ========= MOVEMENT =================
    public float speed = 4;
    
    // ======== HEALTH ==========
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public Transform respawnPosition;
    public ParticleSystem hitParticle;
    public ParticleSystem healthParticle;
    
    // ======== PROJECTILE ==========
    public GameObject projectilePrefab;

    // ======== AUDIO ==========
    public AudioClip hitSound;
    public AudioClip shootingSound;
    public AudioClip loseMusic;
    public AudioClip winMusic;
    public AudioSource audioSource;    

    // ======== HEALTH ==========
    public int health
    {
        get { return currentHealth; }
    }
    
    // =========== MOVEMENT ==============
    Rigidbody2D rigidbody2d;
    Vector2 currentInput;
    
    // ======== HEALTH ==========
    int currentHealth;
    float invincibleTimer;
    bool isInvincible;
   
    // ==== ANIMATION =====
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    //========= GAME STATE ==============

    public bool gameOver = false;
    public static bool staticVar = false;

    // =========== SCORE =================
    public Text score;
    private int scoreValue;
    public GameObject FixedCounter;
    public GameObject lvl1TextObject;
    public GameObject loseTextObject;
    public GameObject winTextObject;

    private static int level = 1;


    // ========= Score =============
    public void ScoreCount(int number)
    {    
      scoreValue = scoreValue + number;
      score.text = "Robots Fixed: " + scoreValue.ToString();

      if (level == 1)
      {
          if (scoreValue == 4)
          {
                lvl1TextObject.SetActive(true);
                gameOver = true;
          }  
      }

      if (level == 2)
      {
          if(scoreValue == 4)
          {
              winTextObject.SetActive(true);
              gameOver = true;
              audioSource.Stop(); 
          }     
      }
    }

    void Start()
    {
        // =========== MOVEMENT ==============
        rigidbody2d = GetComponent<Rigidbody2D>();
                
        // ======== HEALTH ==========
        invincibleTimer = -1.0f;
        currentHealth = maxHealth;
        
        // ==== ANIMATION =====
        animator = GetComponent<Animator>();
        
        // ==== AUDIO =====
        audioSource = GetComponent<AudioSource>();

        //===== UI TEXT ======
        scoreValue = 0;
        score.text = "Robots Fixed: " + scoreValue.ToString();

        lvl1TextObject.SetActive(false);
        loseTextObject.SetActive(false);
        winTextObject.SetActive(false);
        
    }

    void Update()
    {

        // ================= HEALTH ====================
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        // ============== MOVEMENT ======================
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
                
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        currentInput = move;


        // ============== ANIMATION =======================

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // ============== PROJECTILE ======================

        if (Input.GetKeyDown(KeyCode.C))
            LaunchProjectile();
        
        // ======== DIALOGUE ==========
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, 1 << LayerMask.NameToLayer("NPC"));
            if (hit.collider != null)
            {
                if (scoreValue >= 4)
                {
                    level = 2;
                    SceneManager.LoadScene("Level2");
                    scoreValue = 4;
                }

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }  
            }
        }
 
            if (Input.GetKey(KeyCode.R))
            {
                if (gameOver == true)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }

            

    }

    void FixedUpdate()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        Vector2 position = rigidbody2d.position;
        
        position = position + currentInput * speed * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);


    }

    // ===================== HEALTH ==================
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        { 
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);

            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (amount > 0)
        {
            Instantiate(healthParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        if(currentHealth == 0)
        {
            gameOver = true;
            staticVar = true;
            loseTextObject.SetActive(true);
            speed = 0.0f;
            rigidbody2d.simulated = false;
            audioSource.clip = loseMusic;
            audioSource.Play();

        }
        
        UIHealthBar.Instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    // =============== PROJECTICLE ========================
    void LaunchProjectile()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        
        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(shootingSound);
    }
    
    // =============== SOUND ==========================
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }



}
