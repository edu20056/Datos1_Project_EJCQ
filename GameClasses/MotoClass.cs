using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public class Motorcycle
    {
        private int speed;
        private int stels;
        private int fuel;
        private PictureBox box;
        private string[] list_images;

        private int x;
        private int y;
        private int move_indicator; // This variable tells if the motorcycle must move up, down, right or left.

        public Motorcycle(int speed, int stels, int fuel, string[] list_images, PictureBox box, int move_indicator)
        {
            this.speed = speed;
            this.stels = stels;
            this.fuel = fuel;
            this.list_images = list_images;
            this.box = box;
            this.move_indicator = move_indicator;
            this.x = 0;
            this.y = 0;
            Create_Box(); // Configure the PictureBox inside the Motorcycle class
        }

        public int Get_speed()
        {
            return this.speed;
        }

        public void Create_Box()
        {
            if (this.box != null)
            {
                this.box.Location = new System.Drawing.Point(x * 50, y * 50);
                this.box.Name = "Player1";
                this.box.Size = new System.Drawing.Size(50, 50);
                this.box.SizeMode = PictureBoxSizeMode.StretchImage;
                this.box.Image = System.Drawing.Image.FromFile(list_images[0]);
                this.box.BackColor = Color.Transparent; // Ensure no background color
            }
        }

        public void Move_Image(int index_image)
        {
            string new_image = list_images[index_image];
            this.box.Image = System.Drawing.Image.FromFile(new_image);
        }

        public void Change_Direction(int new_indicator)
        {
            if (new_indicator <= 4)
            {
                this.move_indicator = new_indicator;
            }
        }

        public void Change_Position(List<ArrayGrid.Node> list)
        {
            // Buscar el nodo actual de la motocicleta en la lista de nodos
            ArrayGrid.Node current = list.Find(node => node.GetX() == this.x && node.GetY() == this.y);

            if (current == null)
            {
                // Si no se encuentra el nodo actual, salir del método
                Console.WriteLine("No se encontró el nodo actual.");
                return;
            }

            // Verificar la dirección de movimiento
            if (this.move_indicator == 1) // Mover hacia la derecha
            {
                current = current.right;

                if (current != null)
                {
                    // Actualizar la posición de la motocicleta
                    this.x = current.GetX();
                    this.y = current.GetY();
                    this.box.Location = new System.Drawing.Point(current.GetX() * 50, current.GetY() * 50);
                }
            }
            else if (this.move_indicator == 2) // Mover hacia arriba
            {
                current = current.up;

                if (current != null)
                {
                    // Actualizar la posición de la motocicleta
                    this.x = current.GetX();
                    this.y = current.GetY();
                    this.box.Location = new System.Drawing.Point(current.GetX() * 50, current.GetY() * 50);
                }
            }
            else if (this.move_indicator == 3) // Mover hacia la izquierda
            {
                current = current.left;

                if (current != null)
                {
                    // Actualizar la posición de la motocicleta
                    this.x = current.GetX();
                    this.y = current.GetY();
                    this.box.Location = new System.Drawing.Point(current.GetX() * 50, current.GetY() * 50);
                }
            }
            else if (this.move_indicator == 4) // Mover hacia abajo
            {
                current = current.down;

                if (current != null)
                {
                    // Actualizar la posición de la motocicleta
                    this.x = current.GetX();
                    this.y = current.GetY();
                    this.box.Location = new System.Drawing.Point(current.GetX() * 50, current.GetY() * 50);
                }
            }
        }
    }
}
