


// ai 임의로 구성한 내용을 파악하고 이 부분을 ai로 만들어서 만들어지는 결과를 일정한 수준으로 맞추게 유도하는 작업

// '상속'
using System;
using System.Text;
using System.Threading;

namespace ConsoleTetris
{

    class Program
    {
        static void Main(string[] args)
        {
            // 콘솔 출력 인코딩 설정 (특수문자 깨짐 방지)
            Console.OutputEncoding = Encoding.UTF8;

            BaseGame tetris = new TetrisGame();
            tetris.Run();
        }
    }
}
