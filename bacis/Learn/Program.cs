using System;
using System.Threading;

// 게임 실행 및 3요소(입력, 업데이트, 렌더링) 루프 제어
Console.CursorVisible = false; //커서를 안보이게 한다.

GameManager.Initialize(width: 40, height: 15);
GameManager.StartNextWave();

while (GameManager.IsRunning)
{
    GameManager.HandleInput();   // 1. 입력 (Input)
    GameManager.Update();        // 2. 업데이트 (Update)
    GameManager.Render();        // 3. 렌더링 (Render)

    Thread.Sleep(50);            // 프레임 속도 조절 (약 20 FPS)
}

Console.Clear();
Console.CursorVisible = true;
Console.WriteLine($"게임 종료! 최종 도달 Wave: {GameManager.CurrentWave}");
