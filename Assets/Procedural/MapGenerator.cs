using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private enum AssetType
    {
        wall_2x1 = 0,
        wallDoor_2x1 = 1,
        door_1x1 = 2,
        furnitureWall_1x1 = 3,
        furnitureCorner_1x1 = 4,
        furnitureCenter_1x1 = 5,
        furnitureCenter_2x1 = 6,
        floor_2x2 = 7
    }

    // Room generation parameters
    [Header("Generation")]
    public bool __________GENERATION__________ = false;
    public int roomQuantity = 1;
    public float scale = 1.0f;
    public int levelSizeX = 10;
    public int levelSizeY = 10;
    public int maxIteration = 50;
    public int seed = 42;
    public int tableThreshold = 50;
    public int doorThreshold = 50;
    public int sideThreshold = 50;
    public int coin = 5;
    public GameObject coinPrefab;

    [Header("Kitchen")]
    public bool __________KITCHEN__________ = false;
    public GameObject[] wallKitchen_2x1;
    public GameObject[] wallDoorKitchen_2x1;
    public GameObject[] doorKitchen_1x1;
    public GameObject[] furnitureWallKitchen_1x1;
    public GameObject[] furnitureCornerKitchen_1x1;
    public GameObject[] furnitureCenterKitchen_1x1;
    public GameObject[] furnitureCenterKitchen_2x1;
    public GameObject[] floorKitchen_2x2;

    [Header("Batthroom")]
    public bool __________BATHROOM__________ = false;
    public GameObject[] wallBatthroom_2x1;
    public GameObject[] wallDoorBatthroom_2x1;
    public GameObject[] doorBatthroom_1x1;
    public GameObject[] furnitureWallBatthroom_1x1;
    public GameObject[] furnitureCornerBatthroom_1x1;
    public GameObject[] furnitureCenterBatthroom_1x1;
    public GameObject[] furnitureCenterBatthroom_2x1;
    public GameObject[] floorBatthroom_2x2;

    [Header("Living")]
    public bool __________LIVING__________ = false;
    public GameObject[] wallLiving_2x1;
    public GameObject[] wallDoorLiving_2x1;
    public GameObject[] doorLiving_1x1;
    public GameObject[] furnitureWallLiving_1x1;
    public GameObject[] furnitureCornerLiving_1x1;
    public GameObject[] furnitureCenterLiving_1x1;
    public GameObject[] furnitureCenterLiving_2x1;
    public GameObject[] floorLiving_2x2;

    // Used to check if all the tiles were assigned to a room
    private int tilesCount = 0;

    // Room container
    private int[,] rooms;
    private List<int> roomTypes = new List<int>();
    private List<(int X, int Y, int direction)> doors;
    private List<List<List<int>>> kitchenFurnitureCount = new List<List<List<int>>>();
    private List<List<List<int>>> bathroomFurnitureCount = new List<List<List<int>>>();
    private List<List<List<int>>> livingFurnitureCount = new List<List<List<int>>>();
    private List<int> roomLink = new List<int>();
    private List<(float X, float Y)> availableTiles = new List<(float X, float Y)>();

    // Start is called before the first frame update
    void Start()
    {
        initializeRooms();
        extendRooms();
        placeDoors();
        placeWalls();
        placeAsset();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Room generation functions
    private void initializeRooms()
    {
        // Initialize the room at the given size
        rooms = new int[levelSizeX, levelSizeY];

        // Initialize all the tiles
        for (int coordX = 0; coordX < levelSizeX; coordX++)
        {
            for (int coordY = 0; coordY < levelSizeY; coordY++)
            {
                rooms[coordX, coordY] = -1;
            }
        }

        int roomCreated = 0;
        // Loop over all the rooms we have to create
        for (int i = 0; i < roomQuantity; i++)
        {
            bool roomFound = false;
            int iterationCount = 0;
            // Loop until we find an available tile
            while (roomFound == false && iterationCount < maxIteration)
            {
                // Get a random tile
                int selectedRoomX = Random.Range(0, levelSizeX - 1);
                int selectedRoomY = Random.Range(0, levelSizeY - 1);

                // If the tile is available
                if (rooms[selectedRoomX, selectedRoomY] < 0)
                {
                    // Set a room to it
                    rooms[selectedRoomX, selectedRoomY] = i;
                    // Increment the tile count
                    tilesCount++;
                    // A room has ben found
                    roomFound = true;
                    roomCreated++;
                    int type = Random.Range(0, 1000) % 3;
                    roomTypes.Add(type);

                    GameObject lightGameObject = new GameObject("PointLight");
                    Light pointLight = lightGameObject.AddComponent<Light>();
                    pointLight.type = LightType.Point;
                    lightGameObject.transform.Translate(new Vector3(selectedRoomX * scale, 3, selectedRoomY * scale));


                    if (type == 0)
                    {
                        kitchenFurnitureCount.Add(new List<List<int>>());
                        pointLight.color = new Color(0.9f, 0.7f, 0.7f, 1.0f);
                        pointLight.intensity = 0.6f;
                    }
                    else if (type == 1)
                    {
                        bathroomFurnitureCount.Add(new List<List<int>>());
                        pointLight.color = new Color(0.5f, 0.6f, 0.8f, 1.0f);
                        pointLight.intensity = 0.6f;
                    }
                    else if (type == 2)
                    {
                        livingFurnitureCount.Add(new List<List<int>>());
                        pointLight.color = new Color(0.7f, 0.7f, 0.5f, 1.0f);
                        pointLight.intensity = 0.4f;
                    }
                }
                iterationCount++;
            }
        }

        initializeFurnitureCount();
        roomQuantity = roomCreated;
    }

    private void initializeFurnitureCount()
    {
        int kitchenCount = 0;
        int BathroomCount = 0;
        int LivingCount = 0;
        for (int room = 0; room < roomQuantity; room++)
        {
            if (roomTypes[room] == 0)
            {
                roomLink.Add(kitchenCount);
                for (int type = 0; type < 8; type++)
                {
                    kitchenFurnitureCount[kitchenCount].Add(new List<int>());

                    switch (type)
                    {
                        case 0:
                            for (int asset = 0; asset < wallKitchen_2x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 1:
                            for (int asset = 0; asset < wallDoorKitchen_2x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 2:
                            for (int asset = 0; asset < doorKitchen_1x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 3:
                            for (int asset = 0; asset < furnitureWallKitchen_1x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 4:
                            for (int asset = 0; asset < furnitureCornerKitchen_1x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 5:
                            for (int asset = 0; asset < furnitureCenterKitchen_1x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 6:
                            for (int asset = 0; asset < furnitureCenterKitchen_2x1.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        case 7:
                            for (int asset = 0; asset < floorKitchen_2x2.Length; asset++) { kitchenFurnitureCount[kitchenCount][type].Add(0); }
                            break;
                        default:
                            break;
                    }
                }
                kitchenCount++;
            }
            else if (roomTypes[room] == 1)
            {
                roomLink.Add(BathroomCount);
                for (int type = 0; type < 8; type++)
                {
                    bathroomFurnitureCount[BathroomCount].Add(new List<int>());

                    switch (type)
                    {
                        case 0:
                            for (int asset = 0; asset < wallBatthroom_2x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 1:
                            for (int asset = 0; asset < wallDoorBatthroom_2x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 2:
                            for (int asset = 0; asset < doorBatthroom_1x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 3:
                            for (int asset = 0; asset < furnitureWallBatthroom_1x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 4:
                            for (int asset = 0; asset < furnitureCornerBatthroom_1x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 5:
                            for (int asset = 0; asset < furnitureCenterBatthroom_1x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 6:
                            for (int asset = 0; asset < furnitureCenterBatthroom_2x1.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        case 7:
                            for (int asset = 0; asset < floorBatthroom_2x2.Length; asset++) { bathroomFurnitureCount[BathroomCount][type].Add(0); }
                            break;
                        default:
                            break;
                    }
                }
                BathroomCount++;
            }
            else if (roomTypes[room] == 2)
            {
                roomLink.Add(LivingCount);
                for (int type = 0; type < 8; type++)
                {
                    livingFurnitureCount[LivingCount].Add(new List<int>());

                    switch (type)
                    {
                        case 0:
                            for (int asset = 0; asset < wallLiving_2x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 1:
                            for (int asset = 0; asset < wallDoorLiving_2x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 2:
                            for (int asset = 0; asset < doorLiving_1x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 3:
                            for (int asset = 0; asset < furnitureWallLiving_1x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 4:
                            for (int asset = 0; asset < furnitureCornerLiving_1x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 5:
                            for (int asset = 0; asset < furnitureCenterLiving_1x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 6:
                            for (int asset = 0; asset < furnitureCenterLiving_2x1.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        case 7:
                            for (int asset = 0; asset < floorLiving_2x2.Length; asset++) { livingFurnitureCount[LivingCount][type].Add(0); }
                            break;
                        default:
                            break;
                    }
                }
                LivingCount++;
            }
        }
    }

    private void extendRooms()
    {
        int iterationCount = 0;
        while (iterationCount < maxIteration && tilesCount < levelSizeX * levelSizeY)
        {
            // We have to create a roomsTemp variable to make sure each iterations are independant
            int[,] roomsTemp = rooms.Clone() as int[,];

            // Loop over each tiles to initialize the rooms starting points
            for (int coordX = 0; coordX < levelSizeX; coordX++)
            {
                for (int coordY = 0; coordY < levelSizeY; coordY++)
                {
                    // If the tile already has a room, skip it
                    if (rooms[coordX, coordY] != -1) { continue; }

                    // Store all the neighbours of the tile
                    List<int> relativePoses = new List<int>();
                    List<int> neighbourRoomIDs = getNeighbours(coordX, coordY, ref relativePoses);

                    // If some neighbours has been found
                    if (neighbourRoomIDs.Count > 0 && rooms[coordX, coordY] == -1)
                    {
                        // Get a random value from all the available values
                        int randomIndex = Random.Range(0, neighbourRoomIDs.Count - 1);

                        // Store the chosen neighbours
                        roomsTemp[coordX, coordY] = neighbourRoomIDs[randomIndex];
                        // Increment the tile count
                        tilesCount++;
                    }
                }
            }
            // Replace the old rooms with the new rooms
            rooms = roomsTemp.Clone() as int[,];
            iterationCount++;
        }
    }

    private void placeDoors()
    {
        doors = new List<(int X, int Y, int direction)>();

        // Loop over all the rooms
        for (int room = 0; room < roomQuantity; room++)
        {
            List<(int room, List<(int X, int Y, int direction)> tiles)> roomWalls = new List<(int room, List<(int X, int Y, int direction)> tiles)>();

            // Loop over each tiles
            for (int coordX = 0; coordX < levelSizeX; coordX++)
            {
                for (int coordY = 0; coordY < levelSizeY; coordY++)
                {
                    // If the tile is not the current room, we skip
                    if (rooms[coordX, coordY] != room) { continue; }

                    List<int> relativePoses = new List<int>();
                    List<int> neighbourRoomIDs = getNeighbours(coordX, coordY, ref relativePoses);

                    // Loop over each neighbours
                    for (int neighbour = 0; neighbour < neighbourRoomIDs.Count; neighbour++)
                    {
                        // If the neighbour is in the same room as us, skip it
                        if (neighbourRoomIDs[neighbour] == rooms[coordX, coordY]) { continue; }

                        bool isNewNeighbour = true;

                        // For each already appended
                        for (int roomWall = 0; roomWall < roomWalls.Count; roomWall++)
                        {
                            // If the room as already been appended
                            if (roomWalls[roomWall].room != neighbourRoomIDs[neighbour]) { continue; }

                            roomWalls[roomWall].tiles.Add((coordX, coordY, relativePoses[neighbour]));
                            isNewNeighbour = false;
                            break;
                        }

                        if (isNewNeighbour)
                        {
                            roomWalls.Add((neighbourRoomIDs[neighbour], new List<(int X, int Y, int direction)> { (coordX, coordY, relativePoses[neighbour]) }));
                        }
                    }
                }
            }

            shuffleList(ref roomWalls);

            roomWalls.ForEach(delegate ((int room, List<(int X, int Y, int direction)> tiles) roomWall)
            {
                int chosenTile = Random.Range(0, roomWall.tiles.Count - 1);

                if (Random.Range(0, 100) < doorThreshold)
                {
                    doors.Add((roomWall.tiles[chosenTile].X, roomWall.tiles[chosenTile].Y, roomWall.tiles[chosenTile].direction));
                }
            });
        }
    }

    private void placeWalls()
    {
        // Loop over each tiles to check if any walls to place
        for (int coordX = 0; coordX < levelSizeX; coordX++)
        {
            for (int coordY = 0; coordY < levelSizeY; coordY++)
            {
                List<int> relativePoses = new List<int>();
                List<int> neighbourRoomIDs = getNeighbours(coordX, coordY, ref relativePoses);

                List<int> edgeCheck = new List<int>(new int[] { 0, 2, 3, 1 });

                // Check if the neighbour is a door
                bool isDoor = false;
                doors.ForEach(delegate ((int X, int Y, int direction) door)
                {
                    if (coordX == door.X && coordY == door.Y) { isDoor = true; }
                });

                // Loop over each neighbours
                for (int i = 0; i < neighbourRoomIDs.Count; i++)
                {
                    edgeCheck.Remove(relativePoses[i]);

                    // If the neighbour has a difference ID
                    if (neighbourRoomIDs[i] != rooms[coordX, coordY])
                    {
                        int neighbourCoordX = coordX;
                        int neighbourCoordY = coordY;

                        if (relativePoses[i] == 0) { neighbourCoordX -= 1; }
                        if (relativePoses[i] == 1) { neighbourCoordY += 1; }
                        if (relativePoses[i] == 2) { neighbourCoordX += 1; }
                        if (relativePoses[i] == 3) { neighbourCoordY -= 1; }

                        doors.ForEach(delegate ((int X, int Y, int direction) door)
                        {
                            if (neighbourCoordX == door.X && neighbourCoordY == door.Y) { isDoor = true; }
                        });


                        if (!isDoor)
                        {
                            // Place a wall between them
                            instanciatePrefab(AssetType.wall_2x1, new Vector3(coordX * scale, 0, coordY * scale), Quaternion.AngleAxis(90 * relativePoses[i], Vector3.up), rooms[coordX, coordY]);
                        }
                        else
                        {
                            // Place a door between them
                            instanciatePrefab(AssetType.wallDoor_2x1, new Vector3(coordX * scale, 0, coordY * scale), Quaternion.AngleAxis(90 * relativePoses[i], Vector3.up), rooms[coordX, coordY]);
                        }
                    }
                }

                // Place a wall at every edge
                edgeCheck.ForEach(delegate (int edgeDirection)
                {
                    instanciatePrefab(AssetType.wall_2x1, new Vector3(coordX * scale, 0, coordY * scale), Quaternion.AngleAxis(90 * edgeDirection, Vector3.up), rooms[coordX, coordY]);
                });
            }
        }
    }

    private void placeAsset()
    {
        // Loop over each tiles to check if any walls to place
        for (int coordX = 0; coordX < levelSizeX; coordX++)
        {
            for (int coordY = 0; coordY < levelSizeY; coordY++)
            {
                // Find all the neighbours of the subTile
                List<int> relativePoses = new List<int>();
                List<int> neighbourRoomIDs = getNeighbours(coordX, coordY, ref relativePoses);

                List<int> edgeCheck = new List<int>(new int[] { 0, 2, 3, 1 });

                // Check if we are in a coridor and if we are against a wall
                bool isAgainstWall = false;
                bool isCorridor = false;
                int horizontalCount = 0;
                int verticalCount = 0;
                for (int neighbour = 0; neighbour < neighbourRoomIDs.Count; neighbour++)
                {
                    if (neighbourRoomIDs[neighbour] != rooms[coordX, coordY])
                    {
                        isAgainstWall = true;
                        if (relativePoses[neighbour] == 0 || relativePoses[neighbour] == 2) { horizontalCount++; }
                        if (relativePoses[neighbour] == 3 || relativePoses[neighbour] == 1) { verticalCount++; }
                    }
                    edgeCheck.Remove(relativePoses[neighbour]);
                }

                // For all the edges
                edgeCheck.ForEach(delegate (int edgeDirection)
                {
                    isAgainstWall = true;
                    if (edgeDirection == 0 || edgeDirection == 2) { horizontalCount++; }
                    if (edgeDirection == 3 || edgeDirection == 1) { verticalCount++; }
                });

                if (horizontalCount == 2 || verticalCount == 2) { isCorridor = true; }

                bool isAtCenter = true;
                List<int> relativeCornerPoses = new List<int>();
                List<int> neighbourCornerRoomIDs = getCornerNeighbours(coordX, coordY, ref relativePoses);

                for (int cornerNeighbour = 0; cornerNeighbour < neighbourCornerRoomIDs.Count; cornerNeighbour++)
                {
                    if (neighbourCornerRoomIDs[cornerNeighbour] != rooms[coordX, coordY])
                    {
                        isAtCenter = false;
                    }
                }

                bool isDoor = false;
                doors.ForEach(delegate ((int X, int Y, int direction) door)
                {
                    if (coordX == door.X && coordY == door.Y) { isDoor = true; }
                });

                // Loop over each neighbours
                for (int i = 0; i < neighbourRoomIDs.Count; i++)
                {
                    // If the neighbour has the same ID skip it
                    if (neighbourRoomIDs[i] == rooms[coordX, coordY]) { continue; }

                    int neighbourCoordX = coordX;
                    int neighbourCoordY = coordY;

                    if (relativePoses[i] == 0) { neighbourCoordX -= 1; }
                    if (relativePoses[i] == 1) { neighbourCoordY += 1; }
                    if (relativePoses[i] == 2) { neighbourCoordX += 1; }
                    if (relativePoses[i] == 3) { neighbourCoordY -= 1; }

                    doors.ForEach(delegate ((int X, int Y, int direction) door)
                    {
                        if (neighbourCoordX == door.X && neighbourCoordY == door.Y) { isDoor = true; }
                    });
                }

                edgeCheck = new List<int>(new int[] { 0, 2, 3, 1 });

                // ________    IF THE TILE IS A CENTER TILE      ________
                if (!isCorridor && !isAgainstWall && isAtCenter)
                {
                    // Here we want to place a double table
                    if (Random.Range(0, 100) < tableThreshold)
                    {
                        int orientation = Random.Range(0, 100);

                        // Loop over all the two subTiles
                        for (int i = 0; i < 2; i++)
                        {
                            // Get the subCoordinate
                            int subCoordX = i % 2;
                            int subCoordY = 0;

                            if (orientation % 2 == 1)
                            {
                                subCoordX = 0;
                                subCoordY = i % 2;
                            }

                            bool isFullTable = false;

                            if (coordX % 2 == 0 && coordY % 2 == 1 && Random.Range(0, 1) == 0)
                            {
                                if (Random.Range(0, 100) < tableThreshold) { isFullTable = true; }
                            }
                            else if (coordX % 2 == 1 && coordY % 2 == 0)
                            {
                                if (Random.Range(0, 100) < tableThreshold) { isFullTable = true; }
                            }

                            if (isFullTable && orientation % 2 == 0)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f) + scale / 4;
                                instanciatePrefab(AssetType.furnitureCenter_2x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(90.0f, Vector3.up), rooms[coordX, coordY]);
                            }
                            if (isFullTable && orientation % 2 == 1)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f) + scale / 4;
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                                instanciatePrefab(AssetType.furnitureCenter_2x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.identity, rooms[coordX, coordY]);
                            }
                        }
                    }

                    // Here we want to place a simple table
                    else
                    {
                        // Loop over all the subTiles
                        for (int i = 0; i < 4; i++)
                        {
                            // Get the subCoordinate
                            int subCoordX = i % 2;
                            int subCoordY = i / 2;

                            bool isFullTable = false;

                            if (coordX % 2 == 0 && coordY % 2 == 1 && Random.Range(0, 1) == 0)
                            {
                                if (Random.Range(0, 100) < tableThreshold) { isFullTable = true; }
                            }
                            else if (coordX % 2 == 1 && coordY % 2 == 0)
                            {
                                if (Random.Range(0, 100) < tableThreshold) { isFullTable = true; }
                            }

                            if (isFullTable)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                                instanciatePrefab(AssetType.furnitureCenter_1x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(Random.Range(0, 100) * 90, Vector3.up), rooms[coordX, coordY]);
                            }
                        }
                    }
                }

                // ________    IF THE TILE IS AGAINST A WALL     ________
                else if (!isCorridor && isAgainstWall && !isDoor)
                {
                    // Loop over all the subTiles
                    for (int i = 0; i < 4; i++)
                    {
                        // Get the subCoordinate
                        int subCoordX = i % 2;
                        int subCoordY = i / 2;

                        int subtileType = 0;

                        // Check the two possible neighbours
                        for (int neighbour = 0; neighbour < neighbourRoomIDs.Count; neighbour++)
                        {
                            if (relativePoses[neighbour] == 0 && subCoordX == 0 && neighbourRoomIDs[neighbour] != rooms[coordX, coordY]) { subtileType += 1; }
                            if (relativePoses[neighbour] == 2 && subCoordX == 1 && neighbourRoomIDs[neighbour] != rooms[coordX, coordY]) { subtileType += 2; }
                            if (relativePoses[neighbour] == 3 && subCoordY == 0 && neighbourRoomIDs[neighbour] != rooms[coordX, coordY]) { subtileType += 4; }
                            if (relativePoses[neighbour] == 1 && subCoordY == 1 && neighbourRoomIDs[neighbour] != rooms[coordX, coordY]) { subtileType += 8; }

                            edgeCheck.Remove(relativePoses[neighbour]);
                        }

                        // Append every edges
                        edgeCheck.ForEach(delegate (int edgeDirection)
                        {
                            if (edgeDirection == 0 && subCoordX == 0) { subtileType += 1; }
                            if (edgeDirection == 2 && subCoordX == 1) { subtileType += 2; }
                            if (edgeDirection == 3 && subCoordY == 0) { subtileType += 4; }
                            if (edgeDirection == 1 && subCoordY == 1) { subtileType += 8; }
                        });

                        // If the subtile is a corner
                        if (subtileType != 1 && subtileType != 2 && subtileType != 4 && subtileType != 8 && subtileType != 0)
                        {
                            if (Random.Range(0, 100) > sideThreshold)
                            {
                                availableTiles.Add(((float)coordX * scale + (subCoordX - 0.5f), (float)coordY * scale + (subCoordY - 0.5f)));
                                continue;
                            }

                            float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                            float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                            int angle = 90;
                            if (subCoordY == 1) { angle *= subCoordX; }
                            if (subCoordY == 0) { angle *= (1 - subCoordX) + 2; }
                            instanciatePrefab(AssetType.furnitureCorner_1x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(angle, Vector3.up), rooms[coordX, coordY]);
                        }

                        // If the subtile is along a wall
                        else if (subtileType == 1 || subtileType == 2 || subtileType == 4 || subtileType == 8)
                        {
                            if (Random.Range(0, 100) > sideThreshold)
                            {
                                availableTiles.Add(((float)coordX * scale + (subCoordX - 0.5f), (float)coordY * scale + (subCoordY - 0.5f)));
                                continue;
                            }

                            if (subtileType == 1)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                                instanciatePrefab(AssetType.furnitureWall_1x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(0, Vector3.up), rooms[coordX, coordY]);
                            }
                            if (subtileType == 2)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                                instanciatePrefab(AssetType.furnitureWall_1x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(180, Vector3.up), rooms[coordX, coordY]);
                            }
                            if (subtileType == 4)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                                instanciatePrefab(AssetType.furnitureWall_1x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(-90, Vector3.up), rooms[coordX, coordY]);
                            }
                            if (subtileType == 8)
                            {
                                float positionX = (float)coordX * scale + (subCoordX - 0.5f);
                                float positionZ = (float)coordY * scale + (subCoordY - 0.5f);
                                instanciatePrefab(AssetType.furnitureWall_1x1, new Vector3(positionX, 0.0f, positionZ), Quaternion.AngleAxis(90, Vector3.up), rooms[coordX, coordY]);
                            }
                        }
                    }
                }
                else { availableTiles.Add((coordX * scale, coordY * scale)); }

                // Place the floor
                instanciatePrefab(AssetType.floor_2x2, new Vector3(coordX * scale, 0.0f, coordY * scale), Quaternion.identity, rooms[coordX, coordY]);
            }
        }

        shuffleList(ref availableTiles);
        for (int i = 0; i < availableTiles.Count && i < coin; i++)
        {
            Instantiate(coinPrefab, new Vector3(availableTiles[i].X, 0.0f, availableTiles[i].Y), Quaternion.identity);
        }
    }

    private List<int> getNeighbours(int coordX, int coordY, ref List<int> relativePoses)
    {
        // Initialize the neighbourd ID
        List<int> neighbourRoomIDs = new List<int>();

        // Get the neighbour tiles
        if (coordX > 0)
        {
            if (rooms[coordX - 1, coordY] >= 0)
            {
                neighbourRoomIDs.Add(rooms[coordX - 1, coordY]);
                relativePoses.Add(0);
            }
        }
        if (coordX < levelSizeX - 1)
        {
            if (rooms[coordX + 1, coordY] >= 0)
            {
                neighbourRoomIDs.Add(rooms[coordX + 1, coordY]);
                relativePoses.Add(2);
            }
        }
        if (coordY > 0)
        {
            if (rooms[coordX, coordY - 1] >= 0)
            {
                neighbourRoomIDs.Add(rooms[coordX, coordY - 1]);
                relativePoses.Add(3);
            }
        }
        if (coordY < levelSizeY - 1)
        {
            if (rooms[coordX, coordY + 1] >= 0)
            {
                neighbourRoomIDs.Add(rooms[coordX, coordY + 1]);
                relativePoses.Add(1);
            }
        }

        return neighbourRoomIDs;
    }

    private List<int> getCornerNeighbours(int coordX, int coordY, ref List<int> relativePoses)
    {
        // Initialize the corner neighbours ID
        List<int> neighbourCornerRoomIDs = new List<int>();

        if (coordX > 0 && coordY > 0)
        {
            if (rooms[coordX - 1, coordY - 1] >= 0)
            {
                neighbourCornerRoomIDs.Add(rooms[coordX - 1, coordY - 1]);
                relativePoses.Add(0);
            }
        }
        if (coordX > 0 && coordY < levelSizeY - 1)
        {
            if (rooms[coordX - 1, coordY + 1] >= 0)
            {
                neighbourCornerRoomIDs.Add(rooms[coordX - 1, coordY + 1]);
                relativePoses.Add(1);
            }
        }
        if (coordX < levelSizeX - 1 && coordY > 0)
        {
            if (rooms[coordX + 1, coordY - 1] >= 0)
            {
                neighbourCornerRoomIDs.Add(rooms[coordX + 1, coordY - 1]);
                relativePoses.Add(2);
            }
        }
        if (coordX < levelSizeX - 1 && coordY < levelSizeY - 1)
        {
            if (rooms[coordX + 1, coordY + 1] >= 0)
            {
                neighbourCornerRoomIDs.Add(rooms[coordX + 1, coordY + 1]);
                relativePoses.Add(3);
            }
        }

        return neighbourCornerRoomIDs;
    }

    private void instanciatePrefab(AssetType asset, Vector3 position, Quaternion angle, int room)
    {
        GameObject[] prefabs;

        switch (asset)
        {
            case AssetType.wall_2x1:
                if (roomTypes[room] == 0) { prefabs = wallKitchen_2x1; }
                else if (roomTypes[room] == 1) { prefabs = wallBatthroom_2x1; }
                else if (roomTypes[room] == 2) { prefabs = wallLiving_2x1; }
                else { return; }
                break;
            case AssetType.wallDoor_2x1:
                if (roomTypes[room] == 0) { prefabs = wallDoorKitchen_2x1; }
                else if (roomTypes[room] == 1) { prefabs = wallDoorBatthroom_2x1; }
                else if (roomTypes[room] == 2) { prefabs = wallDoorLiving_2x1; }
                else { return; }
                break;
            case AssetType.door_1x1:
                if (roomTypes[room] == 0) { prefabs = doorKitchen_1x1; }
                else if (roomTypes[room] == 1) { prefabs = doorBatthroom_1x1; }
                else if (roomTypes[room] == 2) { prefabs = doorLiving_1x1; }
                else { return; }
                break;
            case AssetType.furnitureWall_1x1:
                if (roomTypes[room] == 0) { prefabs = furnitureWallKitchen_1x1; }
                else if (roomTypes[room] == 1) { prefabs = furnitureWallBatthroom_1x1; }
                else if (roomTypes[room] == 2) { prefabs = furnitureWallLiving_1x1; }
                else { return; }
                break;
            case AssetType.furnitureCorner_1x1:
                if (roomTypes[room] == 0) { prefabs = furnitureCornerKitchen_1x1; }
                else if (roomTypes[room] == 1) { prefabs = furnitureCornerBatthroom_1x1; }
                else if (roomTypes[room] == 2) { prefabs = furnitureCornerLiving_1x1; }
                else { return; }
                break;
            case AssetType.furnitureCenter_1x1:
                if (roomTypes[room] == 0) { prefabs = furnitureCenterKitchen_1x1; }
                else if (roomTypes[room] == 1) { prefabs = furnitureCenterBatthroom_1x1; }
                else if (roomTypes[room] == 2) { prefabs = furnitureCenterLiving_1x1; }
                else { return; }
                break;
            case AssetType.furnitureCenter_2x1:
                if (roomTypes[room] == 0) { prefabs = furnitureCenterKitchen_2x1; }
                else if (roomTypes[room] == 1) { prefabs = furnitureCenterBatthroom_2x1; }
                else if (roomTypes[room] == 2) { prefabs = furnitureCenterLiving_2x1; }
                else { return; }
                break;
            case AssetType.floor_2x2:
                if (roomTypes[room] == 0) { prefabs = floorKitchen_2x2; }
                else if (roomTypes[room] == 1) { prefabs = floorBatthroom_2x2; }
                else if (roomTypes[room] == 2) { prefabs = floorLiving_2x2; }
                else { return; }
                break;
            default:
                return;
        }

        // The list of prefab we can instantialte
        List<GameObject> prefabList = new List<GameObject>(prefabs);
        // The list of the index of the prefab we can instansiate
        List<int> prefabIndex = new List<int>();

        int prefabCount = prefabList.Count;

        // The total weight all the the prefab we can instansiate
        int totalWeight = 0;

        List<int> toRemove = new List<int>();


        // Loop over all the prefabs
        for (int prefab = 0; prefab < prefabCount; prefab++)
        {
            // Get the weight infos of a prefab
            ScatterInfo prefabInfo = prefabList[prefab].GetComponent<ScatterInfo>();
            int prefabInstanceCount = 0;
            if (roomTypes[room] == 0)
            {
                prefabInstanceCount = kitchenFurnitureCount[roomLink[room]][(int)asset][prefab];
            }
            else if (roomTypes[room] == 1)
            {
                prefabInstanceCount = bathroomFurnitureCount[roomLink[room]][(int)asset][prefab];
            }
            else if (roomTypes[room] == 2)
            {
                prefabInstanceCount = livingFurnitureCount[roomLink[room]][(int)asset][prefab];
            }

            if (prefabInstanceCount >= prefabInfo.maxElement)
            {
                toRemove.Add(prefab);
                continue;
            }

            // Append its index
            prefabIndex.Add(prefab);
            // Append its weight
            totalWeight += prefabInfo.weight;
        }

        for (int removeIndex = 0; removeIndex < toRemove.Count; removeIndex++)
        {
            prefabList.RemoveAt(toRemove[removeIndex]);
        }

        // Choose a random weight among the weight
        int chosenIndex = Random.Range(0, totalWeight);

        // Loop over all the prefabs
        int previousWeight = 0;
        int nextWeight = 0;


        for (int prefab = 0; prefab < prefabList.Count; prefab++)
        {
            // The the weight of the next prefab
            nextWeight += prefabList[prefab].GetComponent<ScatterInfo>().weight;

            if (chosenIndex >= previousWeight && chosenIndex <= nextWeight)
            {
                Instantiate(prefabs[prefabIndex[prefab]], position, angle);
                if (roomTypes[room] == 0)
                {
                    kitchenFurnitureCount[roomLink[room]][(int)asset][prefabIndex[prefab]] += 1;
                }
                else if (roomTypes[room] == 1)
                {
                    bathroomFurnitureCount[roomLink[room]][(int)asset][prefabIndex[prefab]] += 1;
                }
                else if (roomTypes[room] == 2)
                {
                    livingFurnitureCount[roomLink[room]][(int)asset][prefabIndex[prefab]] += 1;
                }
                break;
            }

            // The the weight of the previous prefab
            previousWeight = nextWeight;
        }
    }

    private void shuffleList(ref List<GameObject> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            GameObject value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void shuffleList(ref List<(int room, List<(int X, int Y, int direction)> tiles)> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (int room, List<(int X, int Y, int direction)> tiles) value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void shuffleList(ref List<(float X, float Y)> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (float X, float Y) value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}