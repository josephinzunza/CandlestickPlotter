namespace CandleStickPlotter.DataTypes
{
    public class SLLNode<T>
    {
        public T Value { get; set; }
        public SLLNode<T>? Next { get; set; }
        public SLLNode(T value)
        {
            Value = value;
            Next = null;
        }
        public SLLNode(T value, SLLNode<T>? next) : this(value)
        {
            Next = next;
        }
    }
    public class SinglyLinkedList<T>
    {
        private SLLNode<T>? Head { get; set; }
        public int Count { get; private set; }
        public SinglyLinkedList()
        {
            Head = null;
            Count = 0;
        }
        public void Add(T item)
        {
            if (Head != null)
                Head = new SLLNode<T>(item, Head);
            else
                Head = new SLLNode<T>(item);
            Count++;
        }
        public bool MoveDown(int index)
        {
            if (index >= Count || index < 0) throw new IndexOutOfRangeException();
            bool isNotLast = index < Count - 1;
            if (isNotLast) // It's not the last element
            {
                if (index > 0) // It's not the first element
                {
                    SLLNode<T> p = Head!;
                    for (int i = 0; i < index - 1; i++)
                        p = p.Next!;
                    SLLNode<T> q = p.Next!;
                    SLLNode<T> r = q.Next!;
                    p.Next = r;
                    q.Next = r.Next;
                    r.Next = q;
                }
                else // It's the first element
                {
                    SLLNode<T> q = Head!;
                    SLLNode<T> r = q.Next!;
                    q.Next = r.Next;
                    r.Next = q;
                    Head = r;
                }
            }
            return isNotLast;
        }
        public bool MoveUp(int index)
        {
            if (index >= Count || index < 0) throw new IndexOutOfRangeException();
            bool isNotFirst = index > 0;
            if (isNotFirst) // It's not the first element
            {
                if (index < Count - 1) // It's not the last element
                {
                    SLLNode<T> p = Head!;
                    for (int i = 0; i < index - 1; i++)
                        p = p.Next!;
                    SLLNode<T> q = p.Next!;
                    SLLNode<T> r = q.Next!;
                    p.Next = r;
                    q.Next = r.Next;
                    r.Next = q;
                }
                else // It's the last element
                {
                    SLLNode<T> q = Head!;
                    SLLNode<T> r = q.Next!;
                    q.Next = r.Next;
                    r.Next = q;
                    Head = r;
                }
            }
            return isNotFirst;
        }
    }
}
