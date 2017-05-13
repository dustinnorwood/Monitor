using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonadicCSharp;

namespace Monitor
{
    public class MonitorSettings
    {
        private Dictionary<string, Maybe<object>> m_Settings;

        public Maybe<object> this[string key]
        {
            get
            {
                Maybe<object> o;
                if (m_Settings.TryGetValue(key, out o))
                    return o;
                return new Maybe<object>();
            }
            set
            {
                if (m_Settings.ContainsKey(key))
                    m_Settings[key] = value;
                else m_Settings.Add(key, value);
            }
        }

        public MonitorSettings Add(string key, object o)
        {
            this[key] = o == null ? new Maybe<object>() : new Maybe<object>(o);
            return this;
        }

        public MonitorSettings(params Tuple<string, Maybe<object>>[] settings)
        {
            m_Settings = new Dictionary<string, Maybe<object>>();
            settings.ForEach(t => m_Settings.Add(t.Item1, t.Item2));
        }
    }
}
