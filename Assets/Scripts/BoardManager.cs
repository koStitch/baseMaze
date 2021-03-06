using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Managers	
{
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		private Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
		public GameObject playersBase;											//Prefab to spawn for exit.
        public GameObject pathfindingDebugNode;
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] enemyTiles;									//Array of enemy prefabs.
		public GameObject[] outerWallTiles;								//Array of outer tile prefabs.
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
        private Pathfinding.Pathfinding pathfinding;                                //Reference to our pathfinding class witch will handle pathfinding algorithms
        private Transform[,] visualNodeArray;                           //Array containing all grid nodes
        private List<Transform[,]> visualNodesList = new List<Transform[,]>();
        private List<GameObject> nodeHolders = new List<GameObject>();

        public Pathfinding.Pathfinding Pathfinding => pathfinding;                  //Pathfinding getter
        public List<Transform[,]> VisualNodesList => visualNodesList;   //Node list getter

        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList (int columns, int rows)
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 1; x < columns-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < rows-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}

        //Sets up the outer walls and floor (background) of the game board.
        void BoardSetup(int columns, int rows)
        {
            //Create pathfinding object using number of columns and rows our level(grid) will have
            pathfinding = new Pathfinding.Pathfinding(columns, rows);

            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject("Board").transform;

            //Stash the grid in local variable
            var grid = pathfinding.GetGrid();

            //Instantiate node array with grid size(grid width and height)
            visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

            //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
            for (int x = -1; x < grid.GetWidth() + 1; x++)
            {
                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                for (int y = -1; y < grid.GetHeight() + 1; y++)
                {
                    //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                    //Check if we current position is at board edge.
                    bool boardEdge = x == -1 || x == grid.GetWidth() || y == -1 || y == grid.GetHeight();
                    if (boardEdge)
                    {
                        //If so choose a random outer wall prefab from our array of outer wall tiles.
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    }

                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop
                    Transform visualNode = Instantiate(toInstantiate.transform, new Vector3(x, y, 0f), Quaternion.identity);

                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    visualNode.transform.SetParent(boardHolder);

                    //Add node to array only if the nodes position is inside of the playfield
                    if (!boardEdge)
                    {
                        visualNodeArray[x, y] = visualNode;
                    }
                }
            }
        }

        //RandomPosition returns a random position from our list gridPositions.
        Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, bool isWall = false)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				var instantiatedObject = Instantiate(tileChoice, randomPosition, Quaternion.identity);

                 //If we are creating a wall set it as unwalkable node in pathfinding
                if (isWall)
                {
                    pathfinding.GetGrid().GetXY(instantiatedObject.transform.position, out int x, out int y);
                    pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
                }
            }
		}	
		
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level, int columns, int rows, int minObstacles, int maxObstacles)
		{
			//Creates the outer walls and floor.
			BoardSetup (columns, rows);
			
			//Reset our list of gridpositions.
			InitialiseList (columns, rows);
			
			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (wallTiles, minObstacles, maxObstacles, true);
			
			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (enemyTiles, level, level);
			
			//Instantiate the players base tile in the down right hand corner of our game board
			Instantiate (playersBase, new Vector3 (0, 0, 0f), Quaternion.identity);

            //Fill the list with pathfinding debug nodes for each enemy, level = enemies count
            FillPathfindingDebugNodes(level);
        }

        private void FillPathfindingDebugNodes(int enemyCount)
        {
            //Get width and height from visualNodeArray that represents all nodes positioned inside of the playfield
            int width = visualNodeArray.GetLength(0);
            int height = visualNodeArray.GetLength(1);

            for (int i = 0; i < enemyCount; i++)
            {
                //Instantiate NodeHolder that will hold all of the debug nodes
                var nodeHolder = new GameObject("NodeHolder" + i.ToString()).transform;
                //Pool all the nodeHolders together for easier deletion
                nodeHolders.Add(nodeHolder.gameObject);
                //Declare visual node multidimensional array with width and height that we declared earlier
                Transform[,] visualNodes = new Transform[width, height];
                //Go trough entire visualNodeArray
                for (int j = 0; j < width; j++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        //Instantiate the Transform instance using the pathfindingDebugNode prefab at the Vector3 corresponding to visualNodeArray position
                        Transform node = Instantiate(pathfindingDebugNode.transform, new Vector3(visualNodeArray[j, z].position.x, visualNodeArray[j, z].position.y, 0f), Quaternion.identity);
                        //Set the parent of our newly instantiated object instance to nodeHolder, this is just organizational to avoid cluttering hierarchy
                        node.transform.SetParent(nodeHolder);
                        //Set all of the nodes inactive because we do not need to see them now, we will turn them on later
                        node.gameObject.SetActive(false);
                        //Add instantiated node to the multidimensional array we created earlier
                        visualNodes[j, z] = node;
                    }
                }

                //Add entire array to the list
                visualNodesList.Add(visualNodes);
            }
        }

        public void DeleteNodeHolders()
        {
            for (int i = 0; i < nodeHolders.Count; i++)
            {
                Destroy(nodeHolders[i]);
            }
        }
	}
}
