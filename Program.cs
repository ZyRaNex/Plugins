using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parser
{
    public class Parser
    {
        public enum Direction { Top=0, Right=1, Down=2, Left=3};

        public const int GridSize = 100;
        public const int GridMiddle = 50;
        public static List<char> SceneTypeList = new List<char>{
            '═','║',
            '╵','╶','╷','╴',
            '↑','→','↓','←',
            '╚','╝','╔','╗',
            '╣','╠','╩','╦',
            '╬'
        };
        public static List<char> TopFacing = new List<char>{
            '║',
            '╵',
            '↑','↓',
            '╚','╝',
            '╣','╠','╩',
            '╬'
        };
        public static List<char> RightFacing = new List<char>{
            '═',
            '╶',
            '→','←',
            '╚','╔',
            '╠','╩','╦',
            '╬'
        };
        public static List<char> DownFacing = new List<char>{
            '║',
            '╷',
            '↑','↓',
            '╔','╗',
            '╣','╠','╦',
            '╬'
        };
        public static List<char> LeftFacing = new List<char>{
            '═',
            '╴',
            '→','←',
            '╝','╗',
            '╣','╩','╦',
            '╬'
        };
        public static List<char> Lines = new List<char>{
            '═','║'
        };
        public static List<char> Deadends = new List<char>{
            '╵','╶','╷','╴',
        };
        public static List<char> Corners = new List<char>{
            '╚','╝','╔','╗',
        };
        public static List<char> Ts = new List<char>{
            '╣','╠','╩','╦',
        };
        public static List<char> Xs = new List<char>{
            '╬'
        };
        public static List<char> TileCountings = new List<char>{
            '═','║',
            '╚','╝','╔','╗',
            '╣','╠','╩','╦',
            '╬'
        };
        public class MapEntry
        {
            public MapEntry()
            {
                Name_ = "";
                Identifier_ = 0;
                Map_ = "";
                EntranceNumber = 0;
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        Grid[i,j] = ' ';
                    }
                }
                EntranceToExitX = 0;
                EntranceToExitY = 0;
                Deadends = 0;
                Lines = 0;
                Corners = 0;
                Ts = 0;
                Xs = 0;
                FourCorner = false;
                Tiles = 0;
            }
            public bool IsEntry(int i, int j)
            {
                if (!SceneTypeList.Contains(Grid[i, j]))
                {
                    return false;
                }
                if (Grid[i, j] == '↑')
                {
                    if ((i - 1) > 0)
                    {
                        if (Grid[i - 1, j] == '║' || Grid[i - 1, j] == '╔' || Grid[i - 1, j] == '╗' || Grid[i - 1, j] == '╣' || Grid[i - 1, j] == '╠' || Grid[i - 1, j] == '╦' || Grid[i - 1, j] == '╬')
                        {
                            return true;
                        }
                    }
                }
                else if (Grid[i, j] == '→')
                {
                    if ((j + 1) < GridSize)
                    {
                        if (Grid[i, j + 1] == '═' || Grid[i, j + 1] == '╝' || Grid[i, j + 1] == '╗' || Grid[i, j + 1] == '╣' || Grid[i, j + 1] == '╩' || Grid[i, j + 1] == '╦' || Grid[i, j + 1] == '╬')
                        {
                            return true;
                        }
                    }
                }
                else if (Grid[i, j] == '↓')
                {
                    if ((i + 1) < GridSize)
                    {
                        if (Grid[i + 1, j] == '║' || Grid[i + 1, j] == '╚' || Grid[i + 1, j] == '╝' || Grid[i + 1, j] == '╣' || Grid[i + 1, j] == '╠' || Grid[i + 1, j] == '╩' || Grid[i + 1, j] == '╬')
                        {
                            return true;
                        }
                    }
                }
                else if (Grid[i, j] == '←')
                {
                    if ((j - 1) > 0)
                    {
                        if (Grid[i, j - 1] == '═' || Grid[i, j - 1] == '╚' || Grid[i, j - 1] == '╔' || Grid[i, j - 1] == '╠' || Grid[i, j - 1] == '╩' || Grid[i, j - 1] == '╦' || Grid[i, j - 1] == '╬')
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public bool IsExit(int i, int j)
            {
                if (!SceneTypeList.Contains(Grid[i, j]))
                {
                    return false;
                }
                if (Grid[i, j] == '↓')
                {
                    if ((i - 1) > 0)
                    {
                        if (Grid[i - 1, j] == '║' || Grid[i - 1, j] == '╔' || Grid[i - 1, j] == '╗' || Grid[i - 1, j] == '╣' || Grid[i - 1, j] == '╠' || Grid[i - 1, j] == '╦' || Grid[i - 1, j] == '╬')
                        {
                            return true;
                        }
                    }
                }
                else if (Grid[i, j] == '←')
                {
                    if ((j + 1) < GridSize)
                    {
                        if (Grid[i, j + 1] == '═' || Grid[i, j + 1] == '╝' || Grid[i, j + 1] == '╗' || Grid[i, j + 1] == '╣' || Grid[i, j + 1] == '╩' || Grid[i, j + 1] == '╦' || Grid[i, j + 1] == '╬')
                        {
                            return true;
                        }
                    }
                }
                else if (Grid[i, j] == '↑')
                {
                    if ((i + 1) < GridSize)
                    {
                        if (Grid[i + 1, j] == '║' || Grid[i + 1, j] == '╚' || Grid[i + 1, j] == '╝' || Grid[i + 1, j] == '╣' || Grid[i + 1, j] == '╠' || Grid[i + 1, j] == '╩' || Grid[i + 1, j] == '╬')
                        {
                            return true;
                        }
                    }
                }
                else if (Grid[i, j] == '→')
                {
                    if ((j - 1) > 0)
                    {
                        if (Grid[i, j - 1] == '═' || Grid[i, j - 1] == '╚' || Grid[i, j - 1] == '╔' || Grid[i, j - 1] == '╠' || Grid[i, j - 1] == '╩' || Grid[i, j - 1] == '╦' || Grid[i, j - 1] == '╬')
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public bool IsValid(int i, int j)
            {
                if (!SceneTypeList.Contains(Grid[i, j]))
                {
                    return true;
                }
                if (IsEntry(i, j) || IsExit(i, j))
                {
                    return true;
                }

                if (TopFacing.Contains(Grid[i, j]))
                {
                    if ((i - 1) > 0)
                    {
                        if (!DownFacing.Contains(Grid[i - 1, j]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                if (RightFacing.Contains(Grid[i, j]))
                {
                    if ((j + 1) < GridSize)
                    {
                        if (!LeftFacing.Contains(Grid[i, j + 1]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                if (DownFacing.Contains(Grid[i, j]))
                {
                    if ((i + 1) < GridSize)
                    {
                        if (!TopFacing.Contains(Grid[i + 1, j]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                if (LeftFacing.Contains(Grid[i, j]))
                {
                    if ((j - 1) > 0)
                    {
                        if (!RightFacing.Contains(Grid[i, j - 1]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            public string Name_ { get; set; }
            public long Identifier_ { get; set; }
            public string Map_ { get; set; }
            public int EntranceNumber { get; set; }
            public char[,] Grid = new char[GridSize, GridSize];
            public int EntranceToExitX;
            public int EntranceToExitY;
            public int Deadends;
            public int Lines;
            public int Corners;
            public int Ts;
            public int Xs;
            public bool FourCorner;
            public int Tiles;
            public Direction EntranceDirection;
            public Direction ExitDirection;
        }
        public static void Main()
        {
            List<string> MapNames = new List<string>{"Battlefields", "Blue Cave", "Brown Cave", "Cathedral", "Cave", "Church", "Corvus", "Crater", "Crypt", "Desert", "Eternal Woods", "Eternal Woods (no snow)",
                "Festering", "Forgotten Ruins", "Green Cave", "Halls of Agony", "Hell Rift", "Ice Cave", "Keeps", "Pest Tunnel", "Sewers", "Sewers (water)", "Shocktowers", "Shrouded Moors", "Spaghetti", "Spider Cavern",
                "Spikey Cave", "Spire", "Westmarch", "Westmarch Ruins", "Zoltun Kulle"};

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Someone\source\repos\testsharp\testsharp\Result.txt", false))
            {
                Console.WriteLine("Map Analysis");
                file.WriteLine("Map Analysis");
            }

            foreach (string MapName in MapNames)
            {
                List<MapEntry> MapEntrys = new List<MapEntry>();
                StreamReader reader = File.OpenText(@"C:\Users\Someone\Desktop\thMeta\logs\MapKnowledgeFinal.txt");
                string line;
                MapEntry New = new MapEntry();
                int Amount = 0;
                Console.WriteLine("reading");
                while ((line = reader.ReadLine()) != null)
                {
                    string[] items = line.Split('\n');

                    foreach (string item in items)
                    {
                        Regex regexName = new Regex("^[a-zA-Z]+");
                        Match matchName = regexName.Match(item);
                        Regex regexNumber = new Regex("^[0-9]{4,}");
                        Match matchNumber = regexNumber.Match(item);
                        if (matchName.Success)
                        {
                            //check if we are done with previous item
                            if (New.Map_ != "" && New.Identifier_ != 0 && New.Map_ != "")
                            {
                                MapEntry New2 = new MapEntry();
                                New2.Name_ = New.Name_;
                                New2.Identifier_ = New.Identifier_;
                                New2.Map_ = New.Map_;
                                New2.EntranceNumber = Amount;
                                if (New.Name_.Contains(MapName))
                                {
                                    MapEntrys.Add(New2);
                                }
                                New.Identifier_ = 0;
                                New.Map_ = "";
                                Amount++;
                            }
                            New.Name_ = item;
                        }
                        else if (matchNumber.Success)
                        {
                            New.Identifier_ = long.Parse(item); ;
                        }
                        else
                        {
                            if (item != "")
                            {
                                if (New.Map_ != "")
                                {
                                    New.Map_ += System.Environment.NewLine;
                                }
                                New.Map_ += item;
                            }
                        }
                    }
                }

                Console.WriteLine("removing duplicates");
                foreach (MapEntry map in MapEntrys.ToList())
                {
                    foreach (MapEntry map2 in MapEntrys.ToList())
                    {
                        if (map.EntranceNumber != map2.EntranceNumber)
                        {
                            if (map.Name_ == map2.Name_ && map.Identifier_ == map2.Identifier_ && map.Map_ == map2.Map_)
                            {
                                /*Console.WriteLine("duplicate:");
                                Console.WriteLine(map.Name_);
                                Console.WriteLine(map.Identifier_);
                                Console.WriteLine(map.Map_);*/
                                MapEntrys.Remove(map2);
                            }
                        }
                    }
                }

                Console.WriteLine("removing invalid symbols");
                foreach (MapEntry map in MapEntrys.ToList())
                {
                    foreach (char c in map.Map_)
                    {
                        if (!SceneTypeList.Contains(c) && !String.IsNullOrWhiteSpace(c.ToString()))
                        {
                            Console.WriteLine("invalid symbol");
                            Console.WriteLine(c);
                            Console.WriteLine("in");
                            Console.WriteLine(map.Name_);
                            Console.WriteLine(map.Identifier_);
                            MapEntrys.Remove(map);
                        }
                    }
                }

                Console.WriteLine("assigning grids");
                foreach (MapEntry map in MapEntrys)
                {
                    int i = 0;
                    int j = 0;
                    String[] InputLines = map.Map_.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);

                    foreach (String l in InputLines)
                    {
                        foreach (char c in l)
                        {
                            if (SceneTypeList.Contains(c))
                            {
                                if (i < GridSize && j < GridSize)
                                {
                                    map.Grid[j, i] = c;
                                }
                            }
                            i++;
                        }
                        j++;
                        i = 0;
                    }
                }

                int EntranceX = 0;
                int EntranceY = 0;
                int ExitX = 0;
                int ExitY = 0;
                int OffsetX = 0;
                int OffsetY = 0;
                char[,] Copy = new char[GridSize, GridSize];
                Console.WriteLine("centering grids");
                foreach (MapEntry map in MapEntrys)
                {
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            if (map.IsEntry(i, j))
                            {
                                EntranceX = i;
                                EntranceY = j;
                            }
                        }
                    }
                    OffsetX = GridMiddle - EntranceX;
                    OffsetY = GridMiddle - EntranceY;
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            Copy[i, j] = ' ';
                        }
                    }
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            Copy[i, j] = map.Grid[i, j];
                        }
                    }
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            map.Grid[i, j] = ' ';
                        }
                    }
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            if ((i + OffsetX) > 0 && (j + OffsetY) > 0 && (i + OffsetX) < GridSize && (j + OffsetY) < GridSize)
                            {
                                map.Grid[i + OffsetX, j + OffsetY] = Copy[i, j];
                            }
                        }
                    }
                    //find Entrance-to-Exit relation;
                    EntranceX = 0;
                    EntranceY = 0;
                    ExitX = 0;
                    ExitY = 0;
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            if (map.IsEntry(i, j))
                            {
                                EntranceX = i;
                                EntranceY = j;
                            }
                            if (map.IsExit(i, j))
                            {
                                ExitX = i;
                                ExitY = j;
                            }
                        }
                    }
                    map.EntranceToExitX = ExitX - EntranceX;
                    map.EntranceToExitY = ExitY - EntranceY;
                }

                Console.WriteLine("removing not validly connected ones");
                foreach (MapEntry map in MapEntrys.ToList())
                {
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            if (!map.IsValid(i, j))
                            {
                                Console.WriteLine("invalid connection");
                                Console.WriteLine("{0}: ({1}, {2})", map.Grid[i, j], i, j);
                                Console.WriteLine("in");
                                Console.WriteLine(map.Name_);
                                Console.WriteLine(map.Identifier_);
                                MapEntrys.Remove(map);
                            }
                        }
                    }
                }

                Console.WriteLine("counting Tiles lines/corners etc");
                foreach (MapEntry map in MapEntrys)
                {
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            if (Lines.Contains(map.Grid[i, j]))
                            {
                                map.Lines++;
                            }
                            if (Deadends.Contains(map.Grid[i, j]))
                            {
                                map.Deadends++;
                            }
                            if (Corners.Contains(map.Grid[i, j]))
                            {
                                map.Corners++;
                            }
                            if (Ts.Contains(map.Grid[i, j]))
                            {
                                map.Ts++;
                                if (map.Grid[i, j] == '╣' || map.Grid[i, j] == '╠')
                                {
                                    if ((i - 1) > 0)
                                    {
                                        if (Deadends.Contains(map.Grid[i - 1, j]))
                                        {
                                            map.Corners++;
                                        }
                                    }
                                    else if ((i + 1) < GridSize)
                                    {
                                        if (Deadends.Contains(map.Grid[i + 1, j]))
                                        {
                                            map.Corners++;
                                        }
                                    }
                                }
                                else if (map.Grid[i, j] == '╩' || map.Grid[i, j] == '╦')
                                {
                                    if ((j - 1) > 0)
                                    {
                                        if (Deadends.Contains(map.Grid[i, j - 1]))
                                        {
                                            map.Corners++;
                                        }
                                    }
                                    else if ((j + 1) < GridSize)
                                    {
                                        if (Deadends.Contains(map.Grid[i, j + 1]))
                                        {
                                            map.Corners++;
                                        }
                                    }
                                }
                            }
                            if (Xs.Contains(map.Grid[i, j]))
                            {
                                map.Xs++;
                            }
                            if (TileCountings.Contains(map.Grid[i, j]))
                            {
                                map.Tiles++;
                            }
                            if (map.IsEntry(i, j))
                            {
                                if (map.Grid[i, j] == '↑')
                                {
                                    map.EntranceDirection = Direction.Top;
                                }
                                else if (map.Grid[i, j] == '→')
                                {
                                    map.EntranceDirection = Direction.Right;
                                }
                                else if (map.Grid[i, j] == '↓')
                                {
                                    map.EntranceDirection = Direction.Down;
                                }
                                else if (map.Grid[i, j] == '←')
                                {
                                    map.EntranceDirection = Direction.Left;
                                }
                            }
                            else if (map.IsExit(i, j))
                            {
                                if (map.Grid[i, j] == '↓')
                                {
                                    map.ExitDirection = Direction.Top;
                                }
                                else if (map.Grid[i, j] == '←')
                                {
                                    map.ExitDirection = Direction.Right;
                                }
                                else if (map.Grid[i, j] == '↑')
                                {
                                    map.ExitDirection = Direction.Down;
                                }
                                else if (map.Grid[i, j] == '→')
                                {
                                    map.ExitDirection = Direction.Left;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("checking for four corner");
                foreach (MapEntry map in MapEntrys.ToList())
                {
                    for (int i = 0; i < GridSize - 1; i++)
                    {
                        for (int j = 0; j < GridSize - 1; j++)
                        {
                            if (!RightFacing.Contains(map.Grid[i, j]))
                                continue;
                            if (!DownFacing.Contains(map.Grid[i, j]))
                                continue;
                            if (!LeftFacing.Contains(map.Grid[i, j + 1]))
                                continue;
                            if (!DownFacing.Contains(map.Grid[i, j + 1]))
                                continue;
                            if (!RightFacing.Contains(map.Grid[i + 1, j]))
                                continue;
                            if (!TopFacing.Contains(map.Grid[i + 1, j]))
                                continue;
                            if (!LeftFacing.Contains(map.Grid[i + 1, j + 1]))
                                continue;
                            if (!TopFacing.Contains(map.Grid[i + 1, j + 1]))
                                continue;
                            map.FourCorner = true;
                        }
                    }
                }

                int[] Tiles = new int[100];
                int[,,] Heatmap = new int[GridSize, GridSize, 4];
                int[] TypeCount = new int[SceneTypeList.Count()];

                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        for (int k = 0; j < 3; j++)
                        {
                            Heatmap[i, j, k] = 0;
                        }
                    }
                }

                int FourCorners = 0;
                int NotFourCorners = 0;
                for (int i = 0; i < 100; i++)
                {
                    Tiles[i] = 0;
                }
                foreach (MapEntry map in MapEntrys.ToList())
                {
                    //if (map.Tiles > 2)
                    //if (map.Deadends > 0)
                    //if (map.EntranceDirection == Direction.Left)
                    //if (!map.FourCorner)
                    //if (map.Xs > 0)
                    //if (true)
                    {
                        for (int i = 0; i < GridSize; i++)
                        {
                            for (int j = 0; j < GridSize; j++)
                            {
                                if (TileCountings.Contains(map.Grid[i, j]))
                                {
                                    if (TopFacing.Contains(map.Grid[i, j]))
                                    {
                                        Heatmap[i, j, (int)Direction.Top]++;
                                    }
                                    if (RightFacing.Contains(map.Grid[i, j]))
                                    {
                                        Heatmap[i, j, (int)Direction.Right]++;
                                    }
                                    if (DownFacing.Contains(map.Grid[i, j]))
                                    {
                                        Heatmap[i, j, (int)Direction.Down]++;
                                    }
                                    if (LeftFacing.Contains(map.Grid[i, j]))
                                    {
                                        Heatmap[i, j, (int)Direction.Left]++;
                                    }
                                }
                                int k = 0;
                                foreach (char SceneType in SceneTypeList)
                                {
                                    if (map.Grid[i, j] == SceneType)
                                    {
                                        TypeCount[k]++;
                                    }
                                    k++;
                                }
                            }
                        }
                        if (map.Tiles > 0 && map.Tiles < 100)
                        {
                            Tiles[map.Tiles]++;
                        }
                        if (map.FourCorner)
                        {
                            FourCorners++;
                        }
                        else
                        {
                            NotFourCorners++;
                        }
                    }
                }

                Console.WriteLine("creating the coded string");
                string CodedMapsCombined = "";
                List<string> CodedMaps = new List<string>();
                foreach (MapEntry map in MapEntrys.ToList())
                {
                    string CurrentMap = "";
                    char[,] DummyGrid = new char[GridSize, GridSize];
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            DummyGrid[i, j] = ' ';
                        }
                    }

                    DummyGrid[GridMiddle, GridMiddle] = map.Grid[GridMiddle, GridMiddle];
                    CurrentMap += DummyGrid[GridMiddle, GridMiddle];

                    for (int k = 0; k < 100; k++)//maps should be smaller than 100 tiles
                    {
                        bool OpenConnection = false;
                        Direction Connection = Direction.Top;
                        for (int l = 0; l < 4; l++)
                        {
                            bool found = false;
                            for (int i = 0; i < GridSize; i++)
                            {
                                for (int j = 0; j < GridSize; j++)
                                {
                                    if (DummyGrid[i, j] == '↑')
                                    {
                                        if (Connection == Direction.Top && map.IsEntry(i, j) && DummyGrid[i - 1, j] == ' ')
                                        {
                                            CurrentMap += map.Grid[i - 1, j];
                                            DummyGrid[i - 1, j] = map.Grid[i - 1, j];
                                            found = true;
                                            break;
                                        }
                                        else if (Connection == Direction.Down && map.IsExit(i, j) && DummyGrid[i + 1, j] == ' ')
                                        {
                                            CurrentMap += map.Grid[i + 1, j];
                                            DummyGrid[i + 1, j] = map.Grid[i + 1, j];
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (DummyGrid[i, j] == '→')
                                    {
                                        if (Connection == Direction.Right && map.IsEntry(i, j) && DummyGrid[i, j + 1] == ' ')
                                        {
                                            CurrentMap += map.Grid[i, j + 1];
                                            DummyGrid[i, j + 1] = map.Grid[i, j + 1];
                                            found = true;
                                            break;
                                        }
                                        else if (Connection == Direction.Left && map.IsExit(i, j) && DummyGrid[i, j - 1] == ' ')
                                        {
                                            CurrentMap += map.Grid[i, j - 1];
                                            DummyGrid[i, j - 1] = map.Grid[i, j - 1];
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (DummyGrid[i, j] == '↓')
                                    {
                                        if (Connection == Direction.Down && map.IsEntry(i, j) && DummyGrid[i + 1, j] == ' ')
                                        {
                                            CurrentMap += map.Grid[i + 1, j];
                                            DummyGrid[i + 1, j] = map.Grid[i + 1, j];
                                            found = true;
                                            break;
                                        }
                                        else if (Connection == Direction.Top && map.IsExit(i, j) && DummyGrid[i - 1, j] == ' ')
                                        {
                                            CurrentMap += map.Grid[i - 1, j];
                                            DummyGrid[i - 1, j] = map.Grid[i - 1, j];
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (DummyGrid[i, j] == '←')
                                    {
                                        if (Connection == Direction.Left && map.IsEntry(i, j) && DummyGrid[i, j - 1] == ' ')
                                        {
                                            CurrentMap += map.Grid[i, j - 1];
                                            DummyGrid[i, j - 1] = map.Grid[i, j - 1];
                                            found = true;
                                            break;
                                        }
                                        else if (Connection == Direction.Right && map.IsExit(i, j) && DummyGrid[i, j + 1] == ' ')
                                        {
                                            CurrentMap += map.Grid[i, j + 1];
                                            DummyGrid[i, j + 1] = map.Grid[i, j + 1];
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (Connection == Direction.Top && TopFacing.Contains(DummyGrid[i, j]) && DummyGrid[i - 1, j] == ' ')
                                    {
                                        CurrentMap += map.Grid[i - 1, j];
                                        DummyGrid[i - 1, j] = map.Grid[i - 1, j];
                                        found = true;
                                        break;
                                    }
                                    else if (Connection == Direction.Right && RightFacing.Contains(DummyGrid[i, j]) && DummyGrid[i, j + 1] == ' ')
                                    {
                                        CurrentMap += map.Grid[i, j + 1];
                                        DummyGrid[i, j + 1] = map.Grid[i, j + 1];
                                        found = true;
                                        break;
                                    }
                                    else if (Connection == Direction.Down && DownFacing.Contains(DummyGrid[i, j]) && DummyGrid[i + 1, j] == ' ')
                                    {
                                        CurrentMap += map.Grid[i + 1, j];
                                        DummyGrid[i + 1, j] = map.Grid[i + 1, j];
                                        found = true;
                                        break;
                                    }
                                    else if (Connection == Direction.Left && LeftFacing.Contains(DummyGrid[i, j]) && DummyGrid[i, j - 1] == ' ')
                                    {
                                        CurrentMap += map.Grid[i, j - 1];
                                        DummyGrid[i, j - 1] = map.Grid[i, j - 1];
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                {
                                    break;
                                }
                            }
                            Connection++;
                            if (found)
                            {
                                OpenConnection = true;
                                break;
                            }
                        }
                        if (!OpenConnection)
                        {
                            break;
                        }
                    }
                    bool AlreadyExists = false;
                    for (int i = 0; i < CodedMaps.Count; i++)
                    {
                        if (CodedMaps[i].Contains(CurrentMap))
                        {
                            AlreadyExists = true;
                            int Numbers = CodedMaps[i].Count(c => Char.IsNumber(c));

                            if (Numbers > 0)
                            {
                                int n = Int32.Parse(CodedMaps[i].Substring(0, Numbers));
                                CodedMaps[i] = (n + 1) + CodedMaps[i].Substring(Numbers, CodedMaps[i].Length - Numbers);
                            }
                            else
                            {
                                CodedMaps[i] = "2" + CodedMaps[i];
                            }
                        }
                    }

                    if (!AlreadyExists)
                    {
                        CodedMaps.Add(CurrentMap);
                    }
                }

                foreach (string name in CodedMaps)
                {
                    CodedMapsCombined += name;
                }

                Console.WriteLine("MapEntrys:");
                Console.WriteLine(MapEntrys.Count());
                int count = 0;

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Someone\source\repos\testsharp\testsharp\Result.txt", true))
                {
                    file.WriteLine(MapName);
                    for (int i = 0; i < 100; i++)
                    {
                        if (Tiles[i] > 0)
                        {
                            //file.WriteLine("{0} Tiles: {1}", i, Tiles[i]);
                            Console.WriteLine("{0} Tiles: {1}", i, Tiles[i]);
                        }
                    }
                    //file.WriteLine("Four Corners: {0}", FourCorners);
                    //file.WriteLine("not Four Corners: {0}", NotFourCorners);
                    Console.WriteLine("Four Corners: {0}", FourCorners);
                    Console.WriteLine("not Four Corners: {0}", NotFourCorners);
                    //file.WriteLine();
                    foreach (MapEntry map in MapEntrys)
                    {
                        //if (map.Tiles > 2)
                        //if (map.Deadends > 0)
                        //if (map.EntranceDirection == Direction.Left)
                        //if (!map.FourCorner)
                        //if (map.Xs>0)
                        //if (true)
                        {
                            Console.WriteLine(map.Name_);
                            Console.WriteLine(map.Identifier_);
                            Console.WriteLine(map.Map_);
                            Console.WriteLine(map.EntranceNumber);
                            Console.WriteLine("{0}, {1}", map.EntranceToExitX, map.EntranceToExitY);
                            Console.WriteLine(map.ExitDirection);

                            //file.WriteLine(map.Name_);
                            //file.WriteLine(map.Identifier_);
                            //file.WriteLine(map.Map_);
                            //file.WriteLine(map.EntranceNumber);
                            file.WriteLine(map.EntranceDirection);
                            //file.WriteLine(map.ExitDirection);
                            //file.WriteLine(map.Corners);
                            //file.WriteLine("{0}, {1}", map.EntranceToExitX, map.EntranceToExitY);
                            //file.WriteLine(map.FourCorner);
                            //file.WriteLine(map.Grid[51,49]);
                            //file.WriteLine("Tiles: {0}", map.Tiles);
                            //file.WriteLine("Deadends: {0}", map.Deadends);
                            count++;
                        }

                    }
                    //file.WriteLine();
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            if (Heatmap[i, j, 0] != 0 || Heatmap[i, j, 1] != 0 || Heatmap[i, j, 2] != 0 || Heatmap[i, j, 3] != 0)
                            {
                                Console.WriteLine("i,j:" + i + " " + j + " top: " + Heatmap[i, j, (int)Direction.Top] + " right: " + Heatmap[i, j, (int)Direction.Right] + " down: " + Heatmap[i, j, (int)Direction.Down] + " left: " + Heatmap[i, j, (int)Direction.Left]);
                            }
                        }
                    }

                    int k = 0;
                    foreach (char SceneType in SceneTypeList)
                    {
                        Console.WriteLine("{0}:{1}", SceneType, TypeCount[k]);
                        //file.WriteLine("{0}:{1}", SceneType, TypeCount[k]);
                        k++;
                    }
                    //file.WriteLine();
                    Console.WriteLine("{0}:", MapName);
                    Console.WriteLine(count);
                    //file.WriteLine("{0}:", MapName);
                    //file.WriteLine(count);

                    Console.WriteLine("{0}", CodedMapsCombined);
                    file.WriteLine("{0}", CodedMapsCombined);
                }
            }

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Someone\source\repos\testsharp\testsharp\Result.txt", true))
            {
                Console.WriteLine("Done");
                file.WriteLine("Done");
            }
            while (true) { };
        }
    }
}