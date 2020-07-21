using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject, ISerializationCallbackReceiver
{
	private List<GameEventListener> listeners = new List<GameEventListener>();

	public void OnAfterDeserialize()
	{}

	public void OnBeforeSerialize()
	{}

	public void EmitGameEvent(string name)
	{
		foreach (GameEventListener listener in listeners)
		{
            listener.OnGameEvent(name);
		}
	}

	public void RegisterListener(GameEventListener listener) 
	{ 
		listeners.Add(listener); 
	}

	public void UnregisterListener(GameEventListener listener) 
	{ 
		listeners.Remove(listener); 
	}
}

public interface GameEventListener
{
	void OnGameEvent(string name);
}
