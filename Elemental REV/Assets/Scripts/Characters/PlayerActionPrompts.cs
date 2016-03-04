using UnityEngine;
using System.Collections;

public class PlayerActionPrompts : MonoBehaviour
{

    private HeroAwareness _heroAwareness;
    private HeroInteract _heroInteract;
    private GameObject _interactPrompt;

	// Use this for initialization
	void Start ()
	{
	    _heroAwareness = GetComponent<HeroAwareness>();
	    _heroInteract = GetComponent<HeroInteract>();
	    _interactPrompt = Instantiate(Resources.Load<GameObject>("A_Button"));
        _interactPrompt.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

	    if (_heroAwareness.CurrentObjectInAwareness)
	    {
	        var interactable = _heroAwareness.CurrentObjectInAwareness.GetComponent<Interactable>();

	        if (interactable && _heroInteract.currentInteractState != PlayerInput.InteractState.BeingHeld && !interactable.IsBeingUsed())
	            DisplayPromptOnTarget(transform.position + Vector3.up*1.5f);
	        else 
	            HidePrompt();
	    }
        else 
	    {
            HidePrompt();
        }
	}

    private void HidePrompt()
    {
        if (_interactPrompt.activeSelf)
            _interactPrompt.SetActive(false);
    }

    private void DisplayPromptOnTarget(Vector3 target)
    {
        if (!_interactPrompt.activeSelf)
            _interactPrompt.SetActive(true);
        
        _interactPrompt.transform.position = target;
    }
}
