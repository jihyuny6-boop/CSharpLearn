// 배열

// 왜 쓸까요?

using System;

static class Test
{
    static void Main()
    {
        int[] numbers = new int[10];

        for (int i = 0; i < 10; i++)
        {
            numbers[i] = i;
            Console.WriteLine($"현재 값은 : {numbers[i]}");
        }
        
        //갯수를 정하면 시작부터 끝까지 데이터를 저장하고 언제든지 꺼내서 쓸 수 있다
        const int ITEM_MAX_COUNT = 10;


        int[] prices = new int[ITEM_MAX_COUNT];
       
        for (int i = 0; i < ITEM_MAX_COUNT; i++)
        {
            prices[i] = 10 + 10 * i;
        }

        Console.WriteLine("4번쨰 아이탬의 가격은 얼마인가요?");
        Console.WriteLine($" {prices[3]} 입니다");
    }
}

