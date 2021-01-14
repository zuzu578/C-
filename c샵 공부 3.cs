using System;
using ExPublic;
using ExConstructor;
namespace ex002
{
    class Program
    {
        static void Main(string[] args)
        {
            //* namespace 에 선언된 클래스는 protected X , protected internal X
            //public , internal 만 사용가능 
            //public ==> 모든곳에서 접근이 가능하다.
            //private ==> 현재 클래스내에서만 접근이 가능하다.
            //protected ==>현재클래스 , 상속하는 하위 클래스에서만 접근이 가능하다.
            //internal ==> 현재 프로젝트의 모든클래스에 접근이 허가된다.
            //protected internal ==> 어셈블리에서 파생된 모든 클래스에 액세스가 허가된다(같은 프로그램에서 접근이 가능)
            Category c1 = new Category();
            c1.CategoryName = "책";
            Console.WriteLine(c1.CategoryName);
            ExCon e1 = new ExCon(20,40);
            e1.Running();
            A a1 = new A();
            a1.Running();
            F f1 = new F();
            //set
            f1.Name = "Lee";
            //get
            Console.WriteLine(f1.Name);


            //접근자
            Person p1 = new Person();
            p1.Name = "Lee";
            Console.WriteLine(p1.Name);

        }
    }
}
class Person
{
    private String name;
    public String Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }


    }
}

    //접근자와 전체속성
    class Car
{
    private String Name;
    public String Name
    {
        get
        {
            return name;

        }
        set
        {
            name = value;
        }
    


    }



}

//속성을 이용한 get ,set

class F
{
   
    public String Name { get; set; }



}

//화살표 함수 식 본문 생성자
class Pet
{
    private String _name;

    public Pet(String name) => _name = name;
    public override string ToString()
    {
        return _name;
    }

}
class A
{
    //readonly ==> field initialize (X)
    //==> constructor initialize(O)
    private int x;
    public A() => x = 20;
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public void Running() => Console.WriteLine(x + 40);
   


}
namespace ExPublic
{

    internal class Category
    {
        internal String CategoryName;
    }

}
namespace ExConstructor
{
    internal class ExCon
    {
        int k;
        int z;
        //default 생성자 
       
        //default 가 먼저 실행 된다 
        // parameter 가있는 생성자 
        public ExCon(int x , int y)
        {
            this.k = x;
            this.z = y;

        }
        public void Running()
        {
            Console.WriteLine(this.k + this.z);
            Console.WriteLine($"firstNum:{this.k},SecondNum:{this.z}");
        }
    }
}

 class Say
{
    
    private string message = "[1]안녕하세여";
    public Say() => Console.WriteLine(this.message);
    //this()생성자로 자신의 매개변수가 없는 생성자를 먼저 호출 
    public Say(string message) : this()
    {
        this.message = message;
        Console.WriteLine(this.message);
    }
}





public class Car
{
    //private memeber ==>다른 클래스에서는 접근 x
    //public member ==>다른클래스에 접근 O
    public static void Hi()
    {
        Console.WriteLine("hi");
           
    }
    private static void Bye()
    {
        Console.WriteLine("bye");
    }
    public static string name;
    private static int age;
    public static void SetAge(int intAge)
    {
        age = intAge;
    }
    public static int GetAge()
    {
        return age;
    }
}