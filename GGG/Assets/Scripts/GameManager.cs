using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
	public static GameManager GManager { get; private set;}
	public BoardManager GGG_Board;
	public Tilemap baseTileHolder;
	public Tilemap resourceTileHolder;
	public Camera mycam;
	
	private int monster = 0;
	private int level = 1;
	
    // Initialization
    void Awake()
    {
		if(GManager == null)
			GManager = this;
		else
			Debug.Log("Warning: multiple " + this + " in scene!");
		
        GGG_Board = GetComponent<BoardManager>();
		//baseTileHolder = GGG_Board.transform.GetChild(0).GetComponent<Tilemap>();
		//resourceTileHolder = GGG_Board.transform.GetChild(1).GetComponent<Tilemap>();	
		//baseTileHolder = gameObject.GetComponent("Tilemap_BaseBoard") as Tilemap;
		//resourceTileHolder = gameObject.GetComponent("Tilemap_Resources") as Tilemap;
		InitGame();
    }

	void InitGame()
	{
		GGG_Board.SetupScene(monster, level);
	}
	
    // Update is called once per frame
    void Update()
    {
		//Camera mycam = this.GetComponent<Camera>();
		//float speed = 3f;
		//transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, mycam.nearClipPlane)), Vector3.up);
    }
}
