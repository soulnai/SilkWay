using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RandomEvent {

    public EnumSpace.EventType type;

    public string StartMessageWindowHeader;
    public string StartMessageWindowText;

    public string EndMessageWindowHeader;
    public string EndMessageWindowTextSuccess;
    public string EndMessageWindowTextFailure;

    public Dictionary<string, float> companions = new Dictionary<string, float>();

    public bool LaunchNextEvent = false;

    public EnumSpace.EventType NextEvent;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
