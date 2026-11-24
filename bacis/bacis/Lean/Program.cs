Enemy e1 = new Enemy("고블린");
Enemy e2 = new Enemy("고블린");
Enemy e3 = new Enemy("고블린");


// record 특수문법

// static을 쓰면은 클래스 이름으로 바로 쓸 수 있다


Console.WriteLine($"몬스터가 몇마리 소환되었나요? {Enemy.Count} 마리");

class Enemy
{
    public static int Count;    // 객체마다 따로 생긴다

    public string Name;        // 객체마다 하나

    public Enemy(string name)
    {
        Name = name;
        Count++;
    }

        public static void PrintCount()
    {
        Console.WriteLine($"적 {Count}마리");
    }
}
