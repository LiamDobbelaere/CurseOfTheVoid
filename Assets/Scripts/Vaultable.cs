using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaultable : MonoBehaviour, GameEventListener
{
    public GameEvent gameEvent;
    public AudioClip[] audioClips;

    private bool isInside = false;
    private PlayerController player;
    private float graceTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<AudioSource>().clip = this.audioClips[
                Random.Range(0, this.audioClips.Length)
            ];
        this.GetComponent<AudioSource>().Play();
        this.gameEvent.RegisterListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.graceTime > 0f)
        {
            this.graceTime -= Time.deltaTime;

            if (this.graceTime <= 0f)
            {
                this.player.allowRight = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        this.player = col.GetComponent<PlayerController>();
        this.graceTime = 2f;
        isInside = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        isInside = false;
    }

    public void OnGameEvent(string name)
    {
        if (name == "vault" && isInside)
        {
            this.player.allowRight = true;
            this.gameEvent.EmitGameEvent("vault_success");
            this.gameEvent.UnregisterListener(this);
            Destroy(this.gameObject);
        }
    }
}
