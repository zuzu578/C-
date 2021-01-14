using System;
using System.Collections.Generic;

//namespace ==> java 에서의 package
//클래스 , 메서드의 이름이 같아도 서로다른 namespace에 소속되어있으면 다른 클래스이다 .

namespace new02
{
    public class C
    {
        public void Go()
        {
            Console.WriteLine("new02's go");

        }
    }
    namespace new03
    {
        public class C
        {
            public void Go()
            {
                Console.WriteLine("new03's go");
            }
        }
    }
    namespace MyPractice
    {
        class Program
        {
            static void Main(string[] args)
            {
             new02.C c1 = new new02.C ();
                c1.Go();
                new03.C c2 = new new03.C();
                c2.Go();
            }
        }
    }
}












