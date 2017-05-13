using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonadicCSharp;

namespace Monitor
{
    public delegate APIResult APIAction(Maybe<string> command);

    public class APICommand
    {
        private readonly string m_Command;
        public string Command { get { return m_Command; } }

        private readonly APIAction m_Action;
        public APIAction Action { get { return m_Action; } }

        public APICommand(string command, APIAction action)
        {
            m_Command = command;
            m_Action = action;
        }
    }
}
