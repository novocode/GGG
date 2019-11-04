using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Agent : MonoBehaviour
{
    public BoardManager GGG_Board;
	private Tilemap resourceTileHolder;

    // Start is called before the first frame update
    void Start()
    {
		GGG_Board = this.GetComponent<BoardManager>();
		if(GGG_Board == null)
			Debug.Log("GGG_Board is null");
		else
			Debug.Log("GGG_Board is not null");
		resourceTileHolder = (GGG_Board.GetComponentsInChildren<Tilemap>())[0];	        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void pathing()
    {

    }

    /*bool Move(int row, int column)
    {
        //The following are true:
        //The agent moves to an empty tile
        //The enemy agent moves into a human occupied tile
        //The following are false:
        //The enemy agent moves into a enemy agent occupied tile
        //The enemy attempts to move into a resource tile (May give ghost special powers here, so may be true)
        //The enemy attempts to move into a barrier tile (May be true if barrier breaks)
        //The enemy attempts to move on an outer wall
        TileData tempData = new TileData();
        Vector3Int tempPosition = new Vector3Int(row, column, 0);
        ITilemap resourceTileHolderI = null;
        resourceTileHolder.GetTile(tempPosition).GetTileData(tempPosition, resourceTileHolderI, ref tempData);

        if(tempData.sprite == resource)
        {
            if(this == ghost)
            {
                if(tempData != ghostbarrier)
                    return true;
            }
        }
        if(tempData.sprite == barrier)
        {

        }
        if(tempData.sprite == outerWall)
        {

        }
        return false;
    }*/
}