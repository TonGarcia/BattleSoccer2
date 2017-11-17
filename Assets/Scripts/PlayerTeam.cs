using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField]
    private CampTeam team;
    public CampTeam Team { get { return team; } }

    [SerializeField]
    private CampActionAttribute placeAction;
    public CampActionAttribute PlaceAction { get { return placeAction; } }
    //[SerializeField]
    //CampPlaceSide placeSide;
    //public CampPlaceSide PlaceSice { get { return placeSide; } }

    [SerializeField]
    private CampPlaceMarcation placeMarcation;
    public CampPlaceMarcation PlaceMarcation { get { return placeMarcation; } }

}
