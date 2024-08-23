using System;
using Windows_Forms_Attempt;


public class DoubleLinkedList
{
    // Interal Class Node
    private class Node
    {
        public int Value { get; set; }
        public Node Next { get; set; }
        public Node Prev { get; set; }

        public Node(int value)
        {
            this.Value = value;
            this.Next = null;
            this.Prev = null;
        }
    }

    private Node head;
    private int size;

    public DoubleLinkedList()
    {
        this.head = null;
        this.size = 0;
    }

    // Clears the list
    public void Clear()
    {
        this.head = null;
        this.size = 0;
    }

    // Checks if the list is empty
    public bool IsEmpty()
    {
        return this.size == 0;
    }

    // Returns the size of the list
    public int Size()
    {
        return this.size;
    }
    public void PrintIndex(int index)
    {
        if (index > this.size)
        {
            throw new InvalidCastException("Index out of range");
        }

        else
        {
            int actual_index = 0;
            Node current = this.head;
            while (actual_index < index)
            {
                if (actual_index == index)
                {
                    actual_index++;
                    current = current.Next;
                }
            }

            Console.WriteLine("El element presented in the input index is {0}.", current.Value);
        }
    }

    public void PrintList()
    {
        Node current = this.head;
        Console.Write("[");
        while (current != null)
        {
            if (current.Next != null)
            {
                Console.Write(current.Value + ",");
                current = current.Next;
            }
            else 
            {
                Console.Write(current.Value);
                current = current.Next;
            }
            
        }
        Console.Write("]");
        Console.WriteLine(); 
    }   

    // Checks if the list contains a specific element
    public bool Contains(int element)
    {
        Node current = this.head;
        while (current != null)
        {
            if (current.Value == element)
            {
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    // Adds an element to the list
    public bool Add(int element)
    {
        Node newNode = new Node(element);
        if (this.head == null)
        {
            this.head = newNode;
        }
        else
        {
            Node current = this.head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
            newNode.Prev = current;
        }
        this.size++;
        return true;
    }

    // Removes an element from the list
    public int Remove(int element)
    {
        if (this.head == null)
        {
            throw new InvalidOperationException("Element not found");
        }
        if (this.head.Value == element)
        {
            int removedValue = this.head.Value;
            this.head = this.head.Next;
            if (this.head != null)
            {
                this.head.Prev = null;
            }
            this.size--;
            return removedValue;
        }
        Node current = this.head;
        while (current.Next != null)
        {
            if (current.Next.Value == element)
            {
                int removedValue = current.Next.Value;
                current.Next = current.Next.Next;
                if (current.Next != null)
                {
                    current.Next.Prev = current;
                }
                this.size--;
                return removedValue;
            }
            current = current.Next;
        }
        throw new InvalidOperationException("Element not found");
    }
}

