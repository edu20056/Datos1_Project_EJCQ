namespace Windows_Forms_Attempt
{
    public class ArrayGrid
    {
        public class Node 
        {
            public int value;
            public int x_pos;
            public int y_pos;
            public Node up;
            public Node down;
            public Node right;
            public Node left;

            public Node(int value, int x_pos, int y_pos)
            {
                this.value = value;
                this.x_pos = x_pos;
                this.y_pos = y_pos;

                this.down = null;
                this.up = null;
                this.left = null;
                this.right = null;
            }

            public int GetX()
            {
                return this.x_pos;
            }

            public int GetY()
            {
                return this.y_pos;
            }
        }

        private Node head;

        private int x_size = 24;
        private int y_size = 16;
        List<Node> list_nodes = new List<Node>(); 
        public ArrayGrid(Node head, int x_size, int y_size) 
        {
            this.head = head;
            this.x_size = x_size;
            this.y_size = y_size;
        }

        public void Create_Grid()
        {
            int x = 1;
            int y = 0;
            
            //creacion de nodos
            Node head = new Node(0, 0, 0);
            list_nodes.Add(head);
            while (y < y_size)
            {
                while (x < x_size)
                {
                    Node node =  new Node(0, x, y);
                    x++;
                    list_nodes.Add(node);
                }
                x = 0;
                y++;
            }
            //link de nodos
            foreach (Node node in list_nodes)
            {
                node.right = Search_right_node(node.GetX() + 1, node.GetY(), list_nodes );
                node.left = Search_left_node(node.GetX() - 1, node.GetY(), list_nodes );
                node.up = Search_up_node(node.GetX(), node.GetY() - 1, list_nodes );
                node.down = Search_down_node(node.GetX(), node.GetY() + 1, list_nodes );
            }

        }

        private Node Search_right_node(int x, int y, List<Node> lista)
        {
            if (x > 23)
            {
                x = 0;
            }

            foreach (Node node in lista)
            {
                if ( x == node.GetX() && y == node.GetY())
                {
                    return node;
                }
            }
            return null;
        }

        private Node Search_left_node(int x, int y, List<Node> lista)
        {
            if (x < 0)
            {
                x = 23;
            }

            foreach (Node node in lista)
            {
                if ( x == node.GetX() && y == node.GetY())
                {
                    return node;
                }
            }
            return null;
        }

        private Node Search_up_node(int x, int y, List<Node> lista)
        {
            if (y < 0)
            {
                y = 15;
            }

            foreach (Node node in lista)
            {
                if ( x == node.GetX() && y == node.GetY())
                {
                    return node;
                }
            }
            return null;
        }

        private Node Search_down_node(int x, int y, List<Node> lista)
        {
            if (y > 15)
            {
                y = 0;
            }

            foreach (Node node in lista)
            {
                if ( x == node.GetX() && y == node.GetY())
                {
                    return node;
                }
            }
            return null;
        }
    }
}
