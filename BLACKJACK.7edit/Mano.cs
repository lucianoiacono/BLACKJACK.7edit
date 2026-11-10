using System.Collections.Generic;

namespace BLACKJACK._7edit
{
    /// <summary>Las cartas de un jugador (o del crupier) y su puntaje.</summary>
    public class Mano
    {
        private readonly List<Carta> cartas = new List<Carta>();

        public IList<Carta> Cartas
        {
            get { return cartas; }
        }

        public void Agregar(Carta c)
        {
            cartas.Add(c);
        }

        public void Vaciar()
        {
            cartas.Clear();
        }

        /// <summary>Suma de la mano: cada As vale 11, y baja a 1 mientras la mano se pase de 21.</summary>
        public int Total
        {
            get
            {
                int total = 0, ases = 0;
                foreach (Carta c in cartas)
                {
                    total += c.Valor;
                    if (c.Rango == 1) ases++;
                }
                while (total > 21 && ases > 0)
                {
                    total -= 10;
                    ases--;
                }
                return total;
            }
        }

        public bool EsBlackjack
        {
            get { return cartas.Count == 2 && Total == 21; }
        }

        public bool SePaso
        {
            get { return Total > 21; }
        }
    }
}
