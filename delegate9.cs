using System;

namespace hhehe
{
    class Program
    {
        static void Hi() => Console.WriteLine("hi");
        delegate void SayDelegate();
        //딜리게이트 --> 메서드 참조
        // 딜리게이트를 생성하고 , 그 딜리게이트를 담는 변수에 원하는 함수를 넣으면
        // 다른 맥락에서도 사용이 가능 

        static void Main(string[] args)
        {

            SayDelegate say = Hi;
            say();
            run();
            hell();
        }
        static void run()
        {

            SayDelegate say = Hi;
            say();
        }
        static void hell()
        {
            SayDelegate say = Hi;
            say();
        }
    }
}
