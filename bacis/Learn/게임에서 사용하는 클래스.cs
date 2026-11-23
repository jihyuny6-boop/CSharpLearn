// ai를 활용한 배운 문법을 작성하는 페이지 입니다.

// 클래스,생성자
// 클래스 무엇을 할래?

//rpg 턴제 전투. 포켓몬스터
//'플레이어' '적' (도감에 있는 모든 몬스터)
//Unit - 전투를 한다 플레이어도 하고 똑같은 방식으로 한다.
//클래스 한개만 만들어라
//내가 어떻게 만들건지 문법을 설명할거다

//똑같은 프롬프트를 입력해도 결과가 다르개 나온다

//using System;
//Unit 검사 = new Unit("검사",100,10,5,7);
//Unit 궁수 = new Unit("궁수",80, 30, 0, 10);
//검사.Battle(궁수);
public class Unit
{
    public string Name { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }

    public bool IsAlive => Hp > 0;

    public Unit(string name, int hp, int attack, int defense, int speed)
    {
        Name = name;
        MaxHp = hp;
        Hp = hp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
    }

    // 1턴 전투 실행 및 결과 로그 반환
    public string BattleTurn(Unit target)
    {
        Unit first = this.Speed >= target.Speed ? this : target;
        Unit second = first == this ? target : this;

        string log = "";

        int dmg1 = first.AttackTarget(second);
        log += $"[선공] {first.Name}의 공격! {second.Name}에게 {dmg1} 데미지!\n";

        if (second.IsAlive)
        {
            int dmg2 = second.AttackTarget(first);
            log += $"[후공] {second.Name}의 반격! {first.Name}에게 {dmg2} 데미지!\n";
        }

        return log;
    }

    private int AttackTarget(Unit target)
    {
        int damage = Math.Max(1, this.Attack - target.Defense);
        target.Hp = Math.Max(0, target.Hp - damage);
        return damage;
    }
}
