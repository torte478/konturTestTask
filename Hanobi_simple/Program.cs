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
            public bool[] checkValue;

            public Card(String str)
            {
                int color = Colors.IndexOf(str[0]);
                int rank = Convert.ToInt32(str[1]);

                value = new int[2];
                value[(int)Attribute.color] = color;
                value[(int)Attribute.rank] = rank;

                checkValue = new bool[2];
                checkValue[(int)Attribute.color] = false;
                checkValue[(int)Attribute.rank] = false;
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

            int turn, cardsOnTable, risk;
            
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
                cardsOnTable = 0;
                risk = 0;
            }

            private bool tellAttribute(List<String> command)
            {
                int value, attribute;
                if (command[1] == "color")
                {
                    attribute = (int)Attribute.color;
                    value = Colors.IndexOf(command[2][0]);
                }
                else
                {
                    attribute = (int)Attribute.rank;
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

                    if (card.value[attribute] != value)
                        return false;

                    card.checkValue[attribute] = true;
                }

                bool allChecked = players[playerIndex]
                                    .Where(card => !card.checkValue[attribute])
                                    .Count() == 0;
                return allChecked;
            }

            private bool playCard(Card card)
            {
                int cardColor = card.value[(int)Attribute.color];
                int cardRank = card.value[(int)Attribute.rank];

                bool correctAction = table[cardColor] == cardRank - 1;
                if (correctAction)
                {
                    ++table[cardColor];
                    if (card.checkValue[0] && card.checkValue[1] == false)
                        ++risk;

                    ++cardsOnTable;
                    if (cardsOnTable == cardsMaxNumber)
                        correctAction = false;
                }

                return correctAction;
            }

            public bool makeAction(String action)
            {
                if (players == null)
                    return true;

                ++turn;
                List<String> command = action.Split(' ').ToList();
                bool continueGame = true;
                if (command[0] == "Tell")
                    continueGame = tellAttribute(command);
                else
                {
                    int cardIndex = Convert.ToInt32(command[2]);
                    Card card = players[currentPlayer][cardIndex];

                    if (command[0] == "Play")
                        continueGame = playCard(card);

                    for (int i = cardIndex; i < cardsOnOneHands - 1; ++i)
                        players[currentPlayer][i] = players[currentPlayer][i + 1];
                    players[currentPlayer][cardsOnOneHands - 1] = new Card(cardDeck[currentCard++]);

                    continueGame = continueGame && (currentCard != cardDeck.Count());
                }

                return continueGame;
            }

            public string getStatistic()
            {
                return String.Format("Turn: {0}, cards: {1}, with risk: {2}", turn, cardsOnTable, risk);
            }

            public void stopGame()
            {
                players.Clear();
            }
        }

        static void Main(string[] args)
        {
            string command;
            HanobiCardGame game = null;
            while (true)
            {
                command = Console.ReadLine();
                if (command == null)
                    break;

                if (command.Substring(0, 5) == "Start")
                    game = new HanobiCardGame(command
                                                .Split(' ')
                                                .Skip(5)
                                                .ToList());
                else
                    if (!game.makeAction(command))
                {
                    Console.WriteLine(game.getStatistic());
                    game.stopGame();
                }
            }
        }
    }
}
