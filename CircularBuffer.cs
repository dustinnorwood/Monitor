using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public class CCircularBuffer<T>
    {
        public T[] List;
        protected int N;
        public int Capacity { get { return N; } }
        protected int m_Start = 0;
        protected int m_End = 0;

        public CCircularBuffer(int n)
        {
            N = n;
            List = new T[N];
        }

        public bool IsEmpty { get { return m_Start == m_End; } }
        public int Count
        {
            get
            {
                lock (List)
                {
                    return (N + m_End - m_Start) & (N - 1);
                }
            }
        }

        public void Push(T t)
        {
            lock (List)
            {
                List[m_End] = t;
                m_End = (m_End + 1) & (N - 1);
                if (m_End == m_Start)
                {
                    m_Start = (m_Start + 1) & (N - 1);
                }
            }
        }

        public T Pop()
        {
            lock (List)
            {
                if (m_Start == m_End)
                    return default(T);
                int i = m_Start;
                m_Start = (m_Start + 1) & (N - 1);
                return List[i];
            }
        }

        public void Clear()
        {
            lock (List)
            {
                m_Start = 0;
                m_End = 0;
            }
        }
    }
}
