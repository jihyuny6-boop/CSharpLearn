using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//반드시 내가 주석으로 지정한 표현 방식쓰고 너가 추가적으로 기능을 구현하지 말아라
// 가위 바위 보를 조건문으로 만들떄 조건을 내가 만든다.
//만약에 그상태에서 실행이 안된다면 안되는 이유를 나에게 말해라 C#으로

// 나 : 가위 남 : 바위 조건으로 표현해서 이긴사람은 점수를 1점 얻는다.
// 3x3 코드들




namespace bacis
{
    internal class 데이터의_흐름
    {
        static void Main()
        {
            // if ,eles if , if 
            int Score = 90;
            bool GameFinish = true;

            if (GameFinish)
            {
                // 60점 이상 80점 미만
                if(60 <= Score && 80 > Score)
                {
                    Console.WriteLine('B');
                }
                //80점 이상 100점 미만
                else if (80 <= Score && 100 > Score)
                {
                    Console.WriteLine('A');
                }
                else
                {
                    Console.WriteLine("미정의");
                }
            }


            string name = "파이리";

            // switch 스위치
            switch(name)
            {
                case "파이리":
                    Console.WriteLine("파이리");
                    break;
                case "거북이":
                    Console.WriteLine("거북이");
            }

        }
    }
}
