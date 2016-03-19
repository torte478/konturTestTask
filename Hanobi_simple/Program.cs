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

            public bool[] knownValue;

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

                knownValue = Enumerable
                                .Range(0, 2)
                                .Select(x => false)
                                .ToArray();
            }

            public void CallAttribute(int attribute)
            {
                checkValue[attribute][value[attribute]] = true;
                knownValue[attribute] = true;
            }

            public void CheckAttribute(int attribute, int value)
            {
                checkValue[attribute][value] = true;
                if (!knownValue[attribute] && checkValue[attribute]
                    .Where(v => v == true)
                    .Count() == cardsOnOneHands - 1)
                    knownValue[attribute] = true;
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

                    if (cardIndices.Contains(i))
                        card.CallAttribute(attribute);
                    else
                        card.CheckAttribute(attribute, value);
                }

                return true;
            }
            
            private bool IsRiskyCard(Card card)
            {
                bool isRiskyCard = card.knownValue[(int)Attribute.rank] == false;
                if (!isRiskyCard && card.knownValue[(int)Attribute.color] == false)
                {
                    int cardRank = card.value[(int)Attribute.rank];
                    for (int i = 0; i < table.Count; ++i)
                        if (table[i] != cardRank && card.checkValue[(int)Attribute.color][i] == false)
                            isRiskyCard = true;
                }

                return isRiskyCard;
            }

            private bool PlayCard(Card card)
            {
                int cardColor = card.value[(int)Attribute.color];
                int cardRank = card.value[(int)Attribute.rank];

                bool continueGame = table[cardColor] == cardRank;
                if (continueGame)
                {
                    if (IsRiskyCard(card))
                        ++risk;

                    ++table[cardColor];
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
                if (continueGame == false)
                    isStopped = true;

                return continueGame;
            }

            public string GetStatistic()
            {
                return String.Format("Turn: {0}, cards: {1}, with risk: {2}", turn, cardsOnTable, risk);
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
                    if (!game.MakeAction(command))
                        Console.WriteLine(game.GetStatistic());
            }
        }
    }
}
