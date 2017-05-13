using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonadicCSharp;

namespace Monitor
{
    public class APIResult
    {
        private readonly APIState m_State;
        public APIState State { get { return m_State; } }

        private readonly Maybe<string> m_Data;
        public Maybe<string> Data { get { return m_Data; } }

        public APIResult(APIState state, Maybe<string> data)
        {
            m_State = state;
            m_Data = data;
        }
    }
}
