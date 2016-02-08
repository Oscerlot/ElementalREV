using UnityEngine;
using System.Collections;
using System.Linq;
using System.Net;
using UnityEditor;

public class HeroPushAbility : MonoBehaviour
{

    private HeroMove _heroMove;
    private HeroInteract _heroInteract;
    private Animator _animationControl;


    void Start()
    {
        _heroMove = GetComponent<HeroMove>();
        _heroInteract = GetComponent<HeroInteract>();
        _animationControl = GetComponent<Animator>();
    }



}
