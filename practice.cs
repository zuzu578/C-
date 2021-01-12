using System;

namespace new01
{
    class Program
    {
        static void Main(string[] args) {
            A a1 = new A();
            B b1 = new B();
            b1.Running();

         }
        
          
        }
   
}
//클래스 A는 부모 , B는 자식 
class A
{
    public int k;
    public int l;
    
   public A()
    {
        k = 20;
        l = 70;

    }
        
    

}
class B : A
{
   public void Running()
    {
        Console.WriteLine(k + l);
    }
}




