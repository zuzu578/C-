using System;
using InterEx;
using interfaceDemo;
namespace newEx01

{
    class Program
    {
        static void Main(string[] args)
        {
            var good = new Car(new Good());good.Run();
            new Car(new Bad()).Run();

            //1) var 
            var dog = new Dog();
            dog.Eat();
            dog.Yelp();

            //2) Class
            Dog dog1 = new Dog();
            dog1.Yelp();
            dog1.Eat();

        }
    }
}
namespace InterEx
{
    interface IAnimal
    {
        
        void Eat();
    }
    interface IDog
    {
        void Yelp();
    }
    class Dog : IAnimal, IDog
    {
        public void Eat() => Console.WriteLine("강아지가 먹는다 ");
        public void Yelp() => Console.WriteLine("강아지가 짖다");
    }



}








namespace interfaceDemo
{

    interface IBattery
    {
        string GetName();
    }
    class Good : IBattery
    {
        public string GetName() => "Good";
    }
    class Bad : IBattery
    {

        public string GetName() => "bad";
    }
    class Car
    {
        private IBattery _battery;
        public Car(IBattery battery)
        {
            _battery = battery;
        }
        public void Run() => Console.WriteLine("{0} 배터리를 장착한 자동차가 달린다.", _battery.GetName());
    }

}
