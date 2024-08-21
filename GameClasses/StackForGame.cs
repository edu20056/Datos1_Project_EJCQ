namespace Windows_Forms_Attempt
{
    public class ArrayStack
    {
        // Used for powerups
        private item_PU[] stack;
        private int top;
        private int size;

        public ArrayStack(int size)
        {
            this.size = size;
            stack = new item_PU[size];
            top = -1;
        }

        public void Push(item_PU element)
        {
            if (top == size - 1)
            {
                Console.WriteLine("Stack Overflow");
            }
            else
            {
                stack[++top] = element;
            }
        }

        public item_PU Pop()
        {
            if (top == -1)
            {
                Console.WriteLine("Stack Underflow");
                return null;
            }
            else
            {
                return stack[top--];
            }
        }

        public item_PU Top()
        {
            if (top == -1)
            {
                Console.WriteLine("Stack Underflow");
                return null;
            }
            else
            {
                return stack[top];
            }
        }

        // Method to get the count of elements in the stack
        public int Count()
        {
            return top + 1;
        }

        // Method to get the item at a specific index
        public item_PU GetIndex(int index)
        {
            if (index < 0 || index > top)
            {
                Console.WriteLine("Index out of range");
                return null;
            }
            return stack[index];
        }
    }
}
