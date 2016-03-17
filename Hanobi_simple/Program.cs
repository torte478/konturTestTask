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
            public int[] value;
            public bool checkColor;
            public bool checkRank;

            public Card(String str)
            {
                int color = Colors.IndexOf(str[0]);
                int rank = Convert.ToInt32(str[1]);

                value = new int[2];
                value[(int)Attribute.color] = color;
                value[(int)Attribute.rank] = rank;

                checkColor = false;
                checkRank = false;
            }
        }

        const int cardsOnOneHands = 5;
        const int cardsMaxNumber = 25;

        class HanobiCardGame
        {
            List<List<Card>> players;
            List<int> table;
            List<String> cardDeck;
            int currentPlayer, currentCard;

            int turn, cards, risk;
            
            public HanobiCardGame(List<String> cardDeck)
            {
                this.cardDeck = cardDeck;
                players = new List<List<Card>>(2);
                players[0] = cardDeck
                            .Take(cardsOnOneHands)
                            .Select(str => new Card(str))
                            .ToList();
                players[1] = cardDeck
                            .Skip(cardsOnOneHands)
                            .Take(cardsOnOneHands)
                            .Select(str => new Card(str))
                            .ToList();

                currentPlayer = 0;
                currentCard = cardsOnOneHands * 2;
                turn = 0;
                cards = 0;
                risk = 0;
            }

            private bool tellAttribute(List<String> command)
            {
                Attribute attribute;
                int value;
                if (command[1] == "color")
                {
                    attribute = Attribute.color;
                    value = Colors.IndexOf(command[2][0]);
                }
                else
                {
                    attribute = Attribute.rank;
                    value = Convert.ToInt32(command[2]);
                }

                List<int> cards = command
                                    .Skip(5)
                                    .Select(str => Convert.ToInt32(str))
                                    .ToList();
                int playerIndex = (currentPlayer + 1) % 2;

                foreach (int cardIndex in cards)
                {
                    Card card = players[playerIndex][cardIndex];

                    if (card.value[(int)attribute] != value)
                        return false;

                    if (attribute == Attribute.color)
                        card.checkColor = true;
                    else
                        card.checkRank = true;
                }

                //TODO

                return true;
            }

            public bool makeAction(String action)
            {
                ++turn;

                return true;
            }

            public string getStatistic()
            {
                return String.Format("Turn: {0}, cards: {1}, with risk: {2}", turn, cards, risk);
            }
        }

        static void Main(string[] args)
        {

        }
    }
}
