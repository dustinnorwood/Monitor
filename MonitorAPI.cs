using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonadicCSharp;

namespace Monitor
{
    public class MonitorAPI
    {
        protected List<APICommand> m_Commands;

        public MonitorAPI(int readBufferSize)
        {
            m_Commands = new List<APICommand>();
        }

        public APIResult ProcessCommand(string cmd)
        {
            if (cmd.Length >= 2)
            {
                string sub = cmd.Substring(0, 2);
                IEnumerable<APICommand> list = m_Commands.Given(api => api.Command == sub);
                if (list.Count() > 0)
                {
                    return list.First().Action(cmd.Length > 2 ? new Maybe<string>(cmd.Substring(2)) : new Maybe<string>());
                }
            }

            return new APIResult(APIState.None, new Maybe<string>());
        }

        public void Add(APICommand command)
        {
            m_Commands.Add(command);
        }

        public void Remove(APICommand command)
        {
            m_Commands.Remove(command);
        }

        public void Clear()
        {
            m_Commands.Clear();
        }
    }
}
