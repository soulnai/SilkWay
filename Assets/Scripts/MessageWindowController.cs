using UnityEngine;
using System.Collections;
using TMPro;
using EnumSpace;

public class MessageWindowController : MonoBehaviour
{

    public GameObject EventName;
    public GameObject EventDescription;
    public GameObject Button;
    public GameObject Fight;
    public GameObject Run;
    public GameObject Trade;
    public GameObject Accept;
    public GameStateManager GM;

    // Use this for initialization
    void Start()
    {
        GM = GameObject.Find("GM").GetComponent<GameStateManager>();
        TextMeshProUGUI textmeshProName = EventName.GetComponent<TextMeshProUGUI>();
        textmeshProName.SetText("Test Event Name");
        TextMeshProUGUI textmeshProDesc = EventDescription.GetComponent<TextMeshProUGUI>();
        textmeshProDesc.SetText("Test Event Description");
    }

    // Update is called once per frame
    void Update()
    {

    }
    /*
    public void CloseWindow()
    {
        Button.SetActive(false);
        gameObject.SetActive(false);
        GM.gamestate = GameStates.GlobalMap;
        if (Event.LaunchNextEvent)
        {
            foreach (RandomEvent ev in AllEvents)
            {
                if (ev.type == Event.NextEvent)
                {
                    LaunchEvent(ev);
                    break;
                }
            }

        }
    }*/
}
