//클래스 사용하기
Game lol = new Game("롤", 1.01f);   // Game 형태를 저장할수 있고 이름이 lol이다. 공간은 아직 x
                                   // new 타입. 타입 메모리 크기만큼 공간을 만들어준다.
//lol.title = "롤";
//lol.version = 1.01f;
lol.Run();

Game val = new Game("발로란트", 1.21f);
//val.title = "발로란트";
//val.version = 1.21f;
val.Run();

// 클래스 만들기
// 실습 예시. 게임이 버전을 실수형으로 정의를 해서 실행할떄 {title} v{vresion}을 실행합니다
// 생성자.
class Game
{
    public string title;
    //실행한다. 모든 게임이 실행하나요?
    public float version;

    public Game(string title, float version)
    {
        this.title = title;
        this.version = version;
    }

    // 함수 : 가능
    public void Run()
    {
        Console.WriteLine($"{title} 버전:{version} 을 실행합니다.");
    }
}



//클래스 왜 만들어요?
//발로란트를 실행합니다.
//롤을 실행합니다.
//클래스 어떻게 써요?
//