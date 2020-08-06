using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class ServerCommand
    {
        public static bool TryExecute(string input, ClientObject user)
        {
            string command = input.Split(' ')[0];
            string[] arguments = input.Split(' ');
            switch (command)
            {
                case "!score":
                    try
                    {
                        DBWorking.SetScore(user.userName, Convert.ToInt32(arguments[1]));
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                    
                case "!register":
                    try
                    {
                        if (DBWorking.RegisterAccount(arguments[1], arguments[2]))
                        {
                            user.server.AnswerMessage("!ok", user.Id);
                            Console.WriteLine("Пользователь {0} зарегистрирован", arguments[1]);
                        }
                        else
                        {
                            user.server.AnswerMessage("bad login", user.Id);
                            Console.WriteLine("Неудачная попытка регистрации под логином {0}", arguments[1]);
                        }
                        return true;
                    }
                    catch
                    {
                        user.server.AnswerMessage("bad login", user.Id);
                        return false;
                    }
                case "!getscore":
                    try
                    {
                        int score = DBWorking.GetScore(user.userName);
                        user.server.AnswerMessage("!maxscore " + score.ToString(), user.Id);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                    
                default:
                    return false;
            }
        }
    }
}
