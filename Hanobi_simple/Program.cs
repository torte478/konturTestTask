using System;
using System.Collections.Generic;
using System.Linq;


namespace HanabiGame
{
    internal class Program
    {
        private enum Attribute { Color, Rank };
        private const string Colors = "RGBYW";

        private class Card
        {
            public readonly int[] Value;
            public List<List<bool>> isCheckedValue;

            public bool[] isKnownValue;

            public Card(string str)
            {
                var color = Colors.IndexOf(str[0]);
                var rank = Convert.ToInt32(str.Substring(1));

                Value = new int[2];
                Value[(int)Attribute.Color] = color;
                Value[(int)Attribute.Rank] = rank - 1;

                isCheckedValue = Enumerable
                                .Range(0, 2)
                                .Select(i => Enumerable.Range(0, 5)
                                                        .Select(j => false)
                                                        .ToList())
                                .ToList();

                isKnownValue = Enumerable
                                .Range(0, 2)
                                .Select(x => false)
                                .ToArray();
            }

            public void CallAttribute(int attribute)
            {
                isCheckedValue[attribute][Value[attribute]] = true;
                isKnownValue[attribute] = true;
            }

            public void CheckAttribute(int attribute, int value)
            {
                isCheckedValue[attribute][value] = true;
                if (!isKnownValue[attribute] && isCheckedValue[attribute]
                                                    .Count(v => v) ==CardsOnOneHands - 1)
                    isKnownValue[attribute] = true;
            }
        }

        private const int CardsOnOneHands = 5;
        private const int CardsMaxNumber = 25;

        private class HanabiCardGame
        {
            private readonly List<List<Card>> _players;
            private readonly List<int> _table;
            private readonly List<string> _cardDeck;
            private int _currentPlayer, _currentCard;

            private int _turn, _cardsOnTable, _risk;

            private bool _isStopped;
                      
            public HanabiCardGame(List<string> cardDeck)
            {
                _cardDeck = cardDeck;
                _players = new List<List<Card>>(2)
                {
                    _cardDeck
                        .Take(CardsOnOneHands)
                        .Select(str => new Card(str))
                        .ToList(),
                    _cardDeck
                        .Skip(CardsOnOneHands)
                        .Take(CardsOnOneHands)
                        .Select(str => new Card(str))
                        .ToList()
                };

                _table = Enumerable
                            .Range(0, 5)
                            .Select(x => 0)
                            .ToList();
                _currentPlayer = 0;
                _currentCard = CardsOnOneHands * 2;
                _turn = 0;
                _cardsOnTable = 0;
                _risk = 0;

                _isStopped = false;
            }

            private bool TellAttribute(IReadOnlyList<string> command)
            {
                int value, attribute;
                if (command[1] == "color")
                {
                    attribute = (int)Attribute.Color;
                    value = Colors.IndexOf(command[2][0]);
                }
                else
                {
                    attribute = (int)Attribute.Rank;
                    value = Convert.ToInt32(command[2]) - 1;
                }

                var cardIndices = command
                                    .Skip(5)
                                    .Select(str => Convert.ToInt32(str))
                                    .ToList();
                var playerIndex = (_currentPlayer + 1) % 2;

                for (var i = 0; i < _players[playerIndex].Count; ++i)
                {
                    var card = _players[playerIndex][i];

                    if ((card.Value[attribute] == value) == !cardIndices.Contains(i))
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
                var isRiskyCard = card.isKnownValue[(int)Attribute.Rank] == false;
                if (isRiskyCard || card.isKnownValue[(int) Attribute.Color])
                    return isRiskyCard;

                var cardRank = card.Value[(int)Attribute.Rank];
                for (var i = 0; i < _table.Count; ++i)
                    if (_table[i] != cardRank && card.isCheckedValue[(int)Attribute.Color][i] == false)
                        isRiskyCard = true;

                return isRiskyCard;
            }

            private bool PlayCard(Card card)
            {
                var cardColor = card.Value[(int)Attribute.Color];
                var cardRank = card.Value[(int)Attribute.Rank];

                var continueGame = _table[cardColor] == cardRank;

                if (!continueGame)
                    return false;

                if (IsRiskyCard(card))
                    ++_risk;

                ++_table[cardColor];
                ++_cardsOnTable;

                if (_cardsOnTable == CardsMaxNumber)
                    continueGame = false;

                return continueGame;
            }

            public bool MakeAction(string command)
            {
                if (_isStopped)
                    return true;

                ++_turn;
                var commandList = command.Split(' ').ToList();
                var continueGame = true;

                if (commandList[0] == "Tell")
                    continueGame = TellAttribute(commandList);
                else
                {
                    var cardIndex = Convert.ToInt32(commandList[2]);
                    var card = _players[_currentPlayer][cardIndex];

                    if (commandList[0] == "Play")
                        continueGame = PlayCard(card);

                    for (var i = cardIndex; i < CardsOnOneHands - 1; ++i)
                        _players[_currentPlayer][i] = _players[_currentPlayer][i + 1];
                    _players[_currentPlayer][CardsOnOneHands - 1] = new Card(_cardDeck[_currentCard++]);

                    continueGame = continueGame && (_currentCard != _cardDeck.Count());
                }

                _currentPlayer = (_currentPlayer + 1) % 2;
                if (continueGame == false)
                    _isStopped = true;

                return continueGame;
            }

            public string GetStatistic()
            {
                return $"Turn: {_turn}, cards: {_cardsOnTable}, with risk: {_risk}";
            }
        }

        private static void Main()
            {
            HanabiCardGame game = null;
            while (true)
            {
                var command = Console.ReadLine();
                if (command == null)
                    break;

                if (command.Substring(0, 5) == "Start")
                    game = new HanabiCardGame(command
                        .Split(' ')
                        .Skip(5)
                        .ToList());
                else
                {
                    var isFinalCommand = (game != null) && !game.MakeAction(command);
                    if (isFinalCommand)
                        Console.WriteLine(game.GetStatistic());
                }
            }
        }
    }
}
