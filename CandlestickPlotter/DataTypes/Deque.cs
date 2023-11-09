using System.Numerics;

namespace CandleStickPlotter.DataTypes
{
    internal class Node<T> where T : struct, INumber<T>
    {
        public T Value { get; set; }
        public Node<T>? Previous { get; set; }
        public Node<T>? Next { get; set; }
        public Node()
        {
            Value = default;
            Previous = null;
            Next = null;
        }
        public Node(T value)
        {
            Value = value;
            Previous = null;
            Next = null;
        }
        public Node(T value, Node<T>? previous, Node<T>? next)
        {
            Value = value;
            Previous = previous;
            Next = next;
        }
    }
    internal class Deque<T> where T: struct, INumber<T>
    {
        private Node<T>? TailPointer { get; set; } = null;
        private Node<T>? HeadPointer { get; set; } = null;
        public T Tail 
        {
            get
            {
                if (TailPointer == null)
                    throw new InvalidOperationException("Deque is empty.");
                return TailPointer.Value;
            }
        }
        public T Head
        {
            get
            {
                if (HeadPointer == null)
                    throw new InvalidOperationException("Deque is empty.");
                return HeadPointer.Value;
            }
        }
        public int Count { get; set; }
        public Deque() { }
        public void PushBack(T value)
        {
            Node<T> newNode = new(value, null, TailPointer);
            if (TailPointer == null)
                HeadPointer = newNode;
            else
                TailPointer.Previous = newNode;
            TailPointer = newNode;
            Count++;
        }
        public void PushFront(T value)
        {
            Node<T> newNode = new(value, HeadPointer, null);
            if (HeadPointer == null)
                TailPointer = newNode;
            else
                HeadPointer.Next = newNode;
            HeadPointer = newNode;
            Count++;
        }
        public T PopBack()
        {
            if (TailPointer == null) throw new InvalidOperationException("Deque is empty.");
            T returnValue = TailPointer.Value;
            Node<T> next = TailPointer.Next!;
            if (TailPointer == HeadPointer)
                HeadPointer = null;
            else
            {
                next.Previous = null;
                TailPointer.Next = null;
            }
            TailPointer = next;
            Count--;
            return returnValue;
        }
        public T PopFront()
        {
            if (HeadPointer == null) throw new InvalidOperationException("Deque is empty.");
            T returnValue = HeadPointer.Value;
            Node<T>? previous = HeadPointer.Previous;
            if (TailPointer == HeadPointer)
                TailPointer = null;
            else
            {
                previous!.Next = null;
                HeadPointer.Previous = null;
            }
            HeadPointer = previous;
            Count--;
            return returnValue;
        }
    }
}
