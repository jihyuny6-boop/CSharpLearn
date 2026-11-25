using System;
using System.Collections.Generic;
using System.Threading;

namespace StarCraftOOP
{
    public enum MovementType { Ground, Air }
    public enum UnitAttribute { Biological, Mechanical }

    // 1. 특수 능력 인터페이스 정의
    public interface IClockable
    {
        bool IsCloaked { get; }
        void ToggleCloak();
    }

    public interface IDetector
    {
        void Detect();
    }

    public interface ITransport
    {
        int Capacity { get; }
        void Load(Unit target);
        void UnloadAll();
    }

    public interface IWorker
    {
        void Gather();
        void Build(string buildingName);
    }

    public interface IBurrowable
    {
        bool IsBurrowed { get; }
        void ToggleBurrow();
    }

    // 2. 최상위 추상 클래스: Unit
    public abstract class Unit
    {
        public string Name { get; protected set; }
        public int MaxHp { get; protected set; }
        public int Hp { get; protected set; }
        public MovementType Movement { get; protected set; }
        public UnitAttribute Attribute { get; protected set; }
        public string StatusMessage { get; protected set; } = "대기 중";

        protected Unit(string name, int maxHp, MovementType movement, UnitAttribute attribute)
        {
            Name = name;
            MaxHp = maxHp;
            Hp = maxHp;
            Movement = movement;
            Attribute = attribute;
        }

        public virtual void Move(string destination)
        {
            if (this is IBurrowable burrowable && burrowable.IsBurrowed)
            {
                StatusMessage = "잠복 상태라 이동 불가";
                return;
            }
            string moveMode = Movement == MovementType.Ground ? "지상 이동" : "공중 비행";
            StatusMessage = $"{destination}(으)로 {moveMode}";
        }

        public virtual void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - damage);
            StatusMessage = $"피해 {damage} 입음 (남은 HP: {Hp})";
        }

        // 게임 루프 - 갱신(Update) 단계에서 호출될 추상 메서드
        public abstract void Update();

        // 게임 루프 - 렌더링(Render) 단계에서 호출될 가상 메서드
        public virtual void Render()
        {
            Console.WriteLine($"[{GetType().Name}] {Name} | HP: {Hp}/{MaxHp} | {Movement}/{Attribute} | 상태: {StatusMessage}");
        }
    }

    // 3. 종족별 추상 클래스
    public abstract class TerranUnit : Unit
    {
        protected TerranUnit(string name, int maxHp, MovementType movement, UnitAttribute attribute)
            : base(name, maxHp, movement, attribute) { }

        public override void Update()
        {
            if (Attribute == UnitAttribute.Mechanical && Hp < MaxHp * 0.4)
            {
                StatusMessage = "위험! 화염 발생 (SCV 수리 필요)";
            }
        }
    }

    public abstract class ZergUnit : Unit
    {
        private readonly int _regenRate;

        protected ZergUnit(string name, int maxHp, MovementType movement, UnitAttribute attribute, int regenRate = 2)
            : base(name, maxHp, movement, attribute)
        {
            _regenRate = regenRate;
        }

        public override void Update()
        {
            if (Hp > 0 && Hp < MaxHp)
            {
                Hp = Math.Min(MaxHp, Hp + _regenRate);
                StatusMessage = $"체력 재생 (+{_regenRate})";
            }
        }
    }

    public abstract class ProtossUnit : Unit
    {
        public int MaxShield { get; protected set; }
        public int Shield { get; protected set; }
        private readonly int _shieldRegenRate;

        protected ProtossUnit(string name, int maxHp, int maxShield, MovementType movement, UnitAttribute attribute, int shieldRegenRate = 3)
            : base(name, maxHp, movement, attribute)
        {
            MaxShield = maxShield;
            Shield = maxShield;
            _shieldRegenRate = shieldRegenRate;
        }

        public override void TakeDamage(int damage)
        {
            if (Shield >= damage)
            {
                Shield -= damage;
                StatusMessage = $"실드 피격 -{damage}";
            }
            else
            {
                int remaining = damage - Shield;
                Shield = 0;
                base.TakeDamage(remaining);
            }
        }

        public override void Update()
        {
            if (Shield < MaxShield)
            {
                Shield = Math.Min(MaxShield, Shield + _shieldRegenRate);
                StatusMessage = $"실드 재생 (+{_shieldRegenRate})";
            }
        }

        public override void Render()
        {
            Console.WriteLine($"[{GetType().Name}] {Name} | HP: {Hp}/{MaxHp}, Shield: {Shield}/{MaxShield} | {Movement}/{Attribute} | 상태: {StatusMessage}");
        }
    }

    // 4. 구체적인 유닛 클래스 구현
    public class SCV : TerranUnit, IWorker
    {
        public SCV() : base("SCV", 60, MovementType.Ground, UnitAttribute.Mechanical) { }

        public void Gather() => StatusMessage = "미네랄/가스 채취 중";
        public void Build(string buildingName) => StatusMessage = $"{buildingName} 건설 중";
    }

    public class Wraith : TerranUnit, IClockable
    {
        public bool IsCloaked { get; private set; }
        public Wraith() : base("레이스", 120, MovementType.Air, UnitAttribute.Mechanical) { }

        public void ToggleCloak()
        {
            IsCloaked = !IsCloaked;
            StatusMessage = IsCloaked ? "클로킹 활성화" : "클로킹 해제";
        }
    }

    public class Dropship : TerranUnit, ITransport
    {
        private readonly List<Unit> _cargo = new List<Unit>();
        public int Capacity => 8;
        public Dropship() : base("드랍쉽", 150, MovementType.Air, UnitAttribute.Mechanical) { }

        public void Load(Unit target)
        {
            if (target.Movement == MovementType.Ground && _cargo.Count < Capacity)
            {
                _cargo.Add(target);
                StatusMessage = $"{target.Name} 탑승 ({_cargo.Count}/{Capacity})";
            }
        }

        public void UnloadAll()
        {
            _cargo.Clear();
            StatusMessage = "모든 유닛 하차 완료";
        }
    }

    public class Zergling : ZergUnit, IBurrowable
    {
        public bool IsBurrowed { get; private set; }
        public Zergling() : base("저글링", 35, MovementType.Ground, UnitAttribute.Biological, 2) { }

        public void ToggleBurrow()
        {
            IsBurrowed = !IsBurrowed;
            StatusMessage = IsBurrowed ? "잠복(Burrow) 완료" : "잠복 해제";
        }
    }

    public class Overlord : ZergUnit, IDetector, ITransport
    {
        private readonly List<Unit> _cargo = new List<Unit>();
        public int Capacity => 8;
        public Overlord() : base("오버로드", 200, MovementType.Air, UnitAttribute.Biological, 3) { }

        public void Detect() => StatusMessage = "은폐 유닛 탐지 중";

        public void Load(Unit target)
        {
            if (target.Movement == MovementType.Ground && _cargo.Count < Capacity)
            {
                _cargo.Add(target);
                StatusMessage = $"{target.Name} 탑승 ({_cargo.Count}/{Capacity})";
            }
        }

        public void UnloadAll()
        {
            _cargo.Clear();
            StatusMessage = "모든 유닛 하차 완료";
        }
    }

    public class Zealot : ProtossUnit
    {
        public Zealot() : base("질럿", 100, 60, MovementType.Ground, UnitAttribute.Biological, 4) { }
    }

    public class Observer : ProtossUnit, IClockable, IDetector
    {
        public bool IsCloaked { get; private set; } = true;
        public Observer() : base("옵저버", 40, 20, MovementType.Air, UnitAttribute.Mechanical, 2) { }

        public void ToggleCloak() => StatusMessage = "영구 은폐 상태 유지";
        public void Detect() => StatusMessage = "은폐/잠복 유닛 탐지 가동";
    }

    // 5. 메인 실행 클래스 (입력 -> 갱신 -> 렌더링 게임 루프 구조)
    public class Program
    {
        private static List<Unit> _units = new List<Unit>();
        private static bool _isRunning = true;
        private static int _tickCount = 0;

        public static void Main(string[] args)
        {
            Initialize();

            // 최초 초기 상태 렌더링
            Render();

            while (_isRunning)
            {
                ProcessInput();
                if (!_isRunning) break;

                Update();
                Render();
            }
        }

        // 초기화
        private static void Initialize()
        {
            _units = new List<Unit>
            {
                new SCV(),
                new Wraith(),
                new Dropship(),
                new Zergling(),
                new Overlord(),
                new Zealot(),
                new Observer()
            };
        }

        // 1단계: 입력 (Input)
        private static void ProcessInput()
        {
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("[명령어 입력] 1: 전체 이동 | 2: 전체 피격(-25) | 3: 특수능력 발동 | 4: 대기(턴 진행) | 0: 종료");
            Console.Write(">> 선택: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    foreach (var unit in _units)
                        unit.Move("중앙 격전지");
                    break;

                case "2":
                    foreach (var unit in _units)
                        unit.TakeDamage(25);
                    break;

                case "3":
                    ExecuteSpecialAbilities();
                    break;

                case "4":
                    // 아무 명령 없이 시간 경과(재생 효과 확인용)
                    break;

                case "0":
                    _isRunning = false;
                    Console.WriteLine("시뮬레이션을 종료합니다.");
                    break;

                default:
                    Console.WriteLine("잘못된 입력입니다. 턴만 진행됩니다.");
                    break;
            }
        }

        // 다형성을 활용한 인터페이스별 특수 능력 일괄 처리
        private static void ExecuteSpecialAbilities()
        {
            Unit groundTarget = _units.Find(u => u.Movement == MovementType.Ground);

            foreach (var unit in _units)
            {
                if (unit is IWorker worker)
                    worker.Gather();

                if (unit is IClockable clockable)
                    clockable.ToggleCloak();

                if (unit is IBurrowable burrowable)
                    burrowable.ToggleBurrow();

                if (unit is IDetector detector)
                    detector.Detect();

                if (unit is ITransport transport && groundTarget != null)
                    transport.Load(groundTarget);
            }
        }

        // 2단계: 갱신 (Update)
        private static void Update()
        {
            _tickCount++;
            foreach (var unit in _units)
            {
                unit.Update(); // 저그 체력 재생, 프로토스 실드 재생, 테란 상태 체크
            }
        }

        // 3단계: 렌더링 (Render)
        private static void Render()
        {
            Console.Clear();
            Console.WriteLine($"=== StarCraft OOP 시뮬레이터 (Turn: {_tickCount}) ===");
            Console.WriteLine("--------------------------------------------------------------------------");
            foreach (var unit in _units)
            {
                unit.Render();
            }
            Console.WriteLine("--------------------------------------------------------------------------");
        }
    }
}
