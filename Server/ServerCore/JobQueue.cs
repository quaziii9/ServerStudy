using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }

    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();

        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock (_lock)
            {
                _jobQueue.Enqueue(job);

                if (_flush == false)
                {
                    flush = _flush = true;
                }
            }

            if (flush)
            {
                Flush();
            }
        }

        void Flush()
        {
            while (true)
            {
                // 저장된 일감을 모두 가지고 와서 실행
                Action action = Pop();
                if (action == null)
                {
                    return;
                }
                action.Invoke();
            }
        }

        Action Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0)
                {
                    // 
                    // 다른 쓰레드가 진입할 수 있도록 오픈
                    _flush = false;
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }

    
}
