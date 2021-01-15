using System;
using ConstructorBase;
using ConstructorInheritance;
using InterfaceNote;

namespace hhehe
{
    class Program
    {

        static void Main(string[] args)
        {
            B b1 = new B();
            b1.Sum();


            //upcasting --> 암묵적 형변환
            D d1 = new D();
            d1.print();

            //base( ) 생성자 상속 
            string message = "하위클래스의 생성자를 호출할때 상위 클래스의 생성자로 전달 ";
            Child c1 = new Child(message);
            //c1.Printing();

            Result4 r1 = new ResultNew();
            r1.Result();

            //추상클래스 , 인터페이스 --> 인스턴스화 (X)
            //1) 추상클래스(단 , 추상은 일반 메서드도 생성가능하지만 ) , 인터페이스는 몸통만있는 핵심 로직이없는 코드이기 때문에 인스턴스화가 안된다 
            Car c4 = new Car();
            c4.Go();

            //interface 에서의 Upcasting(인터페이스 형식 객체에 인스턴스 담기)
            IRepositoy i = new Repository();
            i.Get();


        }
        
    }
}
//interface
public interface IRepositoy
{
    void Get();
}
public class Repository : IRepositoy
{
    public void Get() => Console.WriteLine("implements Get( )");
}






namespace InterfaceNote
{

    interface ICar
    {
        void Go();
    }
    class Car : ICar
    {
        public void Go()
        {
            Console.WriteLine("interface implements");
           // throw new NotImplementedException();
        }
    }

}


//추상
abstract class Result4
{
    public abstract void Result();
   

}
class ResultNew : Result4
{
    public override void Result()
    {
        Console.WriteLine("hello world");
        
    }
}






//생성자 상속
namespace ConstructorInheritance
{


    public class P
    {
        public string Word { get; set; }
        public P(string Word)
        {
            this.Word = Word;
        }
    }

   sealed public class C : P
    {
        //public C() : this() { }
        public C(string message) : base(message) { }
        public void Say() => Console.WriteLine(base.Word);
    }

   



}






namespace ConstructorBase
{
    class Parent
    {
        public Parent(string message)
        {
            Console.WriteLine(message);
        }
    }
    class Child : Parent
    {
        //  부모의 생성자를 호출 Base( )
        public Child(string message) : base(message) { }

        
    }
   

}





class P
{
   protected int k;
   protected int z;
    public P()
    {
        this.k = 40;
        this.z = 50;
        
    }
    protected void print()
    {
        Console.WriteLine(this.k + this.z);
    }



}

class D :P
{

    public D()
    {

    }
    public void print()
    {
        Console.WriteLine(this.k + this.z+("자식"));
    }


}
abstract class A
{
    public int k;
    public int v;
    public A()
    {
        this.k = 40;
        this.v = 60;
    }
    public abstract void sum();
    public void Sum()
    {
        Console.WriteLine(this.k + this.v);
    }

}
class B :A
{
    
   public void Sum()
    {
        Console.WriteLine((this.k + this.v)+"하위클래스");
    }
    public override void sum()
    {
        Console.WriteLine("abstact");
        //throw new NotImplementedException();
    }
}