using UnityEngine;

public class Block
{
    public bool Visible;
    public GameObject BlockObject;

    private float Health = 100f;

    public Block(bool visible, GameObject block)
    {
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