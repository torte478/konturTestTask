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
            public List<List<bool>> checkValue;

            public Card(String str)
            {
                int color = Colors.IndexOf(str[0]);
                int rank = Convert.ToInt32(str.Substring(1));

                value = new int[2];
                value[(int)Attribute.color] = color;
                value[(int)Attribute.rank] = rank - 1;

                checkValue = Enumerable
                                .Range(0, 2)
                                .Select(i => Enumerable.Range(0, 5)
                                                        .Select(j => false)
                                                        .ToList())
                                .ToList();
            }

            public bool IsKnownCard()
            {
                bool isKnownCard = checkValue[0][value[0]] && checkValue[1][value[1]];
                if (!isKnownCard)
                {
                    bool[] checkedAttrCount = Enumerable
                                                .Range(0, 2)
                                                .Select(attr => checkValue[attr]
                                                                 .Where(v => v == false)
                                                                 .Count() == 1)
                                                .ToArray();
                    isKnownCard = checkedAttrCount[0] && checkedAttrCount[1];
                }
                return isKnownCard;
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

            bool isStopped;
                      
            public HanobiCardGame(List<String> cardDeck)
            {
                this.cardDeck = cardDeck;
                players = new List<List<Card>>(2);
                players.Add(cardDeck
                            .Take(cardsOnOneHands)
                            .Select(str => new Card(str))
                            .ToList());
                players.Add(cardDeck
                            .Skip(cardsOnOneHands)
                            .Take(cardsOnOneHands)
                            .Select(str => new Card(str))
                            .ToList());

                table = Enumerable
                            .Range(0, 5)
                            .Select(x => 0)
                            .ToList();
                currentPlayer = 0;
                currentCard = cardsOnOneHands * 2;
                turn = 0;
                cardsOnTable = 0;
                risk = 0;

                isStopped = false;
            }

            private bool TellAttribute(List<String> command)
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
                    value = Convert.ToInt32(command[2]) - 1;
                }

                List<int> cardIndices = command
                                    .Skip(5)
                                    .Select(str => Convert.ToInt32(str))
                                    .ToList();
                int playerIndex = (currentPlayer + 1) % 2;

                for (int i = 0; i < players[playerIndex].Count; ++i)
                {
                    Card card = players[playerIndex][i];

                    if ((card.value[attribute] == value) == !cardIndices.Contains(i))
                        return false;

                    card.checkValue[attribute][value] = true;
                }

                return true;
            }

            private bool PlayCard(Card card)
            {
                int cardColor = card.value[(int)Attribute.color];
                int cardRank = card.value[(int)Attribute.rank];

                bool continueGame = table[cardColor] == cardRank;
                if (continueGame)
                {
                    ++table[cardColor];

                    if (!card.IsKnownCard())
                        ++risk;

                    ++cardsOnTable;
                    if (cardsOnTable == cardsMaxNumber)
                        continueGame = false;
                }

                return continueGame;
            }

            public bool MakeAction(String command)
            {
                if (isStopped)
                    return true;

                ++turn;
                List<String> commandList = command.Split(' ').ToList();
                bool continueGame = true;

                if (commandList[0] == "Tell")
                    continueGame = TellAttribute(commandList);
                else
                {
                    int cardIndex = Convert.ToInt32(commandList[2]);
                    Card card = players[currentPlayer][cardIndex];

                    if (commandList[0] == "Play")
                        continueGame = PlayCard(card);

                    for (int i = cardIndex; i < cardsOnOneHands - 1; ++i)
                        players[currentPlayer][i] = players[currentPlayer][i + 1];
                    players[currentPlayer][cardsOnOneHands - 1] = new Card(cardDeck[currentCard++]);

                    continueGame = continueGame && (currentCard != cardDeck.Count());
                }

                currentPlayer = (currentPlayer + 1) % 2;

                return continueGame;
            }

            public string GetStatistic()
            {
                return String.Format("Turn: {0}, cards: {1}, with risk: {2}", turn, cardsOnTable, risk);
            }

            public void StopGame()
            {
                isStopped = true;
            }
        }

        static void Main(string[] args)
        {
            string command;
            HanobiCardGame game = null;
            string[] text = System.IO.File.ReadAllLines("1-1.in");
            int i = 0;
            while (true)
            {
                command = text[i++];// Console.ReadLine();
                if (command == null)
                    break;

                if (command.Substring(0, 5) == "Start")
                    game = new HanobiCardGame(command
                                                .Split(' ')
                                                .Skip(5)
                                                .ToList());
                else
                    if (!game.MakeAction(command))
                {
                    Console.WriteLine(game.GetStatistic());
                    game.StopGame();
                }

                if (i == text.Length)
                    break;
            }
        }
    }
}
