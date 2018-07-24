using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableTiles : MonoBehaviour {

	public virtual void OnPlaceTile()
	{
		
		//do something when the tile is placed in the grid
	}

	public virtual string getInfoOnTile (){
		return "";
	}
}
