/*
 * 2026-11-23
 * 개념정리
 * '프로그램의 흐름'
 * 
 * 모든 프로그램은 시작하는 위치가 있다
 *  static void Main()- 메인 함수에서 프로그램을 시작한다
 *  
 */

using System.ComponentModel;

Console.WriteLine("Hello");// 아무 코드가 없는 패이지 2이상 있으면 Main이 두개가 있는거와 같다.
int a = -4;
int b = 8;
Console.WriteLine((float)a / b);

for(int i=0; i <5; i++)
{
    if (i == 0) continue;
    if (i == 3) break;
    Console.WriteLine(i);

}