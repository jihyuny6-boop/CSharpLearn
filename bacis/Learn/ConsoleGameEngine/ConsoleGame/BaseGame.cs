// ai 임의로 구성한 내용을 파악하고 이 부분을 ai로 만들어서 만들어지는 결과를 일정한 수준으로 맞추게 유도하는 작업

// '상속'
namespace ConsoleTetris
{
    abstract class BaseGame
    {
        abstract public void Input();
        abstract public void Update();
        abstract public void Rendering();

        public virtual void Run()
        {
            Console.CursorVisible = false;
            while (true)
            {
                Input();
                Update();
                Rendering();
                Thread.Sleep(50); // 약 20 FPS
            }
        }
    }
}
