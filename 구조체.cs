using System;

namespace new02
{
    class Program
    {
        //구조체 사용//
        struct S{
            public int k;
            public int j;
            public int q;

        }
        static void Main(string[] args)
        {

            S s = new S();
            s.k = 20;
            s.j = 40;
            s.q = 50;
            Draw(s); 

            static void Draw(S s)
            {
                Console.WriteLine(s.k + s.j);
            }
            

        }
    }
}
