using System.Collections.Generic;
using System.Windows.Forms;

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

            public PictureBox Box;

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

        private List<Node> list_nodes;

        public ArrayGrid(int x_size, int y_size)
        {
            list_nodes = new List<Node>();
            Create_Grid(x_size, y_size);
        }

        private void Create_Grid(int x_size, int y_size)
        {
            for (int y = 0; y < y_size; y++)
            {
                for (int x = 0; x < x_size; x++)
                {
                    Node node = new Node(0, x, y);
                    list_nodes.Add(node);
                }
            }

            foreach (Node node in list_nodes)
            {
                node.right = Search_right_node(node.GetX() + 1, node.GetY());
                node.left = Search_left_node(node.GetX() - 1, node.GetY());
                node.up = Search_up_node(node.GetX(), node.GetY() - 1);
                node.down = Search_down_node(node.GetX(), node.GetY() + 1);
            }
        }

        private Node Search_right_node(int x, int y)
        {
            if (x >= 24)
            {
                x = 0;
            }
            return list_nodes.Find(node => node.GetX() == x && node.GetY() == y);
        }

        private Node Search_left_node(int x, int y)
        {
            if (x < 0)
            {
                x = 23;
            }
            return list_nodes.Find(node => node.GetX() == x && node.GetY() == y);
        }

        private Node Search_up_node(int x, int y)
        {
            if (y < 0)
            {
                y = 15;
            }
            return list_nodes.Find(node => node.GetX() == x && node.GetY() == y);
        }

        private Node Search_down_node(int x, int y)
        {
            if (y >= 16)
            {
                y = 0;
            }
            return list_nodes.Find(node => node.GetX() == x && node.GetY() == y);
        }

        public List<Node> GetNodes()
        {
            return list_nodes;
        }
    }
}
