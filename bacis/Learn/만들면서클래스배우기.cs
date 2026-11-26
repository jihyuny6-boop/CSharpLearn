
// 추상 클래스 공통적인 기능들을 모두 뺴낸다

// I미노 is 블록 -> 상속
//
블록 I = new 블록("I");

class 블록
{
    public string shape = "■";

    public 블록(string shape)
    {
        this.shape = shape;
    }
}


// 블록의 모든 성질을 가지면서 새로운 도특한 기능을 한번 추가하고 싶다
// 시간 제한이 있어서 일정시간 후에 파괴된다 위에 있는 블록은 자동으로 파괴된다

class TiemrBlock : 블록
{
    public int deathTimer = 3;

    public TiemrBlock(string shape) : base(shape)
    {
    }

    public void IimeDeath()
    {
        //몇초뒤에 블록이 사라질지 있어야한다
    }
}

// 일정시간안에 블록이 있는 줄을 완성시키지 못하면 -100점 완성시키면 추가 점수 100점

class RiskBlock : 블록
