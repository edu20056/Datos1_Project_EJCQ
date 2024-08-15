using System;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox1;
        private Motorcycle motorcycle;

        public Form1()
        {
            InitializeComponent_1();
            // Configurar el formulario para capturar eventos de teclado
            this.KeyPreview = true;

            // Suscribirse al evento KeyDown
            this.KeyDown += new KeyEventHandler(Movement);
        }

        private void Movement(object sender, KeyEventArgs e)
        {
            // Verificar qué tecla de flecha fue presionada
            switch (e.KeyCode)
            {
                case Keys.Up:
                    this.motorcycle.Move(2);
                    break;
                case Keys.Down:
                    this.motorcycle.Move(3);
                    break;
                case Keys.Left:
                    this.motorcycle.Move(1);
                    break;
                case Keys.Right:
                    this.motorcycle.Move(0);
                    break;
            }
        }

        private void InitializeComponent_1()
        {
            // Inicializar PictureBox
            this.pictureBox1 = new PictureBox();
            this.Controls.Add(this.pictureBox1);

            // Crear la instancia de Motorcycle
            string[] images_player = new string[]
            {
                "Imagenes/jugador/moto_jugador_derecha.png",
                "Imagenes/jugador/moto_jugador_izquierda.png",
                "Imagenes/jugador/moto_jugador_arriba.png",
                "Imagenes/jugador/moto_jugador_abajo.png"
            };

            this.motorcycle = new Motorcycle(5, 5, 4, images_player, this.pictureBox1);


            // Configuración del formulario
            this.ClientSize = new System.Drawing.Size(1400, 800); //1200 pixeles dedicated to the large and 200 extra to show items and powerups info
            //tallness of 800 dedicated to game.
            this.Name = "Form1";
            this.Text = "Mi Proyecto WinForms";
        }

    }
}
