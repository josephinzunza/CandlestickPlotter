using System.Collections;
using System.Numerics;

namespace CandleStickPlotter.DataTypes
{
    public class DequeNode<T>
    {
        public T Value { get; set; }
        public DequeNode<T>? Previous { get; set; }
        public DequeNode<T>? Next { get; set; }
        public DequeNode(T value)
        {
            Value = value;
            Previous = null;
            Next = null;
        }
        public DequeNode(T value, DequeNode<T>? previous, DequeNode<T>? next)
        {
            Value = value;
            Previous = previous;
            Next = next;
        }
    }
    public class Deque<T> : IEnumerable<T>
    {
        private DequeNode<T>? RearPointer { get; set; } = null;
        private DequeNode<T>? FrontPointer { get; set; } = null;
        public T? Rear 
        {
            get
            {
                if (RearPointer == null)
                    throw new InvalidOperationException("Deque is empty.");
                return RearPointer.Value;
            }
        }
        public T? Front
        {
            get
            {
                if (FrontPointer == null)
                    throw new InvalidOperationException("Deque is empty.");
                return FrontPointer.Value;
            }
        }
        public int Count { get; private set; }
        public Deque() { }
        public void PushBack(T value)
        {
            DequeNode<T> newNode = new(value, null, RearPointer);
            if (RearPointer == null)
                FrontPointer = newNode;
            else
                RearPointer.Previous = newNode;
            RearPointer = newNode;
            Count++;
        }
        public void PushFront(T value)
        {
            DequeNode<T> newNode = new(value, FrontPointer, null);
            if (FrontPointer == null)
                RearPointer = newNode;
            else
                FrontPointer.Next = newNode;
            FrontPointer = newNode;
            Count++;
        }
        public T? PopBack()
        {
            if (RearPointer == null) throw new InvalidOperationException("Deque is empty.");
            T? returnValue = RearPointer.Value;
            DequeNode<T> next = RearPointer.Next!;
            if (RearPointer == FrontPointer)
                FrontPointer = null;
            else
            {
                next.Previous = null;
                RearPointer.Next = null;
            }
            RearPointer = next;
            Count--;
            return returnValue;
        }
        public T? PopFront()
        {
            if (FrontPointer == null) throw new InvalidOperationException("Deque is empty.");
            T? returnValue = FrontPointer.Value;
            DequeNode<T>? previous = FrontPointer.Previous;
            if (RearPointer == FrontPointer)
                RearPointer = null;
            else
            {
                previous!.Next = null;
                FrontPointer.Previous = null;
            }
            FrontPointer = previous;
            Count--;
            return returnValue;
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            DequeNode<T>? p = FrontPointer!.Previous;
            DequeNode<T>? q = FrontPointer;
            DequeNode<T>? r = null;
            for (int i = 0; i != index; i++)
            {
                r = q;
                q = p;
                p = p!.Previous;
            }
            if (p != null)
                p.Next = r;
            if (r != null)
                r.Previous = p;
            q!.Next = null;
            q.Previous = null;
            Count--;
        }
        public bool MoveUp(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            bool isNotFirstIndex = index > 0;
            if (isNotFirstIndex) // It's not the top index
            {
                DequeNode<T> p = FrontPointer!;
                for (int i = 0; i < index - 1; i++)
                    p = p.Next!;
                DequeNode<T> q = p.Next!;
                (p.Value, q.Value) = (q.Value, p.Value);
            }
            return isNotFirstIndex;
        }
        public bool MoveDown(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            bool isNotLastIndex = index < Count - 1;
            if (isNotLastIndex) // It's not the bottom index
            {
                DequeNode<T> p = FrontPointer!;
                for (int i = 0; i < index; i++)
                    p = p.Next!;
                DequeNode<T> q = p.Next!;
                (p.Value, q.Value) = (q.Value, p.Value);
            }
            return isNotLastIndex;
        }
        public IEnumerator<T> GetEnumerator()
        {
            DequeNode<T>? current = FrontPointer;
            while (current != null)
            {
                yield return current.Value;
                current = current.Previous;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
