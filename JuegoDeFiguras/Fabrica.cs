using FigurasGDI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JuegoDeFiguras
{
    public class Fabrica
    {
        #region PROPIEDADES
        public Juego Juego { get; set; }
        public Queue<Figura> ContenedorFiguras { get; set; }
        private int AnchoDibujo { get; set; }
        private int AltoDibujo { get; set; }
        #endregion

        #region VARIABLES
        private Task taskCrearObjetos;
        private Task taskRecolectarObjetos;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private Random randomAncho_Figuras = new Random();
        private Random randomAlto_Figuras = new Random();
        private Random randomX_Figuras = new Random();
        private Random randomColorRGB = new Random();
        private Random randomFiguras = new Random();
        #endregion

        #region CONSTRUCTOR(ES)
        public Fabrica(int anchoDibujo, int altoDibujo)
        {
            this.Juego = new Juego(anchoDibujo, altoDibujo);
            this.AnchoDibujo = anchoDibujo;
            this.AltoDibujo = altoDibujo;
            this.ContenedorFiguras = new Queue<Figura>();
        }
        #endregion

        #region MÉTODOS
        public async void Iniciar()
        {
            this.cancellationToken = new CancellationTokenSource();

            IniciarCreadorDeObjetos();
            IniciarRecolectorDeObjetos();

            await taskCrearObjetos;
            await taskRecolectarObjetos;
        }
        public void Parar()
        {
            if (taskCrearObjetos != null)
                this.cancellationToken.Cancel();
        }
        public void CrearObjetos2()
        {
            var varTodosLosTiposDLL = typeof(Figura).Assembly.GetTypes();
            List<Type> todasLasFiguras = new List<Type>();
            foreach (Type type in varTodosLosTiposDLL)
            {
                if (type.BaseType == typeof(Figura))
                    todasLasFiguras.Add(type);
            }
            
            while (!cancellationToken.IsCancellationRequested)
            {
                Figura figura = null;
                double ancho, alto;

                int w = randomFiguras.Next(0, todasLasFiguras.Count-1);

                Type type = todasLasFiguras[w].GetTypeInfo();
                ConstructorInfo ctor = type.GetConstructor(new[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(Color) });
                if(ctor != null)
                {
                    figura = (Figura)ctor.Invoke(new object[] { 0, 0,
                            randomFiguras.Next(this.AnchoDibujo), 0,
                            SeleccionarColor() });
                    DefineAnchoAlto(figura.Forma, out ancho, out alto);
                    figura.Ancho = ancho;
                    figura.Alto = alto;

                    if (figura != null)
                        this.ContenedorFiguras.Enqueue(figura);
                }
                Thread.Sleep(500);
                figura = null;
            }
        }
        public void RecolectarObjetos2()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (this.ContenedorFiguras.Count > 0)
                {
                    Figura figura = this.ContenedorFiguras.Dequeue();
                    this.Juego.Figuras.Add(figura);
                    figura = null;
                }
                Thread.Sleep(650);
                Console.WriteLine("Objeto recolectado");
            }
        }
        private void DefineAnchoAlto(FormaTypes forma, out double ancho, out double alto)
        {
            ancho = 0; alto = 0;
            switch (forma)
            {
                case FormaTypes.Cuadrado:
                    ancho = randomAncho_Figuras.Next(10, 50);
                    alto = ancho;
                    break;
                case FormaTypes.RectanguloHorizontal:
                    alto = randomAlto_Figuras.Next(10, 50);
                    ancho = randomAncho_Figuras.Next((int)alto + 1, 100);
                    break;
                case FormaTypes.RectanguloVertical:
                    ancho = randomAncho_Figuras.Next(10, 50);
                    alto = randomAlto_Figuras.Next((int)ancho + 1, 100);
                    break;
                default:
                    break;
            }
        }
        private Color SeleccionarColor()
        {
            int valor = randomColorRGB.Next(0, 255);

            int R = randomColorRGB.Next(0, 255);
            int G = randomColorRGB.Next(0, 255);
            int B = randomColorRGB.Next(0, 255);

            return Color.FromArgb(R, G, B);

            /*
            //int valor = new Random().Next(0, 11);
            switch (valor)
            {
                case 0:
                    return Color.Orange;
                case 1:
                    return Color.DarkKhaki;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.Brown;
                case 4:
                    return Color.Turquoise;
                case 5:
                    return Color.DarkGray;
                case 6:
                    return Color.Black;
                case 7:
                    return Color.Green;
                case 8:
                    return Color.Red;
                case 9:
                    return Color.Yellow;
                case 10:
                    return Color.HotPink;
                default:
                    return Color.Purple;
            }
            */
        }
        #endregion

        #region METODOS INICIAR TAREAS ASÍNCRONAS
        private void IniciarRecolectorDeObjetos()
        {
            if (taskRecolectarObjetos == null)
            {
                taskRecolectarObjetos = Task.Factory.StartNew(this.RecolectarObjetos2, cancellationToken.Token)
                                        .ContinueWith(_ => taskRecolectarObjetos = null);
            }
            /*
            var task2 = new Task(() => {
                this.Fabrica.RecolectarObjetos2();
            });
            task2.Start();
            return task2;
            */
        }
        private void IniciarCreadorDeObjetos()
        {
            if (taskCrearObjetos == null)
            {
                taskCrearObjetos = Task.Factory.StartNew(this.CrearObjetos2, cancellationToken.Token)
                                        .ContinueWith(_ => taskCrearObjetos = null);
            }
            /*
            var task1 = new Task(() => {
                this.Fabrica.CrearObjetos2();
            });
            task1.Start();
            return task1;
            */
        }
        #endregion

        #region MÉTODOS ASÍNCRONOS (DESUSO)
        public async Task<int> CrearObjetos()
        {
            var task = new Task<int>(() => {
                int caso = 1;
                while (true)
                {
                    Figura figura = null;
                    switch (caso)
                    {
                        case 1:
                            figura = new Pelota(40, 40,
                                new Random().Next(this.AnchoDibujo), 0,
                                SeleccionarColor());
                            break;
                        case 2:
                            figura = new LogoMasones(70, 70,
                                new Random().Next(this.AnchoDibujo), 0,
                                SeleccionarColor());
                            break;
                        default:
                            break;
                    }
                    if (figura != null)
                        this.ContenedorFiguras.Enqueue(figura);
                    Thread.Sleep(1000);
                    caso += 1;
                    if (caso == 3)
                        caso = 1;
                }

                return 0;
            });
            task.Start();
            int valReturn = await task;
            return valReturn;

        }
        public async Task<int> RecolectarObjetos()
        {
            var task = new Task<int>(() => {
                while (true)
                {
                    if (this.ContenedorFiguras.Count > 0)
                    {
                        Figura figura = this.ContenedorFiguras.Dequeue();
                        this.Juego.Figuras.Add(figura);
                    }
                    Thread.Sleep(650);
                }
                return 0;
            });
            task.Start();
            int valReturn = await task;
            return valReturn;

        }
        #endregion
    }
}
