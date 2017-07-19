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
    public GameObject CloudBlock;

    private Block[,,] world = new Block[Width, Height, Depth];

    void Start()
    {
        CreateLandscape();
        CreateClouds(20, 30);
    }

    private void CreateClouds(int numClouds, int cloudSize)
    {

        for (int i = 0; i < numClouds; i++)
        {
            int xPos = Random.Range(0, Width);
            int zPos = Random.Range(0, Depth);
            for (int j = 0; j < cloudSize; j++)
            {
                CreateCloud(new Vector3(xPos, Height - 1, zPos));
                xPos += Random.Range(-1, 2);
                zPos += Random.Range(-1, 2);
                if (xPos < 0) { xPos = 0; } else if (xPos >= Width) { xPos = Width; }
                if (zPos < 0) { zPos = 0; } else if (zPos >= Depth) { zPos = Depth; }
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
        GameObject blockType = grass ? GrassBlock: DirtBlock;
        if (visible)
        {
            GameObject prefab = Instantiate(blockType, position, Quaternion.identity);
            prefab.transform.parent = gameObject.transform;
        }

        world[(int)position.x, (int)position.y, (int)position.z] = new Block(visible);
    }

    private void CreateCloud(Vector3 position)
    {

        GameObject prefab = Instantiate(CloudBlock, position, Quaternion.identity);
        prefab.transform.parent = gameObject.transform;
        world[(int)position.x, (int)position.y, (int)position.z] = new Block(true);

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
