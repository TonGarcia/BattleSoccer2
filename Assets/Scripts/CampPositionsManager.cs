using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;



public class CampPositionsManager : MonoBehaviour
{
   

    [SerializeField]
    private GameObject LeftSide;
    [SerializeField]
    private GameObject RightSide;

    private Dictionary<string, CampPosition> positions;

    private void Awake()
    {
        
        positions = new Dictionary<string, CampPosition>();
        LeftSide.SetActive(true);
        RightSide.SetActive(true);
    }
    public void AddPosition(CampPosition position)
    {
        if (position == null)
            return;


        positions.Add(position.ToKey(), position);
    }
    public CampPosition GetPosition(CampPlaceSide side, CampPlaceMarcation cpos, CampPlaceType ctype)
    {
        CampPosition pos = null;
        string key = CampPosition.GetKey(side, cpos, ctype);

        positions.TryGetValue(key, out pos);

        return pos;
    }


}
