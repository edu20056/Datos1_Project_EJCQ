namespace Windows_Forms_Attempt
{
    public class SingleLinkedForGame
    {
        private class Node
        {
            public object Value;
            public Node Next;
            public Node Prev;

            public Node(object value)
            {
                this.Value = value;
                this.Next = null;
                this.Prev = null;
            }
        }

        private Node head;
        private Node tail;
        private int size;

        public SingleLinkedForGame()
        {
            this.head = null;
            this.tail = null;
            this.size = 0;
        }

        public void Clear()
        {
            this.head = null;
            this.tail = null;
            this.size = 0;
        }

        public int Get_Size()
        {
            return this.size;
        }

        public bool Add(object element)
        {
            Node newNode = new Node(element);
            if (this.head == null)
            {
                this.head = newNode;
                this.tail = newNode;
            }
            else
            {
                this.tail.Next = newNode;
                newNode.Prev = this.tail;
                this.tail = newNode;
            }
            this.size++;
            return true;
        }

        public void Remove(object element)
        {
            if (this.head == null)
            {
                throw new InvalidOperationException("Element not found");
            }
            if (this.head.Value == element)
            {
                this.head = this.head.Next;
                if (this.head != null)
                {
                    this.head.Prev = null;
                }
                else
                {
                    this.tail = null;
                }
                this.size--;
                return;
            }
            Node current = this.head;
            while (current.Next != null)
            {
                if (current.Next.Value == element)
                {
                    current.Next = current.Next.Next;
                    if (current.Next != null)
                    {
                        current.Next.Prev = current;
                    }
                    else
                    {
                        this.tail = current;
                    }
                    this.size--;
                    return;
                }
                current = current.Next;
            }
            throw new InvalidOperationException("Element not found");
        }

        public object Get(int index)
        {
            if (index < 0 || index >= this.size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            Node current = this.head;
            for (int i = 0; i < index; i++)
            {
                current = current.Next;
            }
            return current.Value;
        }
    }
}
