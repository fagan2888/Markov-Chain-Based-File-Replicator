using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApplication3
{
    class FileGenerator
    {
        Random r;
        Dictionary<String, Chunk> WordRelations;
        List<String> LineBeginings;
        List<int> Lengths;

        public FileGenerator()
        {
            WordRelations = new Dictionary<string, Chunk>();
            LineBeginings = new List<String>();
            r = new Random();
            Lengths = new List<int>();
        }


        /// <summary>
        /// Reads a file and adds its information to a word relationship maping
        /// </summary>
        /// <param name="file"></param>
        public void ReadFile(String fName)
        {
            String lines = System.IO.File.ReadAllText(fName);
            string pattern = "\"";
            Regex re = new Regex(pattern);
            lines = re.Replace(lines, "");
            Char[] replaceChars = {'.', '!', '?'};
            String[] Sentances = lines.Split(replaceChars);
            String[] words;

            for (int i = 0; i < Sentances.Length; i++)
            {
                String[] Split = { " " };
                words = Sentances[i].Split(Split, StringSplitOptions.RemoveEmptyEntries);

                // add the generative word to the line beginings
                if (words.Length != 0)
                {
                    LineBeginings.Add(words[0]);

                    for (int j = 0; j < words.Length; j++)
                    {
                        // if the word does not have a chunk add it
                        if (!WordRelations.ContainsKey(words[j]))
                        {
                            WordRelations.Add(words[j], new Chunk(r));
                        }
                    }
                    for (int j = 0; j < words.Length - 3; j++)
                    {
                        // afterwords add the next three words
                        if(words[j] != words[j + 3])
                        {
                            WordRelations[words[j]].AddChunk(words[j + 1], words[j + 2], words[j + 3]);
                        }
                    }

                    if (words.Length < 4)
                    {
                        if (!WordRelations.ContainsKey(words[0]))
                        {
                            WordRelations.Add(words[0], new Chunk(r));
                        }
                        else if (words.Length == 3)
                        {
                            WordRelations[words[0]].AddChunk(words[1], words[2], "");
                        }
                        else if (words.Length == 2)
                        {
                            WordRelations[words[0]].AddChunk(words[1], "", "");
                        }
                        else
                        {
                            WordRelations[words[0]].AddChunk("", "", "");
                        }
                    }
                }
            }

            Lengths.Add(Sentances.Length);
        }

        /// <summary>
        /// Generates a file and writes it to the specified file
        /// </summary>
        /// <returns></returns>
        public void MakeFile(String f)
        {
            int Line = GetLines();
            //FileStream fs = new FileStream(f, FileMode.CreateNew, FileAccess.Write);
            StreamWriter sw = new StreamWriter(f);


            for (int index = 0; index < Line; index++)
            {
                sw.Write(MakeLine());
            }
            sw.Close();
        }

        public String MakeLine()
        {
            String ret = "";
            String Current = "";
            String[] WordsChosen;

            Current = LineBeginings[r.Next(LineBeginings.Count)];
            ret += Current;
            WordsChosen = WordRelations[Current].GenerateNextChunk();

            while (WordsChosen[WordsChosen.Length - 1] != "")
            {
                for (int i = 0; i < WordsChosen.Length; i++)
                {
                    ret += " " + WordsChosen[i];
                }
                WordsChosen = WordRelations[WordsChosen[WordsChosen.Length - 1]].GenerateNextChunk();
            } 
        

            


                return ret + ".\r\n";
        }


        private int GetLines()
        {
            int average = 0;
            int min = Int32.MaxValue;
            int max = 0;
            int Line;
            for (int i = 0; i < Lengths.Count; i++)
            {
                average += Lengths[i];
                if (Lengths[i] < min)
                {
                    min = Lengths[i];
                }
                if (Lengths[i] > max)
                {
                    max = Lengths[i];
                }
            }

            average = average / Lengths.Count;
            Line = r.Next(max - min) + min;

            return Line;
        }
    }

    class Chunk
    {
        static String[] empty = { "", "", "" };
        private List<String[]> Chunks;
        Random r;


        public Chunk(Random _r)
        {
            r = _r;
            Chunks  = new List<String[]>();
        }

        public void AddChunk( String a, String b, String c)
        {
            String[] Chunker = { a, b, c };
            Chunks.Add(Chunker);
        }

        public String[] GenerateNextChunk()
        {
            if (Chunks.Count == 0)
            {
                return empty;
            }
            return Chunks[r.Next(Chunks.Count)];
        }
    }
}
