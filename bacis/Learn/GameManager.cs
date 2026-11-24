using System;
using System.Collections.Generic;

// 게임 상태, 웨이브, 몬스터 생존 수를 관리하는 정적 클래스
public static class GameManager
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    public static bool IsRunning { get; private set; }

    public static int CurrentWave { get; private set; }
    public static int AliveMonsterCount => Monsters.Count;

    public static Player Player { get; private set; } = null!;
    public static List<Monster> Monsters { get; } = new();
    public static List<Bullet> Bullets { get; } = new();

    private static readonly Random _random = new();

    public static void Initialize(int width, int height)
    {
        Width = width;
        Height = height;
        IsRunning = true;
        CurrentWave = 0;

        Player = new Player(x: 2, y: height / 2);
        Monsters.Clear();
        Bullets.Clear();
    }

    public static void StartNextWave()
    {
        CurrentWave++;
        int spawnCount = CurrentWave + 2; // Wave가 오를수록 몬스터 수 증가 (3마리, 4마리...)

        for (int i = 0; i < spawnCount; i++)
        {
            int spawnX = _random.Next(Width / 2, Width - 2);
            int spawnY = _random.Next(1, Height - 1);
            Monsters.Add(new Monster(spawnX, spawnY));
        }
    }

    // 1. 입력 처리 (WASD 이동, Spacebar 발사, ESC 종료)
    public static void HandleInput()
    {
        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.W:
                    Player.Move(0, -1, Width, Height);
                    break;
                case ConsoleKey.S:
                    Player.Move(0, 1, Width, Height);
                    break;
                case ConsoleKey.A:
                    Player.Move(-1, 0, Width, Height);
                    break;
                case ConsoleKey.D:
                    Player.Move(1, 0, Width, Height);
                    break;
                case ConsoleKey.Spacebar:
                    Bullets.Add(Player.Shoot());
                    break;
                case ConsoleKey.Escape:
                    IsRunning = false;
                    break;
            }
        }
    }

    // 2. 게임 로직 및 상태 업데이트
    public static void Update()
    {
        // 총알 직선 이동 및 화면 밖 제거
        for (int i = Bullets.Count - 1; i >= 0; i--)
        {
            Bullets[i].Move();
            if (Bullets[i].X >= Width - 1)
            {
                Bullets.RemoveAt(i);
            }
        }

        // 총알과 몬스터 충돌 판정 및 사망 처리
        for (int b = Bullets.Count - 1; b >= 0; b--)
        {
            for (int m = Monsters.Count - 1; m >= 0; m--)
            {
                if (Bullets[b].X == Monsters[m].X && Bullets[b].Y == Monsters[m].Y)
                {
                    Monsters[m].Kill();
                    Monsters.RemoveAt(m);
                    Bullets.RemoveAt(b);
                    break;
                }
            }
        }

        // 모든 몬스터가 사망하면 다음 Wave 발생
        if (AliveMonsterCount == 0)
        {
            StartNextWave();
        }
    }

    // 3. 화면 렌더링
    public static void Render()
    {
        char[,] buffer = new char[Height, Width];

        // 배경 및 테두리 초기화
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                bool isBorder = (y == 0 || y == Height - 1 || x == 0 || x == Width - 1);
                buffer[y, x] = isBorder ? '#' : ' ';
            }
        }

        // 오브젝트 배치
        buffer[Player.Y, Player.X] = Player.Symbol;

        foreach (var monster in Monsters)
        {
            buffer[monster.Y, monster.X] = monster.Symbol;
        }

        foreach (var bullet in Bullets)
        {
            buffer[bullet.Y, bullet.X] = bullet.Symbol;
        }

        // 콘솔 출력
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"[Wave: {CurrentWave}]  남은 몬스터: {AliveMonsterCount}마리   (이동: WASD / 발사: Space / 종료: ESC)   ");

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.Write(buffer[y, x]);
            }
            Console.WriteLine();
        }
    }
}
