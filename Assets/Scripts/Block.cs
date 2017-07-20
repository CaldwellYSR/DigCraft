using UnityEngine;

public class Block
{
    public bool Visible;
    public BlockType Type;
    public GameObject BlockObject;

    private float Health = 100f;

    public Block(BlockType type, bool visible, GameObject block)
    {
        Type = type;
        Visible = visible;
        BlockObject = block;
    }

    public void Damage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
        }
    }

    public float GetHealth()
    {
        return Health;
    }
}