using FigurasGDI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JuegoDeFiguras
{
    public partial class Form1 : Form
    {
        private Fabrica Fabrica { get; set; }
        private int tiempo = 0;

        #region CONSTRUCTOR(ES)
        public Form1()
        {
            InitializeComponent();
            this.Fabrica = new Fabrica(pbCanvas.Width, pbCanvas.Height);
        }
        #endregion

        #region EVENTOS KEY DOWN AND UP
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // REINICIAR JUEGO
            if (e.KeyData == Keys.Escape)
            {
                this.Fabrica.Juego = new Juego(pbCanvas.Width, pbCanvas.Height);
                this.tiempo = 0;
            }
            else
                this.Fabrica.Juego.KeyDown(e);
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            this.Fabrica.Juego.KeyUp(e);
        }
        #endregion

        #region EVENTOS FORM
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Fabrica.Juego.Actualizar(tiempo);
            Bitmap imagen = this.Fabrica.Juego.Dibujar();
            pbCanvas.Image = imagen;
            tiempo += timer1.Interval;
            GC.Collect();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.Fabrica.Juego.AnchoDibujo = pbCanvas.Width;
            this.Fabrica.Juego.AltoDibujo = pbCanvas.Height;
            this.Fabrica.Juego.Jugador.ActualizarY(pbCanvas.Height);
        }
        #endregion

        #region CLIK MENU
        private void miJuego_Iniciar_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Start();
                this.Fabrica.Iniciar();
                //await this.Fabrica.CrearObjetos();
                //await this.Fabrica.RecolectarObjetos();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private void miJuego_Parar_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Fabrica.Parar();
        }
        #endregion
    }
}
