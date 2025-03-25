namespace OncallAgent.Utils;

using System.Collections.Generic;

public class ConcurrentPriorityQueue<T>
{
    private readonly PriorityQueue<T, int> _queue = new();
    private readonly object _lock = new();

    public void Enqueue(int priority, T item)
    {
        lock (_lock)
        {
            _queue.Enqueue(item, priority);
        }
    }

    public T Dequeue()
    {
        lock (_lock)
        {
            if (_queue.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            return _queue.Dequeue();
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _queue.Count;
            }
        }
    }

    public bool IsEmpty()
    {
        lock (_lock)
        {
            return _queue.Count == 0;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _queue.Clear();
        }
    }

    public T Peek()
    {
        lock (_lock)
        {
            if (_queue.Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            return _queue.Peek();
        }
    }

}
