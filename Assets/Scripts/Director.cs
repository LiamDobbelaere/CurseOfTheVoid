using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public GameEvent gameEvent;
    public NarrationEntry[] narrationClips;
    public SpawnableEntry[] spawnablePrefabs;
    public PlayerController player;
    public int currentDirectorStepIndex = -1;

    private DirectorStep[] steps =
    {
        new EntryDirectorStep(),
        new IntroDirectorStep(),
        new OkayStopDirectorStep(),
        new SoundChoiceDirectorStep(),
        new PostChoiceDirectorStep()
    };

    private AudioSource audioNarration;

    private Dictionary<string, NarrationEntry> narrationDict;
    private Dictionary<string, SpawnableEntry> spawnablesDict;

    // Start is called before the first frame update
    void Start()
    {
        this.audioNarration = this.GetComponent<AudioSource>();

        this.narrationDict = new Dictionary<string, NarrationEntry>();
        foreach (NarrationEntry entry in narrationClips)
        {
            this.narrationDict.Add(entry.name, entry);
        }

        this.spawnablesDict = new Dictionary<string, SpawnableEntry>();
        foreach (SpawnableEntry entry in spawnablePrefabs)
        {
            this.spawnablesDict.Add(entry.name, entry);
        }

        foreach (DirectorStep step in this.steps)
        {
            step.director = this;
        }

        NextDirectorStep();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentDirectorStepIndex >= 0)
        {
            steps[currentDirectorStepIndex].Update();
        }
    }

    public void PlayNarration(string name)
    {
        this.audioNarration.PlayOneShot(this.narrationDict[name].clip);
    }

    public bool IsNarrationFinished()
    {
        return !this.audioNarration.isPlaying;
    }

    public GameObject GetSpawnable(string name)
    {
        return this.spawnablesDict[name].prefab;
    }

    public void NextDirectorStep()
    {
        if (currentDirectorStepIndex >= 0)
        {
            steps[currentDirectorStepIndex].Exit();
        }

        if (currentDirectorStepIndex + 1 < steps.Length)
        {
            currentDirectorStepIndex++;

            steps[currentDirectorStepIndex].Enter();
        }
        else
        {
            Debug.Log("Reached limit of director steps!");
        }

    }
}

[System.Serializable]
public class NarrationEntry
{
    public string name;
    public AudioClip clip;
}

[System.Serializable]
public class SpawnableEntry
{
    public string name;
    public GameObject prefab;
}

public class EntryDirectorStep : DirectorStep
{
    public override void Enter()
    {
        director.PlayNarration("entry");
    }

    public override void Update()
    {
        if (director.IsNarrationFinished())
        {
            director.NextDirectorStep();
        }
    }
}

public class IntroDirectorStep : DirectorStep
{
    private Vector3 playerStartPosition;
    private int maxDistance;
    private bool activated;

    public override void Enter()
    {
        this.playerStartPosition = this.director.player.transform.position;
        this.maxDistance = 64;
        this.activated = false;
        director.player.allowRight = true;
    }

    public override void Update()
    {
        if (!this.activated && (this.director.player.transform.position - this.playerStartPosition).sqrMagnitude > maxDistance)
        {
            this.activated = true;
            director.PlayNarration("intro");
        }

        if (this.activated)
        {
            if (director.IsNarrationFinished())
            {
                director.NextDirectorStep();
            }
        }
    }
}

public class OkayStopDirectorStep : DirectorStep
{
    public override void Enter()
    {
        director.PlayNarration("okay_stop");
        director.player.allowRight = false;
    }

    public override void Update()
    {
        if (director.IsNarrationFinished())
        {
            director.NextDirectorStep();
        }
    }
}

public class SoundChoiceDirectorStep : DirectorStep, GameEventListener
{
    private string touchedSound;
    private GameObject choiceObject;
    private bool explainingChoice;

    public override void Enter()
    {
        director.player.allowRight = true;
        director.player.allowLeft = true;
        touchedSound = null;
        explainingChoice = false;

        director.gameEvent.RegisterListener(this);

        choiceObject = GameObject.Instantiate(
            director.GetSpawnable("sound_choice"),
            director.player.transform.position,
            Quaternion.identity);
    }

    public override void Update()
    {
        if (touchedSound != null && director.IsNarrationFinished() && !explainingChoice)
        {
            director.PlayNarration(touchedSound);
            explainingChoice = true;
        }

        if (explainingChoice && director.IsNarrationFinished())
        {
            director.NextDirectorStep();
        }
    }

    public override void Exit()
    {
        director.gameEvent.UnregisterListener(this);
    }

    public void OnGameEvent(string name)
    {
        touchedSound = name;

        GameObject.Destroy(choiceObject);

        director.PlayNarration("choose_sound");
    }
}

public class PostChoiceDirectorStep : DirectorStep
{
    public override void Enter()
    {
        director.PlayNarration("post_choice");
    }

    public override void Update()
    {
        if (director.IsNarrationFinished())
        {
            director.NextDirectorStep();
        }
    }
}


public class DirectorStep
{
    public Director director;

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}