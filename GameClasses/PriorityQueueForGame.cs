namespace Windows_Forms_Attempt
{
    using System;

    public class PriorityQueue
    {
        //used for items
        private class Node
        {
            public item_PU element;
            public int priority;
            public Node Next;

            public Node(item_PU element, int priority)
            {
                this.element = element;
                this.priority = priority;
                Next = null;
            }
        }
        private Node front;
        private Node rear;

        public PriorityQueue()
        {
            front = null;
            rear = null;
        }

        public void Enqueue(item_PU element, int priority)
        {
            Node newNode = new Node(element, priority);
            if (front == null)
            {
                front = newNode;
                rear = newNode;
            }
            else if (priority < front.priority)
            {
                newNode.Next = front;
                front = newNode;
            }
            else
            {
                Node current = front;
                while (current.Next != null && current.Next.priority <= priority)
                {
                    current = current.Next;
                }
                newNode.Next = current.Next;
                current.Next = newNode;
                if (newNode.Next == null)
                {
                    rear = newNode;
                }
            }
        }

        public item_PU Dequeue()
        {
            if (front == null)
            {
                Console.WriteLine("Queue Underflow");
                return null;
            }
            else
            {
                item_PU element = front.element;
                front = front.Next;
                return element;
            }
        }

        public item_PU Front()
        {
            if (front == null)
            {
                Console.WriteLine("Queue Underflow");
                return null;
            }
            else
            {
                return front.element;
            }
        }
    }
}