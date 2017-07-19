public class Block
{
    public bool Visible;

    private float Health = 100f;

    public Block(bool visible)
    {
        Visible = visible;
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