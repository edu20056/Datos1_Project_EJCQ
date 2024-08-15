using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox1;
        private Motorcycle motorcycle;
        private ArrayGrid grid;

        private List<ArrayGrid.Node> nodes;

        public Form1()
        {
            InitializeComponent_1();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Movement);
            CreateGridDisplay();
        }

        private void InitializeComponent_1()
        {
            // Configuración del formulario
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Name = "Form1";
            this.Text = "Trone Game";

            // Inicializar PictureBox
            this.pictureBox1 = new PictureBox();
            this.pictureBox1.Size = new Size(50, 50);
            this.pictureBox1.BackColor = Color.Blue;
            this.Controls.Add(this.pictureBox1);

            // Crear la instancia de Motorcycle
            string[] images_player = new string[]
            {
                "Imagenes/jugador/moto_jugador_derecha.png",
                "Imagenes/jugador/moto_jugador_izquierda.png",
                "Imagenes/jugador/moto_jugador_arriba.png",
                "Imagenes/jugador/moto_jugador_abajo.png"
            };

            this.motorcycle = new Motorcycle(5, 5, 4, images_player, this.pictureBox1, 1);
        }

        private void CreateGridDisplay()
        {
            grid = new ArrayGrid(24, 16);
            nodes = grid.GetNodes();
            int nodeSize = 50; // Tamaño de cada PictureBox

            foreach (ArrayGrid.Node node in nodes)
            {
                PictureBox box = new PictureBox();
                box.Size = new Size(nodeSize, nodeSize);
                box.Location = new Point(node.GetX() * nodeSize, node.GetY() * nodeSize);
                box.BackColor = Color.Red;
                box.BorderStyle = BorderStyle.FixedSingle;
                this.Controls.Add(box);

                node.Box = box; // Asociar el PictureBox con el nodo
            }
        }

        private void Movement(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    this.motorcycle.Move_Image(2);
                    this.motorcycle.Change_Direction(2);
                    this.motorcycle.Change_Position(nodes);
                    break;
                case Keys.Down:
                    this.motorcycle.Move_Image(3);
                    this.motorcycle.Change_Direction(4);
                    this.motorcycle.Change_Position(nodes);
                    break;
                case Keys.Left:
                    this.motorcycle.Move_Image(1);
                    this.motorcycle.Change_Direction(3);
                    this.motorcycle.Change_Position(nodes);
                    break;
                case Keys.Right:
                    this.motorcycle.Move_Image(0);
                    this.motorcycle.Change_Direction(1);
                    this.motorcycle.Change_Position(nodes);
                    break;
            }
        }
    }
}
