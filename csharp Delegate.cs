using System;
using ExDel;

namespace new03
{
    class Program
    {
        //delegate
        static void Main(string[] args)
        {
            Del d1 = new Del();
            SayDelegate say = d1.Hi;
            say();
        }
    }
}

namespace ExDel
{
    public class Del
    {
        public void Hi() => Console.WriteLine("Hello world");
    }
    //delegate 생성
    delegate void SayDelegate();
    


}