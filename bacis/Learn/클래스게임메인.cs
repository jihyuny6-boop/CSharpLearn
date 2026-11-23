/*
 * 클래스의 생성자까지 사용 문법을 실습한다
 *   -게임 폴더
 *      -Program.cs
 *      -Unit.cs
 * Ai코드를 하나의 Main 작성을 하면 가독성이 매우 떨어집니다. Ai가 무엇을 만들었는지 1도 알아볼수없다.
 *  클래스를 직접 설계하신다음에
 *  그 클래스로 while반복문 압력,조직,rendring만들어줘.
 */






using System;


class Program
{
    static void Main()
    {
        Unit player = new Unit("용사", 100, 20, 5, 10);
        Unit enemy = new Unit("슬라임", 80, 15, 3, 8);

        bool isRunning = true;
        string battleLog = "전투가 시작되었습니다!";

        // 초기 화면 렌더링
        Render(player, enemy, battleLog);

        // 게임 루프 (입력 -> 업데이트 -> 렌더링)
        while (isRunning)
        {
            // 1. 입력 (Input)
            Console.Write("\n명령을 입력하세요 (1: 공격, Q: 종료) > ");
            string input = Console.ReadLine()?.Trim().ToUpper();

            // 2. 업데이트 (Update)
            if (input == "Q")
            {
                battleLog = "게임을 종료합니다.";
                isRunning = false;
            }
            else if (input == "1")
            {
                battleLog = player.BattleTurn(enemy);

                if (!player.IsAlive || !enemy.IsAlive)
                {
                    string winner = player.IsAlive ? player.Name : enemy.Name;
                    battleLog += $"\n=== 전투 종료! 승자: {winner} ===";
                    isRunning = false;
                }
            }
            else
            {
                battleLog = "잘못된 입력입니다. 1 또는 Q를 입력하세요.";
            }

            // 3. 렌더링 (Render)
            Render(player, enemy, battleLog);
        }
    }

    static void Render(Unit player, Unit enemy, string log)
    {
        Console.Clear();
        Console.WriteLine("==================================");
        Console.WriteLine($" {player.Name} | HP: {player.Hp}/{player.MaxHp} | 공:{player.Attack} 방:{player.Defense} 속:{player.Speed}");
        Console.WriteLine($" {enemy.Name} | HP: {enemy.Hp}/{enemy.MaxHp} | 공:{enemy.Attack} 방:{enemy.Defense} 속:{enemy.Speed}");
        Console.WriteLine("==================================");
        Console.WriteLine(log);
    }
}
