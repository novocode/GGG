using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
//using System.Math;

//https://www.redblobgames.com/grids/hexagons/
//http://gojko.github.io/hexgridwidget/

//https://www.quora.com/What-is-the-centre-of-mass-of-an-equilateral-triangular-lamina-having-side-length-l
//https://www.quora.com/What-is-the-centroid-of-equilateral-triangle
// See above website for information
// Need to apply Pythagorean theorem to find centroid of triangle
// a^2 + b^2 = c^2
// Need to find c for the amount of rows
// a^2 = (L/2)^2
// b^2 = x^2 = ((L*sqr(3))/6)^2
// (L/2)^2 + ((L*sqr(3))/6)^2 = c^2
// c = sqr((L/2)^2 + ((L*sqr(3))/6)^2)
// example: L = 15
// c = sqr((15/2)^2 + ((15*sqr(3))/6)^2)
// c = sqr((7.5)^2 + ((15*1.7320508075688772935274463415059‬)/6)^2)
// c = sqr((7.5)^2 + ((25.980762113533159402911695122588‬)/6)^2)
// c = sqr(56.25 + (4.3301270189221932338186158537647‬)^2)
// c = sqr(56.25 + 18.75)
// c = sqr(75)
// c = 8.6602540378443864676372317075294 (I need at least 9 or 10 based on the length of 15.)
// So do I round up?
// example: L = 30
// c = sqr((30/2)^2 + ((30*sqr(3))/6)^2)
// c = sqr((15)^2 + ((30*1.7320508075688772935274463415059‬)/6)^2)
// c = sqr((15)^2 + ((51.961524227066318805823390245176)/6)^2)
// c = sqr(225 + (8.6602540378443864676372317075294‬)^2)
// c = sqr(225 + 75)
// c = sqr(300)
// c = 17.320508075688772935274463415059 (I need at least 9 or 10 based on the length of 15.)
// So do I round up?
// Using trig to find the distance for X
// Sin(A) = opposite / hypotenuse
// Sin (60 degs) = ((L*sqr(3))/6) / C
// C = ((L*sqr(3))/6) / Sin (60)
// example: L = 30
// C = 8.6602540378443864676372317075294‬ / 0.86602540378443864676372317075294
// C = 10

//Randomized Kruskal's algorithm - Maze generation for resource layout
//Randomized Prim's algorithm
//https://en.wikipedia.org/wiki/Maze_generation_algorithm

//Look up Brackeys on Youtube for later
//https://www.youtube.com/watch?v=ryISV_nH8qw

//https://answers.unity.com/questions/170960/objects-in-the-same-place-with-different-coordinat.html

//https://answers.unity.com/questions/1471680/tilemap-not-filling-in-the-whole-space.html

//https://makegamessa.com/discussion/5347/unity3d-hexagonal-tilemap-hex-shape

public class BoardManager : MonoBehaviour
{
	[HideInInspector]
	//public Hashtable<HexTileKey, HexTile> boardGrid;
	public Dictionary<HexTileKey, HexTile> boardGrid;
	public Dictionary<HexTileKey, HexTile> KruskalGrid;
	public List<Dictionary<HexTileKey, HexTile>> InMaze;
	//private Transform boardHolder;
	private Tilemap baseTileHolder;
	private Tilemap resourceTileHolder;
	public BoardManager GGG_Board;
	
	public Camera mainCamera;
	
	//private int columns;
	//private int rows;
	// Columns and rows were the same, changing it to Length
	private int Length;
	private int numOfEnemies;
	
	//General Tiles
	public Tile[] floorTiles;
	public Tile[] outerWallTiles;
	//Resource Tiles
	public Tile[] forestTiles;
	public Tile[] sunstoneTiles;
	public Tile[] holyWaterPoolTiles;
	public Tile[] mithrilOreTiles;
	//City Tiles
	public Tile humanCityTile;
	public Tile ghoulCityTile;
	public Tile ghostCityTile;
	public Tile goblinCityTile;
	
	//Ghoul Enemy Tiles
	private GameObject[] ghoulStandardTiles;
	private GameObject[] ghoulTrapperTiles;
	private GameObject[] ghoulHunterTiles;
	//Ghost Enemy Tiles
	private GameObject[] ghostStandardTiles;
	private GameObject[] ghostTrapperTiles;
	private GameObject[] ghostHunterTiles;
	//Goblin Enemy Tiles
	private GameObject[] goblinStandardTiles;
	private GameObject[] goblinTrapperTiles;
	private GameObject[] goblinHunterTiles;
	
	private bool humanTileSet;
	//private bool ghoulTileSet;
	//private bool ghostTileSet;
	//private bool goblinTileSet;
	private List <Tile> enemyCities;
	private List <Tile[]> resourceList;

	/*public struct TileData(bool GhostBarrier, int resourceAmount, int BarrierHP) : this()
	{
		GhostBarrier = false;
		resourceAmount = 0;
		BarrierHP = 0;
	}*/
	
	public class HexTileKey
	{
		public int row { get; set; }
		public int column { get; set; }
		
		
		public HexTileKey()
		{
			row = -1;
			column = -1;
		}
			
		public override bool Equals(object obj)
		{			
			var tileKey = obj as HexTileKey;
			if (object.ReferenceEquals(tileKey, null))
				return false;			
			
			return column == tileKey.column && row == tileKey.row;
		}
		
		// override object.GetHashCode
		public override int GetHashCode()
		{
			// TODO: write your implementation of GetHashCode() here
			//throw new System.NotImplementedException();
			return base.GetHashCode();
		}
	}
	
	public class HexTile
	{
		private HexTileKey theKey;
		private HexTileKey edgeNorthWest;
		private HexTileKey edgeNorthEast;
		private HexTileKey edgeWest;
		private HexTileKey edgeEast;
		private HexTileKey edgeSouthWest;
		private HexTileKey edgeSouthEast;
		private Vector3 vectorValue;
		private bool visited;
		
		public HexTile()
		{
			theKey = new HexTileKey();
			edgeNorthWest = new HexTileKey();
			edgeNorthEast = new HexTileKey();
			edgeWest = new HexTileKey();
			edgeEast = new HexTileKey();
			edgeSouthWest = new HexTileKey();
			edgeSouthEast = new HexTileKey();
			visited = false;
		}
		
		public bool getVisited()
		{
			return visited;
		}
		
		private void setVisited()
		{
			visited = true;
		}
		
		private void clearVisited()
		{
			visited = false;
		}
		
		public HexTileKey getHexKey()
		{
			return theKey;
		}
		
		public HexTileKey getEdgeNorthWest()
		{
			return edgeNorthWest;
		}
		
		public HexTileKey getEdgeNorthEast()
		{
			return edgeNorthEast;
		}
		
		public HexTileKey getEdgeWest()
		{
			return edgeWest;
		}
		
		public HexTileKey getEdgeEast()
		{
			return edgeEast;
		}
		
		public HexTileKey getEdgeSouthWest()
		{
			return edgeSouthWest;
		}
		
		public HexTileKey getEdgeSouthEast()
		{
			return edgeSouthEast;
		}
		
		public Vector3 getvectorValue()
		{
			return vectorValue;
		}
		
		public void setHexKey(HexTileKey key)
		{
			theKey = key;
		}
		
		public void setEdgeNorthWest(int edgeNorthWestRow, int edgeNorthWestColumn)
		{		
			edgeNorthWest.row = edgeNorthWestRow;
			edgeNorthWest.column = edgeNorthWestColumn;
		}
		
		public void setEdgeNorthEast(int edgeNorthEastRow, int edgeNorthEastColumn)
		{
			edgeNorthEast.row = edgeNorthEastRow;
			edgeNorthEast.column = edgeNorthEastColumn;
		}
		
		public void setEdgeWest(int edgeWestRow, int edgeWestColumn)
		{
			edgeWest.row = edgeWestRow;
			edgeWest.column = edgeWestColumn;
		}
		
		public void setEdgeEast(int edgeEastRow, int edgeEastColumn)
		{
			edgeEast.row = edgeEastRow;
			edgeEast.column = edgeEastColumn;
		}
		
		public void setEdgeSouthWest(int edgeSouthWestRow, int edgeSouthWestColumn)
		{
			edgeSouthWest.row = edgeSouthWestRow;
			edgeSouthWest.column = edgeSouthWestColumn;
		}
		
		public void setEdgeSouthEast(int edgeSouthEastRow, int edgeSouthEastColumn)
		{
			edgeSouthEast.row = edgeSouthEastRow;
			edgeSouthEast.column = edgeSouthEastColumn;
		}
		
		public void setVectorValue(float vectorValueX, float vectorValueY)
		{
			vectorValue = new Vector3(vectorValueX,vectorValueY,0f);
		}
	}
	
	public BoardManager()
	{
		boardGrid = new Dictionary<HexTileKey, HexTile>();
		KruskalGrid = new Dictionary<HexTileKey, HexTile>();		
		InMaze = new List<Dictionary<HexTileKey, HexTile>>();
		//columns = 13;
		//rows = 13;
		Length = 13;
		humanTileSet = false;
		//ghoulTileSet = true;
		//ghostTileSet = true;
		//goblinTileSet = true;
	}
	
	~BoardManager()
	{
		boardGrid.Clear();	
		KruskalGrid.Clear();		
	}
	
	//Camera initialization could not be placed in the constructor. Must be in "Awake" or "Start"
	public void Awake()
	{
		//mainCamera = GGG_Board.GetComponent<Camera>();
		mainCamera = GGG_Board.GetComponentInChildren<Camera>();
		if(mainCamera == null)
			Debug.Log("mainCamera is null");	
	}
	
	public void OnValidate()
	{
		//baseTileHolder = GGG_Board.GetComponent<>("Tilemap_BaseBoard");
		
		GGG_Board = this.GetComponent<BoardManager>();
		if(GGG_Board == null)
			Debug.Log("GGG_Board is null");
		else
			Debug.Log("GGG_Board is not null");
		baseTileHolder = (GGG_Board.GetComponentsInChildren<Tilemap>())[1];
		resourceTileHolder = (GGG_Board.GetComponentsInChildren<Tilemap>())[0];		
	}
	
	private void setLength(int value)
	{
		Length = value;
	}
	
	private int getLength()
	{
		return Length;
	}
	
	//private void setTileParent(GameObject InstantiateMe, int Row, int Col, int Dist, Transform GGG_Tilemap)
	private Vector3Int setTileParent(Tile InstantiateMe, int Row, int Col, int Dist, Tilemap GGG_Tilemap)
	{
		Vector3Int newTile;
		/*if(GGG_Tilemap == null)
			Debug.Log("GGG_Tilemap is null");
		else if(InstantiateMe == null)
			Debug.Log("InstantiateMe is null");
		else
		{*/
			//15
			int halfway;
			Col = Length - Col - 1;
			if(Col%2 != 0)
				halfway = (Col+1)/2;
			else
				halfway = Col/2;
			Row -= halfway;
			newTile = new Vector3Int(Row, Col, Dist);
			Debug.Log("Tile Row and Column: " + Row + " " + Col);
			GGG_Tilemap.SetTile(newTile, InstantiateMe);	
			return newTile;
		//}
		//GameObject instance = GameObject.Instantiate(InstantiateMe, new Vector3(Row,Col,Dist), Quaternion.identity);
		//return newTile;
		//instance.transform.SetParent(GGG_Tilemap);		
	}
	
	private void BoardSetup(int cityLevel)
	{
		//Debug.Log("Test2");
		//boardHolder = new GameObject("GGG_Board").transform;
		//baseTileHolder = new GameObject("Tilemap_BaseBoard").transform;
		//resourceTileHolder = new GameObject("Tilemap_Resources").transform;
		//Tilemap[] tms = GetComponentsInChildren<Tilemap>();
		//baseTileHolder = this.transform.GetChild(0).GetComponent<Tilemap>();
		//resourceTileHolder = this.transform.GetChild(1).GetComponent<Tilemap>();
		//baseTileHolder = (GGG_Board.GetComponentsInChildren<Tilemap>())[0];
		//baseTileHolder = GetComponentsInChildren<Tilemap>("Tilemap_BaseBoard");
		//baseTileHolder = GGG_Board.GetComponentsInChildren("Tilemap_BaseBoard") as Tilemap;
		//resourceTileHolder = (GGG_Board.GetComponentsInChildren<Tilemap>())[1];
		//resourceTileHolder = GetComponentsInChildren<Tilemap>("Tilemap_Resources");
		//resourceTileHolder = GGG_Board.GetComponentsInChildren("Tilemap_Resources") as Tilemap;
		if(baseTileHolder == null)
			Debug.Log("baseTileHolder is null");
		if(resourceTileHolder == null)
			Debug.Log("resourceTileHolder is null");
		float centroid = CalculateCentroid();
		int distanceToHCityX = FindHumanCityX(centroid);
		int distanceToHCityY = FindHumanCityY(centroid);
		Debug.Log("centroid: " + centroid);
		Debug.Log("distanceToHCityX: " + distanceToHCityX);
		Debug.Log("distanceToHCityY: " + distanceToHCityY);
		int enemiesLeft = numOfEnemies;
		int numOfResources = 0;
		int numEnemyTileLeft = 3;
						
		//May need to change to foreach loop(s)
		foreach(HexTile hexValue in boardGrid.Values)
		{		
			Tile toInstantiate = null;
			//HexTile ht = (HexTile)hexValue;
			int x = hexValue.getHexKey().row;
			int y = hexValue.getHexKey().column;
			
			//Outerwalls - TESTING DONE, place tiles!
			if(x == Length-1 || y == Length-1 || (x == Length-y-1))
			{				
				//Debug.Log("Test1 Each Hex Row and Column: " + x + " " + y);
				toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];
				setTileParent(toInstantiate, x, y, 0, baseTileHolder);
			}
			//Corner check for each city - Enemy Bases
			//First enemy City: Top Corner: Position(Length-2, 2)
			//Second enemy City: Bottom-Left: Position(2, Length-2)
			//Third enemy City: Bottom-Right: Position(Length-2, Length-2)
			//TESTING DONE, place tiles!
			else if ((x == 2 && y == Length-x) || (x == Length-2 && y == 2) || (x == Length-2 && y == Length-2))
			{
				switch(numOfEnemies) // Prep for monster cities
				{
					case 1:
					{
						if((enemiesLeft > 0 && Random.Range (0, 100) > 49) || enemyCities.Count == numEnemyTileLeft)
						//if(enemiesLeft > 0 && Random.Range (0, 100) > 49)
						{
							Debug.Log("Enemy City Placed Test2 Each Hex Row and Column: " + x + " " + y);
							toInstantiate = enemyCities[Random.Range (0, enemyCities.Count)];
							setTileParent(toInstantiate, x, y, 0, baseTileHolder);
							enemyCities.Remove(toInstantiate);
							enemiesLeft -= 1;
							Dictionary<HexTileKey, HexTile> addTileToMaze = new Dictionary<HexTileKey, HexTile>();
							addTileToMaze.Add(hexValue.getHexKey(), hexValue);
							InMaze.Add(addTileToMaze);
							//KruskalGrid.Add(hexValue.getHexKey(), hexValue);
						}
						else //If no enemy cities are left, use floor and resource
						{
							Debug.Log("Enemy City Not Placed Test2 Each Hex Row and Column: " + x + " " + y);
							toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
							setTileParent(toInstantiate, x, y, 0, baseTileHolder);
							toInstantiate = forestTiles[Random.Range (0, forestTiles.Length)];	
							setTileParent(toInstantiate, x, y, -1, resourceTileHolder);	
							KruskalGrid.Add(hexValue.getHexKey(), hexValue);	
						}
						numEnemyTileLeft -= 1;
						break;
					}
					case 2:
					{		
						if((enemiesLeft > 0 && Random.Range (0, 100) > 49) || enemyCities.Count == numEnemyTileLeft)
						{	
							Debug.Log("Enemy City Placed Test2 Each Hex Row and Column: " + x + " " + y);
							toInstantiate = enemyCities[Random.Range (0, enemyCities.Count)];
							setTileParent(toInstantiate, x, y, 0, baseTileHolder);
							enemyCities.Remove(toInstantiate);	
							enemiesLeft -= 1;
							Dictionary<HexTileKey, HexTile> addTileToMaze = new Dictionary<HexTileKey, HexTile>();
							addTileToMaze.Add(hexValue.getHexKey(), hexValue);
							InMaze.Add(addTileToMaze);
							//KruskalGrid.Add(hexValue.getHexKey(), hexValue);
						}	
						else //If no enemy cities are left, use floor and resource
						{
							Debug.Log("Enemy City Not Placed Test2 Each Hex Row and Column: " + x + " " + y);
							toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
							setTileParent(toInstantiate, x, y, 0, baseTileHolder);
							toInstantiate = forestTiles[Random.Range (0, forestTiles.Length)];	
							setTileParent(toInstantiate, x, y, -1, resourceTileHolder);
							
							KruskalGrid.Add(hexValue.getHexKey(), hexValue);
						}	
						numEnemyTileLeft -= 1;		
						break;				
					}
					case 3:
					{
						Debug.Log("Enemy City Placed Test2 Each Hex Row and Column: " + x + " " + y);
						toInstantiate = enemyCities[Random.Range (0, enemyCities.Count)];
						setTileParent(toInstantiate, x, y, 0, baseTileHolder);
						enemyCities.Remove(toInstantiate);
						Dictionary<HexTileKey, HexTile> addTileToMaze = new Dictionary<HexTileKey, HexTile>();
						addTileToMaze.Add(hexValue.getHexKey(), hexValue);
						InMaze.Add(addTileToMaze);
						//KruskalGrid.Add(hexValue.getHexKey(), hexValue);
						break;
					}
				}
			}
			//HumanCity Center location
			//HumanCity: int value = FindHumanCity(); Position(value, value)
			//TESTING DONE, place tiles!
			else if (x == distanceToHCityX && y == distanceToHCityY)
			{
				Debug.Log("Test3 Each Hex Row and Column: " + x + " " + y);
				if(!humanTileSet)
				{
					Vector3Int temp;
					toInstantiate = humanCityTile;
					temp = setTileParent(toInstantiate, x, y, 0, baseTileHolder);
					Dictionary<HexTileKey, HexTile> addTileToMaze = new Dictionary<HexTileKey, HexTile>();
					addTileToMaze.Add(hexValue.getHexKey(), hexValue);
					InMaze.Add(addTileToMaze);
					//KruskalGrid.Add(hexValue.getHexKey(), hexValue);
					//Vector3 tempVector = temp; 
					Debug.Log("Hex Row and Column: " + x + " " + y);
					Debug.Log("Tile Row and Column: " + temp.x + " " + temp.y);
					//TileData tileTransformData;
					//ITilemap tilemap; 
					//toInstantiate.GetTileData(temp, tilemap, ref tileTransformData);
					//Transform tileParent = transform.parent;
					//GridLayout gridLayout = tileParent.GetComponentInParent<GridLayout>();
					//Centering the camera on the human city upon map start
					//transform.LookAt(gridLayout.CellToWorld(temp));
					
					//GGG_Board.camera.gameObject.transform.position = new Vector3(temp.x, temp.y, -20);
					mainCamera.transform.position = new Vector3(temp.x, temp.y, -1000);
					//mainCamera.gameObject.transform.SetPositionAndRotation(tempVector, mainCamera.gameObject.transform.rotation);
					humanTileSet = true;
				}			
			}
			//Resources Check
			else 
			{
				//Debug.Log("Test4 Each Hex Row and Column: " + x + " " + y);
				toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
				setTileParent(toInstantiate, x, y, 0, baseTileHolder);
				Tile [] temp = null;
				
				KruskalGrid.Add(hexValue.getHexKey(), hexValue);
				switch(numOfResources) // Prep for resources
				{
					case 0:
					{
						temp = resourceList[numOfResources];
						toInstantiate = temp[Random.Range (0, temp.Length)];	
						setTileParent(toInstantiate, x, y, -1, resourceTileHolder);
						numOfResources++;
						break;
					}
					case 1:
					{
						temp = resourceList[numOfResources];
						toInstantiate = temp[Random.Range (0, temp.Length)];	
						setTileParent(toInstantiate, x, y, -1, resourceTileHolder);
						if(numOfResources == numOfEnemies)
							numOfResources = 0;
						else
							numOfResources++;
						break;
					}
					case 2:
					{
						temp = resourceList[numOfResources];
						toInstantiate = temp[Random.Range (0, temp.Length)];	
						setTileParent(toInstantiate, x, y, -1, resourceTileHolder);
						if(numOfResources == numOfEnemies)
							numOfResources = 0;
						else
							numOfResources++;
						break;
					}
					case 3:
					{
						temp = resourceList[numOfResources];
						toInstantiate = temp[Random.Range (0, temp.Length)];	
						setTileParent(toInstantiate, x, y, -1, resourceTileHolder);
						numOfResources = 0;
						break;
					}
				}
			}//End Else
		}//End foreach
	}
	
	private void InitializeList()
	{
		//Debug.Log("Test1");
		boardGrid.Clear();	
		KruskalGrid.Clear();
		InMaze.Clear();
		
		int y;
		int x = 0;
		
		Debug.Log("Length #: " + Length);
		while(x < Length)
		{
			y = x + 1;
			while(y > 0)
			{			
				//Debug.Log("Row #: " + x + " Col #: " + y);
				HexTile hex = new HexTile();
				HexTileKey hexKey = new HexTileKey();
				hexKey.row = x;	
				hexKey.column = Length-y;
				hex.setVectorValue(Length-y, x);
				hex.setHexKey(hexKey);
				
				//Finding the neighbors
				hex.setEdgeNorthWest(x, Length-y - 1);
				hex.setEdgeNorthEast(x + 1, Length-y - 1);
				hex.setEdgeWest(x - 1, Length-y);
				hex.setEdgeEast(x + 1, Length-y);
				hex.setEdgeSouthWest(x - 1, Length-y + 1);
				hex.setEdgeSouthEast(x, Length-y + 1);
				
				//boardGrid[hex.getHexKey()] = hex;
				boardGrid.Add(hex.getHexKey(), hex);
				y--;
			}
			x++;
		}		
	}
	
	private void DetermineBoardSize(int monsters, int level)
	{
		//setColumn(columns + monsters*5 + level*2);
		//setRows(rows + monsters*5 + level*2);
		setLength(Length + monsters*5 + level*2);
		//Debug.Log("Monsters is at: " + monsters);
		//Debug.Log("Level is at: " + level);
	}
	
	public void SetupScene(int monsters, int level)
	{
		switch(monsters) // Prep for monster cities
		{
			case 0: // Ghouls - Level 0, 1
			{
				//ghoulTileSet = false;
				numOfEnemies = 1; 
				enemyCities = new List<Tile>();
				enemyCities.Add(ghoulCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(sunstoneTiles);
				//Debug.Log("This should be tested.");
				break;
			}
			case 1: // Ghosts - Level 1, 1
			{
				//ghostTileSet = false;
				numOfEnemies = 1;
				enemyCities = new List<Tile>();
				enemyCities.Add(ghostCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(holyWaterPoolTiles);
				//Debug.Log("This should not be tested.");
				break;
			}
			case 2: // Goblins - Level 2, 1
			{
				//goblinTileSet = false;
				numOfEnemies = 1; 
				enemyCities = new List<Tile>();
				enemyCities.Add(goblinCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(mithrilOreTiles);
				//Debug.Log("This should not be tested.");
				break;
			}
			case 3: // Ghouls & Ghosts - Level 3, 1
			{
				//ghoulTileSet = false;
				//ghostTileSet = false;
				numOfEnemies = 2;
				enemyCities = new List<Tile>();
				enemyCities.Add(ghoulCityTile);
				enemyCities.Add(ghostCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(sunstoneTiles);
				resourceList.Add(holyWaterPoolTiles);
				//Debug.Log("This should not be tested.");
				break;
			}
			case 4: // Ghosts & Goblins - Level 4, 1
			{
				//ghostTileSet = false;
				//goblinTileSet = false;
				numOfEnemies = 2;
				enemyCities = new List<Tile>();
				enemyCities.Add(ghostCityTile);
				enemyCities.Add(goblinCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(holyWaterPoolTiles);
				resourceList.Add(mithrilOreTiles);
				//Debug.Log("This should not be tested.");
				break;
			}
			case 5: // Ghouls & Goblins - Level 5, 1
			{
				//ghoulTileSet = false;
				//goblinTileSet = false;
				numOfEnemies = 2;
				enemyCities = new List<Tile>();
				enemyCities.Add(ghoulCityTile);
				enemyCities.Add(goblinCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(sunstoneTiles);
				resourceList.Add(mithrilOreTiles);
				//Debug.Log("This should not be tested.");
				break;
			}
			case 6: // Ghouls, Ghosts, & Goblins - Level 6, 1
			{
				//ghoulTileSet = false;
				//ghostTileSet = false;
				//goblinTileSet = false;
				numOfEnemies = 3;
				enemyCities = new List<Tile>();
				enemyCities.Add(ghoulCityTile);
				enemyCities.Add(ghostCityTile);
				enemyCities.Add(goblinCityTile);
				resourceList = new List<Tile[]>();
				resourceList.Add(forestTiles);
				resourceList.Add(sunstoneTiles);
				resourceList.Add(holyWaterPoolTiles);
				resourceList.Add(mithrilOreTiles);
				//Debug.Log("This should not be tested.");
				break;
			}
		}
		
		DetermineBoardSize(monsters, level);
		InitializeList();
		BoardSetup(monsters);	
		MakeMaze();
	}
	
	private void MakeMaze()
	{		
		/*
			1. Full list of walls already set. Aka the map full of resources - DONE
			2. Put human city and enemyCities into unique sets. - DONE
			3. Pop "wall" out of List - DONE
			4. Check neighbors - DONE
			5. Count nearby sets
				a. If set is 2 or more neighbors, remove wall, do not add to sets, return to step 3 - DONE
				b. If neighbors have only 1 neighbor that is in a set, merge with set - DONE
				c. If neighbors have multiple sets, merge multiple sets into one, merge wall into merged set - DONE
				d. If neighbors do not have any sets, create new set - DONE
			6. If wall list > 0, return to step 3, otherwise done. - DONE
			
		*/
		Random random  = new Random();
		HexTile poppedWall;
		HexTileKey poppedNeighbor;
		Dictionary<HexTileKey, HexTile> newSetOfTiles;
		Dictionary<HexTileKey, HexTile> tempDictionary = null;
		int count = KruskalGrid.Count;
		bool remainWall;
		
		do
		{
			remainWall = false;
			//poppedWall = KruskalGrid.ElementAt(Random.Range(0, count));
			poppedWall = KruskalGrid.Values.ToList()[Random.Range(0, count)];
			//poppedWall = KruskalGrid.HexTileKey.ToList()[Random.Range(0, count)];
			
			int switchPopped = 0;
				
			int [] setCount = new int[InMaze.Count];
			do
			{
				poppedNeighbor = null;
				switch(switchPopped)
				{
					case 0:
						poppedNeighbor = poppedWall.getEdgeNorthWest();
						switchPopped++;
						break;
					case 1:
						poppedNeighbor = poppedWall.getEdgeNorthEast();
						switchPopped++;
						break;
					case 2:
						poppedNeighbor = poppedWall.getEdgeWest();
						switchPopped++;
						break;
					case 3:
						poppedNeighbor = poppedWall.getEdgeEast();
						switchPopped++;
						break;
					case 4:
						poppedNeighbor = poppedWall.getEdgeSouthWest();
						switchPopped++;
						break;
					case 5:
						poppedNeighbor = poppedWall.getEdgeSouthEast();
						break;
				}
				
				int i = 0;
				int col = poppedNeighbor.column;
				int row = poppedNeighbor.row;
				foreach(Dictionary<HexTileKey, HexTile> tempDict in InMaze)
				{
					if(tempDict.Keys.ElementAt(0).Equals(poppedNeighbor))
					//if(tempDict.Keys.Equals(poppedNeighbor))
					{
						if(setCount[i] == 1)
							remainWall = true;
						else
							setCount[i] += 1;					
					}
					
					if(remainWall)
						break;
					else
						i++;
				}
			}while(!remainWall && switchPopped != 5);
			
			if(!remainWall)
			{
				newSetOfTiles = new Dictionary<HexTileKey, HexTile>();
				for(int i = 0; i < InMaze.Count; i++)
				{
					if(setCount[i] == 1)
					{
						tempDictionary = InMaze[i];
						tempDictionary.ToList().ForEach(x => newSetOfTiles.Add(x.Key, x.Value));
						InMaze.RemoveAt(i);
						i--;
					}

					if(tempDictionary != null)
						tempDictionary.Clear();
				}
					
				newSetOfTiles.Add(poppedWall.getHexKey(), poppedWall);
				InMaze.Add(newSetOfTiles);	
				int rowValue = poppedWall.getHexKey().row;
				int columnValue = poppedWall.getHexKey().column;
				Vector3Int tempPosition = new Vector3Int(rowValue, columnValue, 0);
				
				Debug.Log("Tile Removed Row and Column: " + rowValue + " " + columnValue);
				resourceTileHolder.SetTile(tempPosition, null);
				resourceTileHolder.RefreshTile(tempPosition);
				
				//TileData tempData = new TileData();
				//ITilemap resourceTileHolderI = GGG_Board.GetComponentsInChildren<ITilemap>().ElementAt(0);
				//ITilemap resourceTileHolderI = null;
				//resourceTileHolderI = GGG_Board.GetComponentsInChildren<ITilemap>().ElementAt(0);
				//resourceTileHolder.GetTile(tempPosition).GetTileData(tempPosition, resourceTileHolderI, ref tempData);
				//tempData.gameObject.SetActive(false);
				//Tile example = new Tile();
				//example.gameObject.SetActive(false);
			
				//newSetOfTiles.Clear();
			}
			
			KruskalGrid.Remove(poppedWall.getHexKey());
			count = KruskalGrid.Count;
		}while(count > 0);				
	}
	
	private float CalculateCentroid()
	{
		//Debug.Log("Finding Human City");
		//return(Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(Length/2.0f,2) + Mathf.Pow((Length*Mathf.Sqrt(3))/6.0f,2))));
		//return((Length*Mathf.Sqrt(3))/6.0f);
		//Found this next formula from quora centroid site.
		return(Length - (Length*2)/3.0f);
	}
	
	private int FindHumanCityX(float value)
	{
		//Debug.Log("Finding Human City");
		//return(Length - Mathf.FloorToInt(value));
		//return(Mathf.FloorToInt(value));
		return(Length - Mathf.CeilToInt(value));
		//return(Mathf.CeilToInt(value));
		
		//return(Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(Length/2.0f,2) + Mathf.Pow((Length*Mathf.Sqrt(3))/6.0f,2))));
		//return(Mathf.CeilToInt(Mathf.Sqrt(Mathf.Pow(Length/2.0f,2) + Mathf.Pow((Length*Mathf.Sqrt(3))/6.0f,2))));
	}
	
	private int FindHumanCityY(float value)
	{
		//Debug.Log("Finding Human City");
		Debug.Log("Sine value: " + Mathf.Sin(Mathf.Deg2Rad * 60));
		return(Length - Mathf.CeilToInt(value / Mathf.Sin(Mathf.Deg2Rad * 60)));
		//return(Mathf.CeilToInt(value / Mathf.Sin(Mathf.Deg2Rad * 60)));
		
		//return(Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(Length/2.0f,2) + Mathf.Pow((Length*Mathf.Sqrt(3))/6.0f,2))));
		//return(Mathf.CeilToInt(Mathf.Sqrt(Mathf.Pow(Length/2.0f,2) + Mathf.Pow((Length*Mathf.Sqrt(3))/6.0f,2))));
	}
}
