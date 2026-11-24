using System;

public class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public char Symbol => 'P';

    public Player(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Move(int deltaX, int deltaY, int mapWidth, int mapHeight)
    {
        int nextX = Math.Clamp(X + deltaX, 1, mapWidth - 2);
        int nextY = Math.Clamp(Y + deltaY, 1, mapHeight - 2);

        X = nextX;
        Y = nextY;
    }

    public Bullet Shoot()
    {
        return new Bullet(X + 1, Y);
    }
}
