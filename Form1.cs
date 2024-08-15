using System;
using System.Windows.Forms;

namespace Windows_Forms_Attempt
{
    public partial class Form1 : Form
    {
        private Button button1;
        private TextBox textBox1;

        private PictureBox pictureBox1;

        public Form1()
        {
            InitializeComponentForm();
        }

        private void InitializeComponentForm()
        {
            this.button1 = new Button();
            this.textBox1 = new TextBox();

            // Configuración del botón
            this.button1.Location = new System.Drawing.Point(100, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 50);
            this.button1.Text = "Haz clic aquí";
            this.button1.Click += new EventHandler(this.button1_Click);


            this.pictureBox1 = new PictureBox();

            // Configuración del PictureBox
            this.pictureBox1.Location = new System.Drawing.Point(50, 50);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Image = Image.FromFile("Imagenes/jugador/moto_jugador_abajo.png"); // Cambia "tuImagen.png" por el nombre de tu archivo de imagen


            // Configuración del cuadro de texto
            this.textBox1.Location = new System.Drawing.Point(100, 50);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(200, 20);

            // Agregar controles al formulario
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.pictureBox1);

            // Configuración del formulario
            this.ClientSize = new System.Drawing.Size(800, 800);
            this.Name = "Form1";
            this.Text = "Mi Proyecto WinForms";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("¡Hola, " + textBox1.Text + "!");
        }
    }
}

