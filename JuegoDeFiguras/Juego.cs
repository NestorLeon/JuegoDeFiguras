using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FigurasGDI;

namespace JuegoDeFiguras
{
    public class Juego
    {
        #region PROPIEDADES
        public List<Figura> Figuras { get; set; }
        public Jugador Jugador { get; set; }
        private int Tiempo { get; set; }
        public int AnchoDibujo { get; set; }
        public int AltoDibujo { get; set; }
        private int Contador { get; set; }
        #endregion

        #region VARIABLES
        HashSet<GameKeys> keysPressed;
        #endregion

        #region CONSTRUCTOR
        public Juego(int ancho, int alto)
        {
            this.Figuras = new List<Figura>();
            this.Figuras.Add(
                new Pelota(50, 50, 10, 10, Color.Blue)
                );
            this.Figuras.Add(
                new LogoMasones(100, 80, 100, 0, Color.Red)
                );
            this.Jugador = new Jugador(40, 20, ancho / 2, alto - 20, Color.Red);
            this.AnchoDibujo = ancho;
            this.AltoDibujo = alto;
            this.Tiempo = 0;
            this.keysPressed = new HashSet<GameKeys>();
            this.Contador = 0;
        }
        #endregion

        #region MÉTODOS
        public void Actualizar(int tiempo)
        {
            try
            {
                this.Tiempo = tiempo;

                int valorDeMovimientoEnX = 10;
                int valorDeMovimientoEnY = 2;

                if (IsKeyPressed(GameKeys.Left))
                    this.Jugador.X -= valorDeMovimientoEnX;
                if (IsKeyPressed(GameKeys.Right))
                    this.Jugador.X += valorDeMovimientoEnX;

               
                #region DETECCIÓN DE COLISIONES
                double Dx = 0, Dy = 0;
                List<Figura> figurasAEliminar = new List<Figura>();                

                foreach(Figura figura in this.Figuras)
                {
                    // DETERMINAR QUIEN ESTA A LA IZQUIERDA Y DERECHA
                    if(figura.X <= this.Jugador.X)
                        Dx = Math.Abs(figura.X - (this.Jugador.X + this.Jugador.Ancho));
                    else
                        Dx = Math.Abs(this.Jugador.X - (figura.X + figura.Ancho));

                    Dy = Math.Abs(figura.Y - (this.Jugador.Y + this.Jugador.Alto));

                    if (Dx < figura.Ancho + this.Jugador.Ancho &&
                        Dy < figura.Alto + this.Jugador.Alto)
                    { // HAY COLISIÓN
                        figurasAEliminar.Add(figura);
                        this.Contador++;
                    }
                }
                // ELIMINAR FIGURAS ATRAPADAS
                foreach (Figura figuraAEliminar in figurasAEliminar)
                {
                    this.Figuras.Remove(figuraAEliminar);
                }
                #endregion

                // ELIMINAR FIGURAS QUE DESAPARECEN POR DEBAJO
                figurasAEliminar = new List<Figura>();
                foreach (Figura figura in this.Figuras)
                {
                    figura.Y += valorDeMovimientoEnY;
                    if (figura.Y > this.AltoDibujo)
                        figurasAEliminar.Add(figura);
                }
                foreach(Figura figuraAEliminar in figurasAEliminar)
                {
                    this.Figuras.Remove(figuraAEliminar);
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {

            }
        }
        public Bitmap Dibujar()
        {
            Bitmap imagen = new Bitmap(this.AnchoDibujo, this.AltoDibujo);
            Graphics graphics = Graphics.FromImage(imagen);

            try
            {
                
                // DIBUJAR FIGURAS
                foreach(Figura figura in this.Figuras)
                {
                    figura.Dibujar(ref graphics);
                }
                

                // DIBUJAR EL JUGADOR
                this.Jugador.Dibujar(ref graphics);

                // MOSTRAR EL CONTADOR
                graphics.DrawString(this.Contador.ToString(),
                    new Font(FontFamily.GenericSerif, 12f), 
                    Brushes.Red,
                    5, 2);

                /* // DIBUJO DEL TIEMPO
                graphics.DrawString(this.Tiempo.ToString(),
                    new Font(FontFamily.GenericSansSerif, 12f), 
                    Brushes.Red,
                    5, 
                    this.AltoDibujo / 2);
                    */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return imagen;
        }

        public void KeyDown(KeyEventArgs e)
        {
            keysPressed.Add(GetGameKeyFrom(e));
        }
        public void KeyUp(KeyEventArgs e)
        {
            keysPressed.Remove(GetGameKeyFrom(e));
        }
        private GameKeys GetGameKeyFrom(KeyEventArgs e)
        {
            GameKeys key;
            switch (e.KeyData)
            {
                case Keys.Left:
                    key = GameKeys.Left;
                    break;
                case Keys.Right:
                    key = GameKeys.Right;
                    break;
                case Keys.Escape:
                    key = GameKeys.Escape;
                    break;
                default:
                    key = GameKeys.Unknown;
                    break;
            }
            return key;
        }
        private bool IsKeyPressed(GameKeys key)
        {
            return keysPressed.Contains(key);
        }

        #endregion

    }
}
