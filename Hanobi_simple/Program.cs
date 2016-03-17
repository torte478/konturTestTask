using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanobiGame
{
    class Program
    {
        enum Attribute { color, rank };
        const String Colors = "RGBYW";

        class Card
        {
            int[] value;
            bool checkColor;
            bool checkRank;

            public Card(int color, int rank)
            {
                value = new int[2];
                value[(int)Attribute.color] = color;
                value[(int)Attribute.rank] = rank;

                checkColor = false;
                checkRank = false;
            }
        }

        static void Main(string[] args)
        {
            
        }
    }
}
