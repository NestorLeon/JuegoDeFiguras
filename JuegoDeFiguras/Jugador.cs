using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FigurasGDI;

namespace JuegoDeFiguras
{
    public class Jugador : Figura
    {
        
        public Jugador(double ancho, double alto, double x, double y, Color color) : 
            base(ancho, alto, x, y, color, FormaTypes.RectanguloHorizontal)
        {
            
        }

        public void ActualizarY(int altoDibujo)
        {
            this.Y = altoDibujo - this.Alto;
        }

        public override void Dibujar(ref Graphics context)
        {
            context.FillRectangle(new SolidBrush(this.Color),
                new Rectangle((int)this.X, (int)this.Y, (int)this.Ancho, (int)this.Alto));
        }
    }
}
