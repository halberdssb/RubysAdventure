using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public ParticleSystem healthParticles;
    public ParticleSystem hurtParticles;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public Vector3 startPos;
    public bool allowInput;

    // bool for collecting multishot pick up added - Jeff Stevenson
    public bool hasMultishot;

    // bool for Sludge Spill added- Garrett Oliver
    private bool _inSludgeSpill;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        startPos = transform.position;
        allowInput = true;

        hasMultishot = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (allowInput)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Launch();
            }

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
            }
        }
    }

    void FixedUpdate()
    {
        if (allowInput)
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;

            rigidbody2d.MovePosition(position);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);
            hurtParticles.Play();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth <= 0)
        {
            UIManager.main.OnDeath();
        }
    }

    void Launch()
    {
        // changes to launch function to add multishot functionality - Jeff Stevenson
        // normal launch
        if (!hasMultishot)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);
        }
        // START NEW MULTISHOT CODE - made by Jeff Stevenson
        else
        {
            // instantiate three projectiles
            GameObject projectile1Object = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            GameObject projectile2Object = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            GameObject projectile3Object = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            // get projectile components for each object
            Projectile projectile1 = projectile1Object.GetComponent<Projectile>();
            Projectile projectile2 = projectile2Object.GetComponent<Projectile>();
            Projectile projectile3 = projectile3Object.GetComponent<Projectile>();

            // calculate look angle from vector2 lookdirection and convert to upper and lower angles for multishot
            float lookAngle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg * -1;
            lookAngle += 90;
            Vector2 upperAngle = new Vector2(Mathf.Cos((lookAngle + 10) * (Mathf.PI / 180)), Mathf.Sin((lookAngle + 10) * (Mathf.PI / 180)));
            Vector2 lowerAngle = new Vector2(Mathf.Cos((lookAngle - 10) * (Mathf.PI / 180)), Mathf.Sin((lookAngle - 10) * (Mathf.PI / 180)));

            Debug.Log("Look angle: " + lookAngle);
            Debug.Log("Upper angle: " + upperAngle);
            Debug.Log("Lower angle: " + lowerAngle);

            projectile1.Launch(lookDirection, 300);
            projectile2.Launch(upperAngle, 300);
            projectile3.Launch(lowerAngle, 300);
        }

        // END NEW MULTISHOT CODE - made by Jeff Stevenson

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag ("SludgeSpill"))
        {
            _inSludgeSpill=true;
            speed=speed /2;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
         if(other.CompareTag ("SludgeSpill"))
         {
            _inSludgeSpill=false;
            speed=speed *2;

         }
    }


}

