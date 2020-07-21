using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool allowRight = true;
    public bool allowLeft = true;
    public bool allowUp = true;
    public bool allowDown = true;
    public bool enableRun = true;
    public List<AudioClip> footsteps;

    private float currentSpeed = 0f;
    private float baseSpeed = 0.5f;
    private float acceleration = 0.05f;
    private float maxSpeed = 4f;
    private float maxSpeedRunning = 10f;
    private float actualMaxSpeed = 0f;

    private float footstepTimer = 0f;
    private float footstepTimerMax = 1f;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = baseSpeed;

        this.audioSource = this.GetComponent<AudioSource>();
        this.footstepTimer = this.footstepTimerMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableRun) {
            this.actualMaxSpeed = this.maxSpeedRunning;
        } else {
            this.actualMaxSpeed = this.maxSpeed;
        }

        bool moved = false;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 position = this.transform.position;

        if (horizontal > 0.1f && allowRight)
        {
            position.x += Time.deltaTime * currentSpeed;
            moved = true;
        }

        if (horizontal < -0.1f && allowLeft)
        {
            position.x -= Time.deltaTime * currentSpeed;
            moved = true;
        }

        if (vertical > 0.1f && allowUp)
        {
            position.y += Time.deltaTime * currentSpeed;
            moved = true;
        }

        if (vertical < -0.1f && allowDown)
        {
            position.y -= Time.deltaTime * currentSpeed;
            moved = true;
        }

        this.transform.position = position;

        if (moved)
        {
            if (currentSpeed < this.actualMaxSpeed)
            {
                currentSpeed += acceleration;
            }

            this.footstepTimer += Time.deltaTime;

            if (this.footstepTimer >= this.footstepTimerMax)
            {
                AudioClip footstepSound = this.footsteps[Random.Range(0, this.footsteps.Count)];
                this.audioSource.PlayOneShot(footstepSound);
                this.footstepTimer = 0f;
            }
        }
        else
        {
            currentSpeed = baseSpeed;
            this.footstepTimer = this.footstepTimerMax;
        }

        this.footstepTimerMax = 2f / currentSpeed;
    }
}
