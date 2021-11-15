using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RubyController : MonoBehaviour
{

    public float speed = 4;
    public Text loseText;
    public Text score;
    public Text gameOver;
    private bool gameRestart;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
   
    public ParticleSystem hitParticle;

    private int scoreValue = 0;


    public GameObject projectilePrefab;
    float horizontal;
    float vertical;

    public AudioClip hitSound;
    public AudioClip shootingSound;
    public AudioSource winMusic;
    public AudioSource loseMusic;
    public AudioSource backgroundMusic;

    public int health
    {
        get { return currentHealth; }
    }
    

    Rigidbody2D rigidbody2d;
    Vector2 currentInput;


    int currentHealth;
    float invincibleTimer;
    bool isInvincible;


    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);


    AudioSource audioSource;

    void Start()
    {
        backgroundMusic.Play();
        rigidbody2d = GetComponent<Rigidbody2D>();
        gameRestart = false;
       
     
        invincibleTimer = -1.0f;
        currentHealth = maxHealth;
        score.text = scoreValue.ToString();

        animator = GetComponent<Animator>();


        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
            
            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }


        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        currentInput = move;




        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);



        if (Input.GetKeyDown(KeyCode.C))
            LaunchProjectile();


        if (Input.GetKeyDown(KeyCode.X))
        {

            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f,  1 << LayerMask.NameToLayer("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;

        position = position + currentInput * speed * Time.deltaTime;
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }


        rigidbody2d.MovePosition(position);
    }


    public void ChangeHealth(int amount)
    {
  
        
        if (amount < 0)
        {
            
            if (isInvincible)
                return;

            

            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
           

            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth == 0)
        {
            loseMusic.Play();
            backgroundMusic.Stop();
            gameRestart = true;
            if (Input.GetKey(KeyCode.R))

            {

                if (gameRestart == true)

                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

                }

            }
            loseText.text = "YOU LOSE! Game Created by: Cristian del Real (Press R to restart) ";
            //Destroy(gameObject);
            
        }

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

 

    void LaunchProjectile()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(shootingSound);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

       
    }


    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeScore(int sAmount)
    {
        scoreValue += sAmount;
        score.text = "" + scoreValue;
        if (scoreValue == 4)
        {
            winMusic.Play();
            backgroundMusic.Stop();
            gameRestart = true;
            score.text = null;
            gameOver.text = "You Win! Game Created By Cristian del Real (press R to restart)" +
                "";
            if (gameRestart == true) 

            {

                if (Input.GetKey(KeyCode.R))

                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

                }

            }
        }
        
    }

}
