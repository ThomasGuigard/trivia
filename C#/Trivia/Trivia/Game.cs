using System;
using System.Collections.Generic;
using System.Linq;

namespace Trivia
{
    public class Game
    {
        private readonly Dictionary<int, string> _categories = new Dictionary<int, string>() {{0, "Pop"}, {1, "Science"}, {2, "Sports"}, {3, "Rock"}};

        private readonly Players _players;
        QuestionsStack QS = new QuestionsStack();

        bool isGettingOutOfPenaltyBox;


        public Game(Players players)
        {
            _players = players;

            QS.MesCategories.Add(new Questions("Pop"));
            QS.MesCategories.Add(new Questions("Science"));
            QS.MesCategories.Add(new Questions("Sport"));
            QS.MesCategories.Add(new Questions("Rock"));

            QS.GenerateQuestion();
        }


        public void Roll(int roll)
        {
            Console.WriteLine(_players.Current.Name + " is the current player");
            Console.WriteLine("They have rolled a " + roll);

            if (_players.Current.InPenaltyBox)
            {
                if (roll % 2 != 0)
                {
                    isGettingOutOfPenaltyBox = true;

                    Console.WriteLine(_players.Current.Name + " is getting out of the penalty box");
                    _players.Current.Move(roll);

                    Console.WriteLine(_players.Current.Name
                            + "'s new location is "
                            + _players.Current.Place);
                    Console.WriteLine("The category is " + CurrentCategory());
                    QS.AskQuestion(CurrentCategory());
                }
                else
                {
                    Console.WriteLine(_players.Current.Name + " is not getting out of the penalty box");
                    isGettingOutOfPenaltyBox = false;
                }

            }
            else
            {
                _players.Current.Move(roll);

                Console.WriteLine(_players.Current.Name
                        + "'s new location is "
                        + _players.Current.Place);
                Console.WriteLine("The category is " + CurrentCategory());
                QS.AskQuestion(CurrentCategory());
            }

        }



        private string CurrentCategory()
        {
            return _categories[_players.Current.Place % 4];
        }

        public bool WasCorrectlyAnswered()
        {
            bool winner;
            if (_players.Current.InPenaltyBox)
            {
                if (isGettingOutOfPenaltyBox)
                {
                    Console.WriteLine("Answer was correct!!!!");
                    _players.Current.WinAGoldCoin();

                    winner = _players.Current.IsWinner();
                    _players.NextPlayer();

                    return winner;
                }

                _players.NextPlayer();
                return false;
            }

            Console.WriteLine("Answer was corrent!!!!");
            _players.Current.WinAGoldCoin();

            winner = _players.Current.IsWinner();
            _players.NextPlayer();

            return winner;
        }

        public bool WrongAnswer()
        {
            Console.WriteLine("Question was incorrectly answered");
            Console.WriteLine(_players.Current.Name + " was sent to the penalty box");
            _players.Current.GoToPenaltyBox();

            _players.NextPlayer();
            return false;
        }
    }
}
