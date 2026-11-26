
namespace ConsoleBlockGame
{
    // 점수 관리 매니저
    public static class GameManager
    {
        public static int Score { get; private set; } = 0;

        public static void AddScore(int amount)
        {
            Score += amount;
            Console.WriteLine($"\n[점수 변동] {(amount > 0 ? "+" : "")}{amount}점 (현재 총점: {Score}점)");
        }
    }

    // 부모 클래스: 블록
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
            // 기본 블록의 줄 제거 시 처리 로직 (예: 보드 배열에서 제거)
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

    // 자식 클래스: RiskBlock (일정 시간 내 줄 완성 시 +100점, 실패 시 -100점)
    public class RiskBlock : 블록
    {
        public double timeLimitMs = 10000; // 제한 시간 10초 (10,000ms)
        public int bonusScore = 100;
        public int penaltyScore = -100;

        private bool isResolved = false;
        private Timer riskTimer;
        private readonly object lockObj = new object();

        public RiskBlock()
        {
            // 일반 블록과 구분되도록 빨간색으로 설정
            Color = ConsoleColor.Red;
        }

        public override void OnBlockPlaced()
        {
            // 이미 고정된 상태라면 타이머 중복 실행 방지
            if (IsFixed) return;

            base.OnBlockPlaced(); // IsFixed = true 처리

            riskTimer = new Timer(timeLimitMs);
            riskTimer.Elapsed += OnTimeExpired;
            riskTimer.AutoReset = false; // 1회만 실행
            riskTimer.Start();
        }

        private void OnTimeExpired(object sender, ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                if (!isResolved)
                {
                    isResolved = true;
                    GameManager.AddScore(penaltyScore); // -100점
                    Color = ConsoleColor.DarkGray;      // 시간 초과 시 어두운 회색으로 변경
                    DisposeTimer();
                }
            }
        }

        public override void OnLineCleared()
        {
            lock (lockObj)
            {
                if (IsFixed && !isResolved)
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

    // 실행 및 동작 테스트용 메인 클래스
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== RiskBlock 콘솔 테스트 ===");
            RiskBlock riskBlock = new RiskBlock { X = 5, Y = 10 };

            Console.WriteLine("1. 블록이 바닥에 고정됩니다. (10초 카운트다운 시작)");
            riskBlock.OnBlockPlaced();

            Console.WriteLine("10초 안에 Enter 키를 누르면 줄 완성(+100점), 기다리면 시간 초과(-100점)가 발생합니다.");
            Console.ReadLine();

            // Enter 입력 시 줄 완성 처리 호출
            riskBlock.OnLineCleared();

            Console.WriteLine("테스트 종료. 아무 키나 누르면 창이 닫힙니다.");
            Console.ReadKey();
        }
    }
}
