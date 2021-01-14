using System;
using Korea.Seoul;
using In = Korea.Incheon;
using System.Collections.Generic;
using myFieldItems.Items1;
using Example.Ex1;
using System.Collections;


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
namespace myFieldItems
{
    namespace Items1
    {
        public class I
        {
            //field
            //실행문이 오는것을 권장하지 않는다.//

            private int k;
            private int z;
            private String x;
            //필드에 list사용//
            ArrayList a1 ;
            public I()
            {
                //initialize
                this.k = 20;
                this.z = 40;
                this.x = "hello wolrd";

                //arrayList initialize
                a1 = new ArrayList();

                a1.Add(20);
                a1.Add(30);
                a1.Add(40);

             
            }
            public void Running2()
            {
                for(int i = 0; i < a1.Count; i++)
                {
                    Console.WriteLine(a1[i].ToString());
                }
            }
            public void Running()
            {
                //변수와 메모리구조
                //1) 지역변수 , 함수를 선언하면 --> stack 메모리에 누적이됩니다.
                //2) 지역변수 ,함수는 선언되면 stack 에 잡히고 , 선언이 끝나면 stack 프레임에서 해체가 되는것인데
                //3) c#에서는 이를 자동으로 해줘서 이를 GC(Garbage Collector)
                //4) new 연산자를 이용한 객체는 Heap 메모리에 저장이된다 Ex)인스턴스화 한 클래스 객체 , 전역변수
                //5) 그렇기 때문에 this라는 키워드는 메모리 를 가리키는 말이되고
                //6) 객체화된 class 에서의 Class 멤버변수이므로 이들은 heap 에 저장이되는 객체들이기때문에 this이용하면 필드변수를
                //7) 도트연산자를 이용해서 객체에 접근한다는 의미이다.


                //함수안에서는 전역변수보다 지역변수가 우선순위가 된다.

                int k = 20;
                Console.WriteLine(k);

                //반면에 this를 사용하면 객체(메모리에 실제로 저장된 것) 자기자신을 가리키게된다
                Console.WriteLine(this.k);
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


            //ArrayList initialize//
            I n = new I();
            n.Running2();
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













