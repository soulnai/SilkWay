using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Companion {

    public string CompanionName = "companion";

    public Dictionary<EnumSpace.EventType, float> eventReactions = new Dictionary<EnumSpace.EventType, float>();

    public Dictionary<EnumSpace.EventType, string> eventStringsSuccess = new Dictionary<EnumSpace.EventType, string>();
    public Dictionary<EnumSpace.EventType, string> eventStringsLoss = new Dictionary<EnumSpace.EventType, string>();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
