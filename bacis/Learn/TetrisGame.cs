using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;

namespace TetrisConsole
{
    public class TetrisGame
    {
        public class 블록
        {
            public int X { get; set; }
            public int Y { get; set; }
            public ConsoleColor Color { get; set; } = ConsoleColor.White;
            public bool IsFixed { get; protected set; } = false;

            // 블록이 바닥이나 다른 블록 위에 안착해 고정될 때 호출
            public virtual void OnBlockPlaced()
            {
                IsFixed = true;
            }

            // 이 블록이 속한 줄이 완성되어 보드에서 지워질 때 호출
            public virtual void OnLineCleared()
            {
                // 기본 블록의 줄 제거 시 처리 로직 (예: 보드 데이터에서 삭제 등)
            }

            // 콘솔 화면에 블록 출력
            public virtual void Draw()
            {
                Console.SetCursorPosition(X * 2, Y);
                Console.ForegroundColor = Color;
                Console.Write("■");
                Console.ResetColor();
            }
        }

        public class TimerBlock : 블록
        {
            public double deathTimer = 3.0;
            public bool IsDestroyed => deathTimer <= 0;

            public TimerBlock(string shape, double seconds = 3.0)
                : base(shape, ConsoleColor.Yellow)
            {
                this.deathTimer = seconds;
            }

            public void TimeDeath(double deltaTime)
            {
                if (deathTimer > 0)
                {
                    deathTimer -= deltaTime;
                }
            }
        }

        private const int Width = 10;
        private const int Height = 20;

        private 블록[,] board = new 블록[Height, Width];
        private Random rand = new Random();

        private int[,] currentPiece;
        private int currentX;
        private int currentY;
        private bool currentIsTimerBlock;
        private ConsoleColor currentColor;

        private bool gameOver = false;
        private int score = 0;

        private double dropTimer = 0;
        private double dropInterval = 0.5;

        private readonly int[][,] shapes = new int[][,]
        {
            new int[,] { { 1, 1, 1, 1 } },
            new int[,] { { 1, 1 }, { 1, 1 } },
            new int[,] { { 0, 1, 0 }, { 1, 1, 1 } },
            new int[,] { { 0, 1, 1 }, { 1, 1, 0 } },
            new int[,] { { 1, 1, 0 }, { 0, 1, 1 } },
            new int[,] { { 1, 0, 0 }, { 1, 1, 1 } },
            new int[,] { { 0, 0, 1 }, { 1, 1, 1 } }
        };

        private readonly ConsoleColor[] colors = new ConsoleColor[]
        {
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
            ConsoleColor.Green,
            ConsoleColor.Red,
            ConsoleColor.DarkYellow,
            ConsoleColor.Gray
        };

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            TetrisGame game = new TetrisGame();
            game.Run();
        }

        public void Run()
        {
            SpawnPiece();
            Stopwatch sw = Stopwatch.StartNew();
            double lastTime = sw.Elapsed.TotalSeconds;

            while (!gameOver)
            {
                double currentTime = sw.Elapsed.TotalSeconds;
                double deltaTime = currentTime - lastTime;
                lastTime = currentTime;

                ProcessInput();
                Update(deltaTime);
                Render();

                Thread.Sleep(30);
            }

            Console.SetCursorPosition(0, Height + 4);
            Console.ResetColor();
            Console.WriteLine($"게임 오버! 최종 점수: {score}");
            Console.CursorVisible = true;
        }

        // 1. 입력 (Input)
        private void ProcessInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (!CheckCollision(currentPiece, currentX - 1, currentY))
                            currentX--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (!CheckCollision(currentPiece, currentX + 1, currentY))
                            currentX++;
                        break;
                    case ConsoleKey.DownArrow:
                        if (!CheckCollision(currentPiece, currentX, currentY + 1))
                        {
                            currentY++;
                            dropTimer = 0;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        int[,] rotated = RotateMatrix(currentPiece);
                        if (!CheckCollision(rotated, currentX, currentY))
                            currentPiece = rotated;
                        break;
                    case ConsoleKey.Spacebar:
                        while (!CheckCollision(currentPiece, currentX, currentY + 1))
                        {
                            currentY++;
                        }
                        LockPiece();
                        ClearLines();
                        SpawnPiece();
                        dropTimer = 0;
                        break;
                }
            }
        }

        // 2. 갱신 (Update)
        private void Update(double deltaTime)
        {
            // 보드 위 TimerBlock 시간 갱신 및 파괴 처리
            bool anyTimerBlockDestroyed = false;

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if (board[r, c] is TimerBlock tb)
                    {
                        tb.TimeDeath(deltaTime);
                        if (tb.IsDestroyed)
                        {
                            board[r, c] = null;
                            anyTimerBlockDestroyed = true;
                        }
                    }
                }
            }

            // 파괴된 TimerBlock 위의 블록들을 자동으로 아래로 내림
            if (anyTimerBlockDestroyed)
            {
                ApplyGravityToBoard();
                ClearLines();
            }

            // 현재 조작 중인 블록 자동 낙하
            dropTimer += deltaTime;
            if (dropTimer >= dropInterval)
            {
                dropTimer = 0;
                if (!CheckCollision(currentPiece, currentX, currentY + 1))
                {
                    currentY++;
                }
                else
                {
                    LockPiece();
                    ClearLines();
                    SpawnPiece();
                }
            }
        }

        // 3. 렌더링 (Render)
        private void Render()
        {
            Console.SetCursorPosition(0, 0);

            // 임시 화면 버퍼 구성
            블록[,] displayBoard = new 블록[Height, Width];
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    displayBoard[r, c] = board[r, c];
                }
            }

            // 현재 떨어지는 블록을 임시 버퍼에 투영
            if (currentPiece != null)
            {
                int rows = currentPiece.GetLength(0);
                int cols = currentPiece.GetLength(1);
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (currentPiece[r, c] == 1)
                        {
                            int br = currentY + r;
                            int bc = currentX + c;
                            if (br >= 0 && br < Height && bc >= 0 && bc < Width)
                            {
                                displayBoard[br, bc] = currentIsTimerBlock
                                    ? new TimerBlock("▣", 3.0)
                                    : new 블록("■", currentColor);
                            }
                        }
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌" + new string('─', Width * 2) + "┐");

            for (int r = 0; r < Height; r++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("│");

                for (int c = 0; c < Width; c++)
                {
                    블록 cell = displayBoard[r, c];
                    if (cell != null)
                    {
                        Console.ForegroundColor = cell.Color;
                        if (cell is TimerBlock tb)
                        {
                            // 남은 시간(초)을 올림하여 숫자로 표시 (예: 3, 2, 1)
                            int remainSec = Math.Max(1, (int)Math.Ceiling(tb.deathTimer));
                            Console.Write(remainSec + " ");
                        }
                        else
                        {
                            Console.Write(cell.shape + " ");
                        }
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("│");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("└" + new string('─', Width * 2) + "┘");
            Console.WriteLine($"점수: {score}   |   노란 숫자 블록: 3초 후 파괴");
            Console.WriteLine("조작: ←/→ 이동 | ↑ 회전 | ↓ 소프트드롭 | Space 하드드롭");
        }

        private void ApplyGravityToBoard()
        {
            for (int c = 0; c < Width; c++)
            {
                int writeRow = Height - 1;
                for (int r = Height - 1; r >= 0; r--)
                {
                    if (board[r, c] != null)
                    {
                        블록 temp = board[r, c];
                        board[r, c] = null;
                        board[writeRow, c] = temp;
                        writeRow--;
                    }
                }
            }
        }

        private void SpawnPiece()
        {
            int idx = rand.Next(shapes.Length);
            currentPiece = shapes[idx];
            currentColor = colors[idx];
            // 30% 확률로 TimerBlock 조각 생성
            currentIsTimerBlock = rand.NextDouble() < 0.3;

            currentX = (Width - currentPiece.GetLength(1)) / 2;
            currentY = 0;

            if (CheckCollision(currentPiece, currentX, currentY))
            {
                gameOver = true;
            }
        }

        private bool CheckCollision(int[,] piece, int x, int y)
        {
            int rows = piece.GetLength(0);
            int cols = piece.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (piece[r, c] == 1)
                    {
                        int newX = x + c;
                        int newY = y + r;

                        if (newX < 0 || newX >= Width || newY >= Height)
                            return true;

                        if (newY >= 0 && board[newY, newX] != null)
                            return true;
                    }
                }
            }
            return false;
        }

        private void LockPiece()
        {
            int rows = currentPiece.GetLength(0);
            int cols = currentPiece.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (currentPiece[r, c] == 1)
                    {
                        int br = currentY + r;
                        int bc = currentX + c;
                        if (br >= 0 && br < Height && bc >= 0 && bc < Width)
                        {
                            board[br, bc] = currentIsTimerBlock
                                ? new TimerBlock("▣", 3.0)
                                : new 블록("■", currentColor);
                        }
                    }
                }
            }
        }

        private void ClearLines()
        {
            int linesCleared = 0;
            for (int r = Height - 1; r >= 0; r--)
            {
                bool full = true;
                for (int c = 0; c < Width; c++)
                {
                    if (board[r, c] == null)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    linesCleared++;
                    for (int moveRow = r; moveRow > 0; moveRow--)
                    {
                        for (int c = 0; c < Width; c++)
                        {
                            board[moveRow, c] = board[moveRow - 1, c];
                        }
                    }
                    for (int c = 0; c < Width; c++)
                    {
                        board[0, c] = null;
                    }
                    r++;
                }
            }

            if (linesCleared > 0)
            {
                score += linesCleared * 100;
            }
        }

        private int[,] RotateMatrix(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    rotated[c, rows - 1 - r] = matrix[r, c];
                }
            }
            return rotated;
        }
        public class RiskBlock : 블록
        {
            public double timeLimitMs = 10000; // 제한 시간 (10초 = 10,000ms)
            public int bonusScore = 100;
            public int penaltyScore = -100;

            private bool isPlaced = false;
            private bool isResolved = false;
            private Timer riskTimer;
            private readonly object lockObj = new object(); // 스레드 충돌 방지

            public RiskBlock(string shape, ConsoleColor color = ConsoleColor.White) : base(shape, color)
            {
            }

            // 블록이 바닥에 고정될 때 호출
            public override void OnBlockPlaced()
            {
                base.OnBlockPlaced();

                if (!isPlaced)
                {
                    isPlaced = true;
                    riskTimer = new Timer(timeLimitMs);
                    riskTimer.Elapsed += OnTimeExpired;
                    riskTimer.AutoReset = false; // 한 번만 실행
                    riskTimer.Start();
                }
            }

            // 제한 시간 초과 시 백그라운드 스레드에서 자동 호출
            private void OnTimeExpired(object sender, ElapsedEventArgs e)
            {
                lock (lockObj)
                {
                    if (!isResolved)
                    {
                        isResolved = true;
                        GameManager.AddScore(penaltyScore); // -100점
                        DisposeTimer();
                    }
                }
            }

            // 제한 시간 내에 줄이 완성되어 블록이 지워질 때 호출
            public override void OnLineCleared()
            {
                lock (lockObj)
                {
                    if (isPlaced && !isResolved)
                    {
                        isResolved = true;
                        DisposeTimer();
                        GameManager.AddScore(bonusScore); // +100점
                    }
                }

                base.OnLineCleared();
            }

            private void DisposeTimer()
            {
                if (riskTimer != null)
                {
                    riskTimer.Stop();
                    riskTimer.Dispose();
                    riskTimer = null;
                }
            }
        }
    }
}
