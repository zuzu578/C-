using System;
using System.Collections;

class TestPerson
{
    //구조체 (struc)
    struct Point
    {
        public int x;
        public int y;

    }
    static void Main()
    {
        //Struc 사용 
        Point point;
        point.x = 100;
        point.y = 200;
        running(point);
        static void running(Point point)                               
        {
            Console.WriteLine((point.x + point.y)+"struc");
        }

        //컬렉션 프레임워크 ? //
        //list 
        ArrayList a = new ArrayList();
        a.Add(0);
        a.Add(1);
        a.Add(2);
        a.Add(3);
        for(int i = 0; i < a.Count; i++)
        {
            Console.WriteLine(a[i].ToString());
        }
        A a1 = new A();
        a1.Running();
        B b1 = new B();
        b1.Running2(10,30);
        C c1 = new C();
        Console.WriteLine( c1.Running3(50,60));
        Sum(20,30);
        //매개변수 이름을 정해서 호출 , 자리바꾸기 ==> 명명된 매개변수//
        Sum(first: 20, second: 30);
        Sum(second: 20, first: 10);
      

    }
    //메서드 오버라이딩 (연산자 오버라이딩)
    //--> 이름이 같은 메서드가 여러가지일을 수행하는것
    //ex) 생성자오버라이딩 , parameter , Type에 따른 매개변수 유무 함수
    static void myFnc(int x)
    {

    }
    static void myFnc()
    {
        Console.WriteLine("Myfunction");
    }
    static void Sum(int first ,int second)
    {
        Console.WriteLine(first + second);
    }
}

public class A
{
    public A()
    {

    }
    public void Running()
    {
        Console.WriteLine("Im Running");
    }


}
public class B
{
    int k;
    int q;
    public B()
    {
        this.k = 20;
        this.q = 40;

    }
    public void Running2(int x, int y)
    {
        Console.WriteLine(k - x);
        Console.WriteLine(q - y);
    }
  
}
public class C
{
    int q;
    int r;
    public C()
    {
        this.q = 10;
        this.r = 50;
    }
    public int Running3(int x, int y)
    {
        int result = (q - x);
        return result;

    }
}

