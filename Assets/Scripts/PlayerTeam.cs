using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField]
    CampTeam team;
    public CampTeam Team { get { return team; } }

    //[SerializeField]
    //CampPlaceSide placeSide;
    //public CampPlaceSide PlaceSice { get { return placeSide; } }

    [SerializeField]
    CampPlacePosition placePosition;
    public CampPlacePosition PlacePosition { get { return placePosition; } }

}
