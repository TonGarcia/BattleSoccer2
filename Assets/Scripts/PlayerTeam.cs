using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class PlayerTeam : MonoBehaviour
{
    [SerializeField]
    CampPlaceSide placeSide;
    public CampPlaceSide PlaceSice { get { return placeSide; } }

    [SerializeField]
    CampPlacePosition placePosition;
    public CampPlacePosition PlacePosition { get { return placePosition; } }
   
}
