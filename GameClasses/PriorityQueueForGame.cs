using System;
using System.Collections;
using System.Collections.Generic;

namespace Windows_Forms_Attempt
{
    public class PriorityQueue
    {
        private class Node
        {
            public item_PU element;
            public int priority;
            public Node Next;

            public Node(item_PU element)
            {
                this.element = element;
                this.priority = element.Get_value();
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

        public void Enqueue(item_PU element) // Add element
        {
            Node newNode = new Node(element);
            if (front == null)
            {
                front = newNode;
                rear = newNode;
            }
            else if (element.Get_value() < front.priority)
            {
                newNode.Next = front;
                front = newNode;
            }
            else
            {
                Node current = front;
                while (current.Next != null && current.Next.priority <= element.Get_value())
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

        public item_PU Dequeue() // Remove element
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

        // Get item by index
        public item_PU GetByIndex(int index)
        {
            Node current = front;
            int currentIndex = 0;

            while (current != null)
            {
                if (currentIndex == index)
                {
                    return current.element;
                }
                current = current.Next;
                currentIndex++;
            }

            return null; // Index out of range
        }

        // Get count of elements
        public int Count()
        {
            Node current = front;
            int count = 0;

            while (current != null)
            {
                count++;
                current = current.Next;
            }

            return count;
        }
    }
}
