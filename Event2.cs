using System;

namespace EventEx
{
    public class ButtonClass
    {
        //delegate
        public delegate void EventHandler();
        //event
        public event EventHandler Click;
        //event handler
        public void OnClick()
        {
            if(Click!= null)
            {
                Click();
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            ButtonClass btn = new ButtonClass();
            btn.Click += Hi1;
            btn.Click += Hi2;
            btn.OnClick();
        }
        static void Hi1() => Console.WriteLine("Hi1");
        static void Hi2() => Console.WriteLine("Hi1");


    }
}
