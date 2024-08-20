using System;

public class LinkedListPriorityQueue
{
    private class Node
    {
        public int Element { get; set; }
        public int Priority { get; set; }
        public Node Next { get; set; }

        public Node(int element, int priority)
        {
            Element = element;
            Priority = priority;
            Next = null;
        }
    }
    private Node front;
    private Node rear;

    public LinkedListPriorityQueue()
    {
        front = null;
        rear = null;
    }

    public void Enqueue(int element, int priority)
    {
        Node newNode = new Node(element, priority);
        if (front == null)
        {
            front = newNode;
            rear = newNode;
        }
        else if (priority < front.Priority)
        {
            newNode.Next = front;
            front = newNode;
        }
        else
        {
            Node current = front;
            while (current.Next != null && current.Next.Priority <= priority)
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

    public int Dequeue()
    {
        if (front == null)
        {
            Console.WriteLine("Queue Underflow");
            return -1;
        }
        else
        {
            int element = front.Element;
            front = front.Next;
            return element;
        }
    }

    public int Front()
    {
        if (front == null)
        {
            Console.WriteLine("Queue Underflow");
            return -1;
        }
        else
        {
            return front.Element;
        }
    }
}
