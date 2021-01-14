using System;
using Korea.Seoul;
using In = Korea.Incheon;
using System.Collections.Generic;

using Example.Ex1;


//namespace ==> java 에서의 package
//클래스 , 메서드의 이름이 같아도 서로다른 namespace에 소속되어있으면 다른 클래스이다 .

namespace Korea
{
    namespace Seoul
    {
        public class Car
        {
            public void Run()
            {
                Console.WriteLine("서울 자동차가 달린다");
            }
        }
    }
    namespace Incheon
    {
        public class Car
        {
            public void Run()
            {
                Console.WriteLine("인천 자동차가 달린다");
            }
        }
    }
}
    namespace MyPractice

    {
        class Program
        {
            static void Main(string[] args)
            {
            //네임 스페이스 전체지정
            Korea.Seoul.Car s = new Korea.Seoul.Car();
            s.Run();
            Korea.Incheon.Car i = new Korea.Incheon.Car();
            i.Run();
            //네임스페이스 선언부에 선언
            //ex)using Korea.Seoul;
            Car seoul = new Car();
            //별칭을 사용하는 방법
            //ex)using In = Korea.Incheon;
            In.Car ic = new In.Car();
            ic.Run();
            Example.Ex1.Ex ex1 = new Example.Ex1.Ex();
            ex1.Running();
            Example.EX2.Ex ex2 = new Example.EX2.Ex();
            ex2.Running();
            //네임스페이스의 클래스를 바로 사용할수있게 하는 방법 
            Ex ex = new Ex();
            ex.Running();
            }
        }
    }

namespace Example
{
    namespace Ex1
    {
       public class Ex
        {
            public void Running()
            {
                Console.WriteLine("Ex1의 console");
            }
        }
    }
    namespace EX2
    {
        public class Ex
        {
            public void Running()
            {
                Console.WriteLine("Ex2의 console");
            }
        }

    }


}













