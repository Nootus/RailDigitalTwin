namespace Rail.DigitalTwin.Core.Utilities
{
    public delegate void EventQueueHandler<T>(T sender);
    public class EventQueue<T>
    {
        private static readonly EventQueue<T> instance = new EventQueue<T>();
        public static EventQueue<T> Instance { get { return instance; } }

        private readonly Queue<T> queue = new Queue<T>();
        
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public event EventQueueHandler<T> Enqueued;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected virtual void OnEnqueued(T item)
        {
            if (Enqueued != null)
                Enqueued(item);
        }

        public virtual void Enqueue(T item)
        {
            queue.Enqueue(item);
            OnEnqueued(item);
        }

        public int Count
        {
            get
            {
                return queue.Count;
            }
        }

        public virtual T Dequeue()
        {
            T item = queue.Dequeue();
            return item;
        }
    }
}
