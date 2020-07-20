using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public NarrationEntry[] narrationClips;

    private DirectorStep[] steps =
    {
        new DirectorStepTest()
    };

    private AudioSource audioNarration;

    private Dictionary<string, NarrationEntry> narrationDict;

    // Start is called before the first frame update
    void Start()
    {
        this.audioNarration = this.GetComponent<AudioSource>();

        this.narrationDict = new Dictionary<string, NarrationEntry>();
        foreach (NarrationEntry entry in narrationClips)
        {
            this.narrationDict.Add(entry.name, entry);
        }

        //PlayNarration("entry");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayNarration(string name)
    {
        this.audioNarration.PlayOneShot(this.narrationDict[name].clip);
    }
}

[System.Serializable]
public class NarrationEntry
{
    public string name;
    public AudioClip clip;
}

public class DirectorStepTest : DirectorStep
{
}

public class DirectorStep
{
    public Director director;

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}