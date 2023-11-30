// See https://aka.ms/new-console-template for more information

using ConsoleApp3;

class Program
{
    static void Main(string[] args)
    {

        using (GameClass game = new GameClass())
        {
            game.UpdateFrequency = 60.0;
            game.Run();
        }
    }
}

