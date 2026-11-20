using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bacis
{
        // 1. 아이템 종류 (열거형)
        enum ItemType
        {
            Potion, // 체력 물약
            Sword,  // 강철 검
            Shield, // 나무 방패
            Boots   // 가죽 부츠
        }

        class Program
        {
            static void Main(string[] args)
            {
                // 2. 데이터 초기화 (배열 활용)
                // 열거형(ItemType)의 개수만큼 배열 크기 설정
                int itemCount = Enum.GetValues(typeof(ItemType)).Length;

                string[] itemNames = { "체력 물약", "강철 검", "나무 방패", "가죽 부츠" };
                int[] itemPrices = { 50, 300, 150, 100 };     // 아이템 가격
                int[] shopStock = { 5, 2, 3, 4 };             // 상점 재고 (갯수)
                int[] playerInventory = new int[itemCount];   // 플레이어 보유 수량

                int playerGold = 1000;                        // 플레이어 소지금
                string systemMessage = "상점에 오신 것을 환영합니다!";
                bool isRunning = true;

                // 첫 화면 렌더링
                Render(itemNames, itemPrices, shopStock, playerInventory, playerGold, systemMessage);

                // 게임 루프: 입력(Input) -> 업데이트(Update) -> 렌더링(Render)
                while (isRunning)
                {
                    // ==========================================
                    // [1. 입력 (Input)]
                    // ==========================================
                    Console.Write("\n구매할 아이템 번호 입력 (1~4, 종료는 0): ");
                    string input = Console.ReadLine();

                    // ==========================================
                    // [2. 업데이트 (Update)]
                    // ==========================================
                    // 숫자로 변환 가능한지 확인 (조건문)
                    if (int.TryParse(input, out int choice))
                    {
                        if (choice == 0)
                        {
                            isRunning = false;
                            systemMessage = "상점을 이용해주셔서 감사합니다.";
                        }
                        else if (choice >= 1 && choice <= itemCount)
                        {
                            int index = choice - 1; // 배열 인덱스는 0부터 시작
                            ItemType selectedItem = (ItemType)index; // 열거형으로 변환하여 활용 가능

                            // 구매 조건 검사 (재고 및 골드 확인)
                            if (shopStock[index] <= 0)
                            {
                                systemMessage = $"[실패] '{itemNames[index]}'의 재고가 없습니다.";
                            }
                            else if (playerGold < itemPrices[index])
                            {
                                systemMessage = $"[실패] 골드가 부족합니다. (필요: {itemPrices[index]}G)";
                            }
                            else
                            {
                                // 데이터 변경 (구매 처리)
                                playerGold -= itemPrices[index];
                                shopStock[index]--;
                                playerInventory[index]++;
                                systemMessage = $"[성공] '{itemNames[index]}' ({selectedItem}) 1개를 구매했습니다!";
                            }
                        }
                        else
                        {
                            systemMessage = "[오류] 메뉴에 없는 번호입니다. 1~4 사이의 숫자를 입력하세요.";
                        }
                    }
                    else
                    {
                        systemMessage = "[오류] 올바른 숫자를 입력해주세요.";
                    }

                    // ==========================================
                    // [3. 렌더링 (Render)]
                    // ==========================================
                    Render(itemNames, itemPrices, shopStock, playerInventory, playerGold, systemMessage);
                }
            }

            // 화면 출력을 전담하는 메서드
            static void Render(string[] names, int[] prices, int[] stock, int[] inventory, int gold, string message)
            {
                Console.Clear(); // 이전 프레임 화면 지우기

                Console.WriteLine("=================================================");
                Console.WriteLine("                 [ 무기 및 잡화 상점 ]             ");
                Console.WriteLine("=================================================");
                Console.WriteLine($" 현재 보유 골드: {gold} G");
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine(" 번호 | 아이템명   |   가격   | 상점재고 | 내 가방");
                Console.WriteLine("-------------------------------------------------");

                // 반복문을 통한 상점 목록 출력
                for (int i = 0; i < names.Length; i++)
                {
                    // 재고가 0이면 '품절'로 표시 (조건문)
                    string stockText = stock[i] > 0 ? $"{stock[i]}개" : "품절";

                    Console.WriteLine($"  {i + 1}   | {names[i],-8} | {prices[i],6} G | {stockText,6} |  {inventory[i]}개");
                }

                Console.WriteLine("=================================================");
                Console.WriteLine($" 알림: {message}");
                Console.WriteLine("=================================================");
            }
        }
    }

    //아이탬은 어떤 종류가 있을 가요? 열거형
    //갯수는 몇개가 있을 가요?
    //이를 콘솔에서 어떻게 보여지게 할까요 

    //조건문 반복문 배열 열거형

    //입력 - 업데이트 - 렌더링 방식으로 상점 기능이 실행 디게 만들어주세요

    //어떤 입력을 해서 무엇을 변경해야하고 그것을 어떻게 보여줘야한다.


