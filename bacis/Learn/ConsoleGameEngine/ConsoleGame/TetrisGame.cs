


// ai 임의로 구성한 내용을 파악하고 이 부분을 ai로 만들어서 만들어지는 결과를 일정한 수준으로 맞추게 유도하는 작업

// '상속'
using System.Text;

namespace ConsoleTetris
{
    class TetrisGame : BaseGame
    {
        private const int Width = 10;
        private const int Height = 20;
        private int[,] board = new int[Height, Width];

        // 7가지 테트로미노 블록 정의 (I, O, T, S, Z, J, L)
        private readonly int[][,] shapes = new int[][,]
        {
            new int[,] { {1, 1, 1, 1} },                         // I
            new int[,] { {1, 1}, {1, 1} },                       // O
            new int[,] { {0, 1, 0}, {1, 1, 1} },                 // T
            new int[,] { {0, 1, 1}, {1, 1, 0} },                 // S
            new int[,] { {1, 1, 0}, {0, 1, 1} },                 // Z
            new int[,] { {1, 0, 0}, {1, 1, 1} },                 // J
            new int[,] { {0, 0, 1}, {1, 1, 1} }                  // L
        };

        private int[,] currentPiece;
        private int pieceX, pieceY;
        private int score = 0;
        private bool isGameOver = false;

        // 중력(자동 낙하) 타이머용 변수
        private int dropCounter = 0;
        private int dropInterval = 10; // 10프레임(약 0.5초)마다 1칸 낙하
        private Random rand = new Random();

        public TetrisGame()
        {
            SpawnPiece();
        }

        private void SpawnPiece()
        {
            currentPiece = (int[,])shapes[rand.Next(shapes.Length)].Clone();
            pieceX = Width / 2 - currentPiece.GetLength(1) / 2;
            pieceY = 0;

            // 생성되자마자 충돌하면 게임 오버
            if (!IsValidPosition(currentPiece, pieceX, pieceY))
            {
                isGameOver = true;
            }
        }

        public override void Input()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKey key = Console.ReadKey(true).Key;

            if (isGameOver)
            {
                if (key == ConsoleKey.R) ResetGame();
                else if (key == ConsoleKey.Escape) Environment.Exit(0);
                return;
            }

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (IsValidPosition(currentPiece, pieceX - 1, pieceY)) pieceX--;
                    break;
                case ConsoleKey.RightArrow:
                    if (IsValidPosition(currentPiece, pieceX + 1, pieceY)) pieceX++;
                    break;
                case ConsoleKey.DownArrow:
                    if (IsValidPosition(currentPiece, pieceX, pieceY + 1)) pieceY++;
                    break;
                case ConsoleKey.UpArrow: // 회전
                    int[,] rotated = RotateMatrix(currentPiece);
                    if (IsValidPosition(rotated, pieceX, pieceY)) currentPiece = rotated;
                    break;
                case ConsoleKey.Spacebar: // 하드 드롭 (바닥까지 즉시 낙하)
                    while (IsValidPosition(currentPiece, pieceX, pieceY + 1)) pieceY++;
                    dropCounter = dropInterval; // 즉시 고정되도록 유도
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
        }

        public override void Update()
        {
            if (isGameOver) return;

            dropCounter++;
            if (dropCounter >= dropInterval)
            {
                dropCounter = 0;
                if (IsValidPosition(currentPiece, pieceX, pieceY + 1))
                {
                    pieceY++;
                }
                else
                {
                    LockPiece();
                    ClearLines();
                    SpawnPiece();
                }
            }
        }

        public override void Rendering()
        {
            // 깜빡임 방지를 위해 Clear() 대신 커서를 좌상단으로 이동
            Console.SetCursorPosition(0, 0);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"=== C# TETRIS ===  Score: {score}   ");
            sb.AppendLine("조작: 방향키(이동/회전), Space(즉시낙하), ESC(종료)");
            sb.AppendLine("+" + new string('-', Width * 2) + "+");

            for (int y = 0; y < Height; y++)
            {
                sb.Append("|");
                for (int x = 0; x < Width; x++)
                {
                    bool isCurrentPieceCell = false;

                    // 현재 떨어지고 있는 블록 영역인지 확인
                    int py = y - pieceY;
                    int px = x - pieceX;
                    if (!isGameOver &&
                        py >= 0 && py < currentPiece.GetLength(0) &&
                        px >= 0 && px < currentPiece.GetLength(1) &&
                        currentPiece[py, px] == 1)
                    {
                        isCurrentPieceCell = true;
                    }

                    if (isCurrentPieceCell)
                        sb.Append("▣"); // 낙하 중인 블록
                    else if (board[y, x] == 1)
                        sb.Append("■"); // 바닥에 고정된 블록
                    else
                        sb.Append(" ."); // 빈 공간
                }
                sb.AppendLine("|");
            }

            sb.AppendLine("+" + new string('-', Width * 2) + "+");

            if (isGameOver)
            {
                sb.AppendLine("   [ GAME OVER! ] 'R'키를 누르면 재시작합니다.   ");
            }
            else
            {
                sb.AppendLine("                                                  ");
            }

            Console.Write(sb.ToString());
        }

        // 충돌 및 경계 검사
        private bool IsValidPosition(int[,] piece, int targetX, int targetY)
        {
            int rows = piece.GetLength(0);
            int cols = piece.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (piece[y, x] == 0) continue;

                    int boardX = targetX + x;
                    int boardY = targetY + y;

                    // 좌우 및 바닥 경계 초과
                    if (boardX < 0 || boardX >= Width || boardY >= Height)
                        return false;

                    // 이미 쌓인 블록과 충돌
                    if (boardY >= 0 && board[boardY, boardX] == 1)
                        return false;
                }
            }
            return true;
        }

        // 시계 방향 90도 행렬 회전
        private int[,] RotateMatrix(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    rotated[x, rows - 1 - y] = matrix[y, x];
                }
            }
            return rotated;
        }

        // 블록을 보드에 고정
        private void LockPiece()
        {
            int rows = currentPiece.GetLength(0);
            int cols = currentPiece.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (currentPiece[y, x] == 1)
                    {
                        board[pieceY + y, pieceX + x] = 1;
                    }
                }
            }
        }

        // 꽉 찬 줄 삭제 및 위쪽 블록 내리기
        private void ClearLines()
        {
            int linesCleared = 0;

            for (int y = Height - 1; y >= 0; y--)
            {
                bool isFull = true;
                for (int x = 0; x < Width; x++)
                {
                    if (board[y, x] == 0)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                {
                    linesCleared++;
                    // 해당 줄 위의 모든 줄을 한 칸씩 아래로 복사
                    for (int moveY = y; moveY > 0; moveY--)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            board[moveY, x] = board[moveY - 1, x];
                        }
                    }
                    // 맨 윗줄 비우기
                    for (int x = 0; x < Width; x++) board[0, x] = 0;

                    y++; // 줄이 내려왔으므로 현재 y 위치 다시 검사
                }
            }

            if (linesCleared > 0)
            {
                score += linesCleared * 100;
                // 점수에 따라 낙하 속도 약간씩 증가 (최소 2프레임까지)
                dropInterval = Math.Max(2, 10 - (score / 500));
            }
        }

        private void ResetGame()
        {
            Array.Clear(board, 0, board.Length);
            score = 0;
            dropInterval = 10;
            isGameOver = false;
            SpawnPiece();
        }
    }
}
