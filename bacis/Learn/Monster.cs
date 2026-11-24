public class Monster
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsAlive { get; private set; }
    public char Symbol => 'M';

    public Monster(int x, int y)
    {
        X = x;
        Y = y;
        IsAlive = true;
    }

    public void Kill()
    {
        IsAlive = false;
    }
}
