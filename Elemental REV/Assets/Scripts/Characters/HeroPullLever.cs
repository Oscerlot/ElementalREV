using UnityEngine;
using System.Collections;

public class HeroPullLever : MonoBehaviour
{
    private HeroInteract _heroInteract;
    private HeroMove _heroMove;
    private Animator _animationControl;
    private Lever _lever;

	// Use this for initialization
	void Start ()
	{
	    _heroInteract = GetComponent<HeroInteract>();
	    _heroMove = GetComponent<HeroMove>();
	    _animationControl = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	    CheckForLevers();
	}


    //TODO: Check for levers and check for pushables seem awfully similar dont you think? Do something about it mate...
    private void CheckForLevers()
    {
        if (_heroInteract.CurrentInteractable && _heroInteract.CurrentInteractable.tag.Equals("Lever"))
        {
            if (!_lever)
            {
                _lever = (Lever)_heroInteract.CurrentInteractable;
                _lever.ToggleLever();
                _animationControl.SetTrigger("PullLever");
            }
        }
        if (_lever && !_lever.IsBeingUsed())
        {
            _heroInteract.DetachHero();
            _lever = null;
        }
    }
}
