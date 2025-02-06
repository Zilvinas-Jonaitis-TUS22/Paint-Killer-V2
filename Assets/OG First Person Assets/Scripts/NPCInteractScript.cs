using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCInteractScript : MonoBehaviour
{
    public bool InDialogue;
    private StarterAssetsInputs _inputs;
    private FirstPersonController _firstPersonController;
    private ShootingScript _shootingScript;
    public Transform cam;
    public float interactDistance;
    public LayerMask interactLayerMask;
    public GameObject interactGuide;
    public GameObject dialogueBox;
    public GameObject dialogueText;
    public int dialogueStringPos;
    private TextMeshProUGUI textMeshProUGUI;
    private string[] _dialogueString;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<StarterAssetsInputs>();
        _firstPersonController = GetComponent<FirstPersonController>();
        _shootingScript = GetComponent<ShootingScript>();
        textMeshProUGUI = dialogueText.GetComponent<TextMeshProUGUI>();
        InDialogue = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, interactDistance, interactLayerMask) && !InDialogue)
        {
            interactGuide.SetActive(true);
            if (_inputs.interact)
            {
                InteractStart(hit.transform.gameObject);
                _inputs.interact = false;
            }
        } else
        {
            interactGuide.SetActive(false);
        }
        if (InDialogue && timer >= 0.1)
        {
            InteractUpdate();
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    public void InteractStart(GameObject npc)
    {
        NPCDialogue dialogue = npc.GetComponent<NPCDialogue>();
        if (dialogue != null)
        {
            InDialogue = true;  
            _dialogueString = dialogue.dialogueString;
            dialogueStringPos = 0;
            textMeshProUGUI.text = _dialogueString[dialogueStringPos];

            _firstPersonController.enabled = false;
            _shootingScript.enabled = false;
            dialogueBox.SetActive(true);
            dialogueText.SetActive(true);
        } else
        {
            Debug.Log("No Dialogue Script Attached");
        }
    }

    public void InteractUpdate()
    {

        if (_inputs.interact || _inputs.Shoot)
        {
            dialogueStringPos += 1;
            if(dialogueStringPos >= _dialogueString.Length)
            {
                InteractEnd();
            } else
            {
                textMeshProUGUI.text = _dialogueString[dialogueStringPos];

            }
            _inputs.interact = false;
            _inputs.Shoot = false;
        }

    }

    public void InteractEnd()
    {
        InDialogue = false;
        _firstPersonController.enabled = true;
        _shootingScript.enabled = true;
        dialogueBox.SetActive(false);
        dialogueText.SetActive(false);
    }
}
