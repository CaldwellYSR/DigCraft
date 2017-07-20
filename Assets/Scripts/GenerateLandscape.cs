using UnityEngine;

public class GenerateLandscape : MonoBehaviour
{

    public float PlayerRange = 1000f;
    public float PlayerPower = 10f;

    private static int Width = 128;
    private static int Depth = 128;
    private static int Height = 128;
    public int HeightOffset = 100;
    public int HeightScale = 20;
    public float DetailScale = 25f;

    public GameObject GrassBlock;
    public GameObject DirtBlock;
    public GameObject StoneBlock;
    public GameObject SnowBlock;
    public GameObject GoldBlock;
    public GameObject CloudBlock;

    private Block[,,] world = new Block[Width, Height, Depth];

    void Start()
    {
        CreateLandscape();
        CreateClouds(20, 50);
        DigMines(5, 600);
    }

    private void DigMines(int numMines, int mineSize)
    {
        int holeSize = 2;

        // Dig the holes
        for (int i = 0; i < numMines; i++)
        {
            int xPos = Random.Range(10, Width - 10);
            int yPos = Random.Range(10, Height - 10);
            int zPos = Random.Range(10, Depth - 10);
            for (int j = 0; j < mineSize; j++) {
                for (int x = -holeSize; x < holeSize; x++)
                {
                    for (int y = -holeSize; y < holeSize; y++)
                    {
                        for (int z = -holeSize; z < holeSize; z++)
                        {
                            if (!(x == 0 && y == 0 && z == 0))
                            {
                                Vector3 blockPosition = new Vector3(xPos + x, yPos + y, zPos + z);
                                if (world[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] != null)
                                {
                                    if (world[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z].BlockObject != null)
                                    {
                                        Destroy(world[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z].BlockObject);
                                    }
                                }
                                world[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] = null;
                            }
                        }
                    }
                }

                xPos += Random.Range(-1, 2);
                yPos += Random.Range(-1, 2);
                zPos += Random.Range(-1, 2);
                if (xPos <= holeSize || xPos >= Width - holeSize) { xPos = (int)Mathf.Floor(Width * 0.5f); }
                if (yPos <= holeSize || yPos >= Height - holeSize) { yPos = (int)Mathf.Floor(Height * 0.5f); }
                if (zPos <= holeSize || zPos >= Depth - holeSize) { zPos = (int)Mathf.Floor(Depth * 0.5f); }

            }

        }

        // Build walls around holes
        for (int z = 1; z < Depth - 1; z++)
        {
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {

                    if (world[x, y, z] == null)
                    {
                        for (int x1 = -1; x1 <= 1; x1++)
                        {
                            for (int y1 = -1; y1 <= 1; y1++)
                            {
                                for (int z1 = -1; z1 <= 1; z1++)
                                {
                                    if (!(x1 == 0 && y1 == 0 && z1 == 0))
                                    {
                                        Vector3 neighbor = new Vector3(x + x1, y + y1, z + z1);
                                        DrawBlock(neighbor);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

    }

    private void CreateClouds(int numClouds, int cloudSize)
    {

        for (int i = 0; i < numClouds; i++)
        {
            int xPos = Random.Range(0, Width);
            int zPos = Random.Range(0, Depth);
            for (int j = 0; j < cloudSize; j++)
            {
                DrawCloud(new Vector3(xPos, Height - 1, zPos));
                xPos += Random.Range(-1, 2);
                zPos += Random.Range(-1, 2);
                if (xPos <= 0 || xPos >= Width) { xPos = (int)Mathf.Floor(Width * 0.5f); }
                if (zPos <= 0 || zPos >= Depth) { zPos = (int)Mathf.Floor(Depth * 0.5f); }
            }
        }

    }

    private void CreateLandscape()
    {
        int seed = (int)Network.time * 10;
        for (int z = 0; z < Depth; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                int y = (int)(Mathf.PerlinNoise((x + seed) / DetailScale, (z + seed) / DetailScale) * HeightScale) + HeightOffset;
                Vector3 blockPosition = new Vector3(x, y, z);

                CreateBlock(blockPosition, true, true);

                while (y-- > 0)
                {
                    blockPosition.y = y;
                    CreateBlock(blockPosition, false, false);
                }
            }
        }
    }

    private void CreateBlock(Vector3 position, bool grass, bool visible)
    {
        GameObject prefab = null;
        GameObject blockType;
        BlockType type;

        if (position.y > HeightOffset + (Height - HeightOffset) * 0.5f)
        {
            blockType = SnowBlock;
            type = BlockType.Snow;
        }
        else if (position.y > HeightOffset * 0.75f)
        {
            blockType = DirtBlock;
            type = BlockType.Dirt;
        }
        else if (position.y > HeightOffset * 0.65f && Random.Range(0, 100) < 35)
        {
            blockType = DirtBlock;
            type = BlockType.Dirt;
        }
        else if (position.y > HeightOffset * 0.35f)
        {
            blockType = StoneBlock;
            type = BlockType.Stone;
        }
        else if (position.y > 0 && Random.Range(0, 1000) < 5)
        {
            blockType = GoldBlock;
            type = BlockType.Gold;
        }
        else
        {
            blockType = StoneBlock;
            type = BlockType.Stone;
        }

        if (grass && type != BlockType.Snow)
        {
            blockType = GrassBlock;
            type = BlockType.Grass;
        }

        if (visible)
        {
            prefab = Instantiate(blockType, position, Quaternion.identity);
            prefab.transform.parent = gameObject.transform;
        }

        world[(int)position.x, (int)position.y, (int)position.z] = new Block(type, visible, prefab);
    }

    private void DrawCloud(Vector3 position)
    {

        GameObject prefab = Instantiate(CloudBlock, position, Quaternion.identity);
        prefab.transform.parent = gameObject.transform;
        world[(int)position.x, (int)position.y, (int)position.z] = new Block(BlockType.Cloud, true, prefab);

    }

    private void DrawBlock(Vector3 position)
    {
        // Cube is outside the map
        if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height || position.z < 0 || position.z >= Depth)
        {
            return;
        }

        // Cube is already destroyed
        if (world[(int)position.x, (int)position.y, (int)position.z] == null)
        {
            return;
        }

        // Cube isn't visible, draw it
        if (!world[(int)position.x, (int)position.y, (int)position.z].Visible)
        {
            world[(int)position.x, (int)position.y, (int)position.z].Visible = true;
            CreateBlock(position, false, true);
        }

    }

    private void DamageBlock()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (Physics.Raycast(ray, out hit, PlayerRange) && hit.transform.gameObject.CompareTag("Block"))
        {
            Vector3 blockPosition = hit.transform.position;

            // Don't destroy the last block
            if ((int)blockPosition.y == 0)
            {
                return;
            }

            Block block = world[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z];

            float damage = PlayerPower * Time.deltaTime;
            
            block.Damage(damage);

            if (block.GetHealth() == 0)
            {
                DestroyBlock(hit.transform.gameObject);
            }

        }
    }

    private void DestroyBlock(GameObject block)
    {

        Vector3 blockPosition = block.transform.position;
        world[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] = null;
        Destroy(block.transform.gameObject);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (!(x == 0 && y == 0 && z == 0))
                    {
                        Vector3 neighbor = new Vector3(blockPosition.x + x, blockPosition.y + y, blockPosition.z + z);
                        DrawBlock(neighbor);
                    }
                }
            }
        }
    }

    void Update()
    {

        if (Input.GetButton("Shoot"))
        {
            DamageBlock();
        }

    }

}
