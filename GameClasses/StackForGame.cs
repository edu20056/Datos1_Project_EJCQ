namespace Windows_Forms_Attempt
{
    public class ArrayStack
    {
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
        

        public int Count()
        {
            return top + 1;
        }

        public item_PU GetIndex(int index)
        {
            if (index < 0 || index > top)
            {
                Console.WriteLine("Index out of range");
                return null;
            }
            return stack[index];
        }

        public void Set_bottom_to_top()
        {
            if (top <= 0) 
                return;

            // Store the last element (bottom)
            item_PU lastElement = stack[top];

            // Moves elements to the right, from position 0 to top - 1
            for (int i = top; i > 0; i--)
            {
                stack[i] = stack[i - 1];
            }
            
            // Place the last element in the head
            stack[0] = lastElement;
        }

    }
}
