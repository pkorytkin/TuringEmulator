using System;
using System.IO;
using System.Collections.Generic;
namespace TuningEmulator
{
    enum Direction {Stop=0, Left=-1,Right=1}
    class Command
    {
        public string State;
        public string NewState;
        public string Symbol;
        public string NewSymbol;
        public Direction Dir;
        public Command(string State,string NewState,string Symbol,string NewSymbol,Direction Dir)
        {
            this.State = State;
            this.NewState = NewState;
            this.Symbol = Symbol;
            this.NewSymbol = NewSymbol;
            this.Dir = Dir;
        }
        public void Print()
        {
            string str = State+Symbol + "->" + NewState + NewSymbol;
            //string str = State + " " + Symbol + "->" + NewState + " " + NewSymbol + " "+Dir;
            if (Dir == Direction.Left)
            {
                str += "L";
            }
            if (Dir == Direction.Right)
            {
                str += "R";
            }
            if (Dir == Direction.Stop)
            {
                str += "S";
            }
            Console.WriteLine(str);
        }
    }
    class Program
    {
        static int LeftLimit = 0;
        static int RightLimit = 0;
        static int HeadPoint = 0;
        static List<Command> Commands = new List<Command>();
        static List<string> WordLine = new List<string>();

        static string HeadState = "";
        static string HeadWord
        {
            get
            {
                return WordLine[HeadPoint];
            }
            set
            {
                WordLine[HeadPoint] = value;
            }
        }
        static string Line
        {
            get
            {
                string str = string.Empty;
                foreach(string s in WordLine)
                {
                    str += s;
                }
                return str;
            }
        }
        static string LineHeaded
        {
            get
            {
                string str = string.Empty;
                for(int i = 0; i < HeadPoint; i++)
                {
                    str += WordLine[i];
                }
                //str += " " + HeadState + " ";
                str +=  HeadState ;
                for (int i = HeadPoint; i < WordLine.Count; i++)
                {
                    str += WordLine[i];
                }
                return str;
            }
        }

        static void Main(string[] args)
        {
            
            Console.WriteLine("Введите путь к файлу строки задачи (то как выглядит лента), где находится строка вида '1111011q10' без кавычек");
            var LinePath = Console.ReadLine().Replace("\"","");
            while(LinePath == null || !File.Exists(LinePath)) {
                if (LinePath == null)
                {
                    Console.WriteLine("Введён пустой путь к файлу со строкой задачи, пропробуйте ещё раз.");
                }
                else
                {
                    Console.WriteLine("Файл со строкой задачи не найден, пропробуйте ещё раз.");
                }
                LinePath = Console.ReadLine().Replace("\"", ""); ;
            }
            Console.WriteLine("Введите путь к файлу команд, определяющему как двигать каретку.");
            var CommandsPath = Console.ReadLine().Replace("\"", ""); ;
            while (CommandsPath == null || !File.Exists(CommandsPath))
            {
                if (CommandsPath == null)
                {
                    Console.WriteLine("Введён пустой путь к файлу команд, пропробуйте ещё раз.");
                }
                else
                {
                    Console.WriteLine("Файл команд не найден, пропробуйте ещё раз.");
                }
                CommandsPath = Console.ReadLine().Replace("\"", ""); ;
            }
            string[] Commands=File.ReadAllLines(CommandsPath);
            string line= File.ReadAllLines(LinePath)[0];
            foreach(string command in Commands)
            {
                if (command.Length > 0 && !command.Contains("//"))
                {
                    string State = command.Substring(0, 2);
                    string Symbol = command.Substring(2, 1);
                    string NewState = command.Substring(3, 2);
                    string NewSymbol = command.Substring(5, 1);
                    string Dir = command.Substring(6, 1);
                    Direction _Dir = Direction.Stop;
                    if (Dir.ToLower().Contains("l"))
                    {
                        _Dir = Direction.Left;
                    }
                    else
                    if (Dir.ToLower().Contains("r"))
                    {
                        _Dir = Direction.Right;
                    }
                    Program.Commands.Add(new Command(State, NewState, Symbol, NewSymbol, _Dir));
                }
            }
            for(int i = 0; i < line.Length-2; i++)
            {
                WordLine.Add("0");
            }
            HeadPoint = line.LastIndexOf("q");
            HeadState = line.Substring(HeadPoint,2);

            //Console.WriteLine("Head "+HeadPoint+" "+line);
            for (int i = 0; i < HeadPoint; i++)
            {
                WordLine[i] = string.Empty+line[i];
                //Console.WriteLine(line[i]);
            }
            for (int i = HeadPoint+2; i < line.Length; i++)
            {
                WordLine[i-2] = string.Empty + line[i];
            }

            RightLimit = WordLine.Count;
            foreach (Command command in Program.Commands)
            {
                command.Print();
            }
            Console.WriteLine(LineHeaded);


            Console.WriteLine("Начинаем работу.");

            while (true)
            {
                Command TheCommand = null;
                bool found=FindCommand(out TheCommand);
                if (!found)
                {
                    Console.WriteLine("Каретка остановлена, вид ленты такой: " + LineHeaded);
                    
                    break;
                }
                else
                {
                    TheCommand.Print();
                    if (TheCommand.Dir == Direction.Left && HeadPoint == 0)
                    {
                        AddZeroWords(Direction.Left);
                    }else
                    if (TheCommand.Dir == Direction.Right && HeadPoint == WordLine.Count-1)
                    {
                        AddZeroWords(Direction.Right);
                    }
                    HeadWord = TheCommand.NewSymbol;
                    HeadPoint += (int)TheCommand.Dir;
                    HeadState = TheCommand.NewState;
                    
                    Console.WriteLine(LineHeaded);
                }
            }

            Console.ReadKey();
            }
        static void AddZeroWords(Direction dir)
        {
            if (dir == Direction.Left)
            {
                LeftLimit++;
                WordLine.Insert(0, "0");
                HeadPoint++;
            }
            if (dir == Direction.Right)
            {
                RightLimit++;
                WordLine.Add("0");
            }
        }
        static bool FindCommand(out Command cmd)
        {
            
            foreach(Command command in Commands)
            {
                if (command.State == HeadState && command.Symbol == HeadWord)
                {
                    cmd = command;
                    return true;
                }
            }
            cmd = null;
            return false;
        }
    }
}
