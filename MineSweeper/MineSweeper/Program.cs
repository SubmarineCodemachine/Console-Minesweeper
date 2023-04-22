using System;
using System.Collections.Generic;

namespace MineSweeper
{
    class Program
    {
        static private char[,] _display;
        static private char[,] _field;
        static private Random rng = new Random();

        static void Main(string[] args)
        {
            bool lost = false;
            bool replay = false;
            while (true)
            {
                if (!replay)
                {
                    Dictionary<string, ChoiceConsequence> menuItems = new Dictionary<string, ChoiceConsequence>();
                    menuItems.Add("Play", StartGame);
                    menuItems.Add("Exit", null);
                    if (Menu(menuItems, "---MineSweeper---"))
                        return;
                }
                else
                    replay = false;

                while (true)
                {
                    for(int y = 0; y < _display.GetLength(0); y++)
                        for(int x=0; x < _display.GetLength(1); x++)
                            if(_display[y,x] == '.')
                                for(int dy = y-1; dy <= y + 1; dy++)
                                    for (int dx = x - 1; dx <= x + 1; dx++)
                                        if (dy >= 0 && dy < _display.GetLength(0) && dx >= 0 && dx < _display.GetLength(1))
                                            if (_display[dy, dx] == '+')
                                                _display[dy, dx] = _field[dy, dx];
             
                    UpdateDisplay();
                    int[] coordinate;

                    Console.WriteLine("\nReveal koordinate[0] / Mark potential bomb[1]");
                    while (true)
                    {
                        ConsoleKey Input = Console.ReadKey(true).Key;
                        if (Input == ConsoleKey.D0)
                        {
                            Console.WriteLine("\nInput coordinate to reveal:");
                            coordinate = InputCoordinate();
                            _display[coordinate[0], coordinate[1]] = _field[coordinate[0], coordinate[1]];
                            lost = (_field[coordinate[0], coordinate[1]] == 'x');
                            break;
                        }
                        else if (Input == ConsoleKey.D1)
                        {
                            Console.WriteLine("\nInput coordinate to mark:");
                            coordinate = InputCoordinate();
                            _display[coordinate[0], coordinate[1]] = 'X';
                            break;
                        }
                    }
                    if (lost || ChekIfWon())
                        break;
                }
                int bombCount = 0;
                for(int y = 0; y < _field.GetLength(0); y++)
                    for(int x = 0; x < _field.GetLength(1); x++)
                        if (_field[y, x] == 'x')
                        {
                            bombCount++;
                            if (lost)
                                _display[y, x] = _field[y, x];
                        }
                UpdateDisplay();
                if (lost)
                {
                    Console.WriteLine("\nYou loose!\n");
                    lost = false;
                }
                else if (ChekIfWon())
                    Console.WriteLine("\nYou win!\n");
                Console.WriteLine("Back to menu[0] / Replay[1]");
                while (true)
                {
                    ConsoleKey input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.D0)
                        break;
                    else if(input == ConsoleKey.D1)
                    {
                        CreateDisplayAndField(new int[] { _field.GetLength(0), _field.GetLength(1) }, bombCount);
                        replay = true;
                        break;
                    }

                }
            }
        }

        static private bool ChekIfWon()
        {
            for(int y = 0; y < _display.GetLength(0); y++)
            {
                for(int x = 0; x < _display.GetLength(1); x++)
                {
                    if (_display[y, x] != '+' && (_display[y, x] == _field[y, x] || _field[y, x] == 'x') && _display[y, x] != 'x') ;
                    else
                        return false;
                }
            }
            return true;
        }

        static private int[] InputCoordinate() 
        {
            int[] coordinate = new int[2];
            
            do
            {
                do
                {
                    Console.Write("Y: ");
                } while (!Int32.TryParse(Console.ReadLine(), out coordinate[0]));
            } while (coordinate[0] >= _display.GetLength(0));

            do
            {
                do
                {
                    Console.Write("X: ");
                } while (!Int32.TryParse(Console.ReadLine(), out coordinate[1]));
            } while (coordinate[1] >= _display.GetLength(1));

            return coordinate;
        }

        static private void UpdateDisplay()
        {
            Console.Clear();

            for (int y = 0; y < _display.GetLength(0); y++)
            {
                for(int x = 0; x < _display.GetLength(1); x++)
                {
                    Console.Write(_display[y, x]);
                }
                Console.WriteLine("| > "+y);
            }
            for (int i = 0; i < _display.GetLength(1) + 1; i++)
                Console.Write("_");
            Console.WriteLine("\n");
            for (int i = 0; i < _display.GetLength(1); i++)
                Console.Write("v");
            Console.WriteLine("");

            int maxLength = (_display.GetLength(1) - 1).ToString().Length;
            for(int y=0; y < maxLength; y++)
            {
                for(int x = 0; x < _display.GetLength(1); x++)
                {
                    string dx = x.ToString();
                    if (y < dx.Length)
                        Console.Write(dx[y]);
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        private delegate void ChoiceConsequence();

        static private bool Menu(Dictionary<string,ChoiceConsequence> options, string headLine="", ChoiceConsequence previousStep = null)
        {
            int cursorPos = 0;
            while (true)
            {
                Console.Clear();
                if(headLine != "")
                    Console.WriteLine(headLine + "\n");

                {
                    int i = 0;
                    foreach (KeyValuePair<string, ChoiceConsequence> pair in options)
                    {
                        if (cursorPos == i)
                            Console.Write("> ");
                        Console.WriteLine(pair.Key);
                        i++;
                    }
                }
 
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        if (cursorPos != 0)
                            cursorPos--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursorPos != options.Count - 1)
                            cursorPos++;
                        break;
                    case ConsoleKey.Enter:
                        int i=0;
                        foreach(KeyValuePair<string,ChoiceConsequence> pair in options)
                        {
                            if (cursorPos == i)
                                if (pair.Value != null)
                                    pair.Value();
                                else
                                    return true;
                            i++;
                        }
                        return false;
                    case ConsoleKey.Escape:
                        if (previousStep != null)
                        {
                            previousStep();
                            return false;
                        }
                        return true;
                }
            }
        }  
        
        static private void StartGame()
        {
            int[] dimensions = new int[2];
            Console.Clear();
            Console.WriteLine("Enter dimensions\n");
            do
            {
                Console.WriteLine("Height:");
            } while (!Int32.TryParse(Console.ReadLine(), out dimensions[0]));

            Console.WriteLine();

            do
            {
                Console.WriteLine("Width:");
            } while (!Int32.TryParse(Console.ReadLine(), out dimensions[1]));

            Console.WriteLine("\n");

            int bombs;
            do
            {
                Console.WriteLine("Ammount of bombs:");
            } while (!Int32.TryParse(Console.ReadLine(), out bombs));

            CreateDisplayAndField(dimensions,bombs);
        }

        static private void CreateDisplayAndField(int[] dimensions, int bombs)
        {
            _display = new char[dimensions[0], dimensions[1]];
            _field = new char[dimensions[0], dimensions[1]];

            for (int i = 0; i < bombs; i++)
            {
                int randY = 0;
                int randX = 0;
                do
                {
                    randY = rng.Next(0, _field.GetLength(0));
                    randX = rng.Next(0, _field.GetLength(1));
                } while (_field[randY, randX] == 'x');
                _field[randY, randX] = 'x';
            }

            for (int y = 0; y < dimensions[0]; y++)
            {
                for (int x = 0; x < dimensions[1]; x++)
                {
                    _display[y, x] = '+';

                    int nearbyBombs=0;
                    for(int yy = y-1; yy <= y+1; yy++)
                    {
                        for(int xx = x-1; xx <= x + 1; xx++)
                        {
                            if (yy >= 0 && yy < _field.GetLength(0) && xx >= 0 && xx < _field.GetLength(1))
                                if (_field[yy, xx] == 'x')
                                    nearbyBombs++;
                        }
                    }
                    if (_field[y, x] != 'x')
                        if (nearbyBombs != 0)
                            _field[y, x] = Convert.ToChar(nearbyBombs.ToString());
                        else
                            _field[y, x] = '.';
                }
            }
        }
    }
}
