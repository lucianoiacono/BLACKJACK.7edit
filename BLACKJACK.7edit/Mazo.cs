using System;

namespace BLACKJACK._7edit
{
    /// <summary>
    /// Mazo de 52 cartas. En vez de mezclar todo de entrada, cada vez que se roba se elige
    /// una carta al azar entre las que quedan (Fisher-Yates "perezoso"): el resultado es
    /// igual de uniforme que mezclar el mazo completo, pero mucho más rápido para simular.
    /// </summary>
    public class Mazo
    {
        private static readonly Carta[] baraja = CrearBaraja();
        private readonly Carta[] cartas = new Carta[52];
        private readonly Random rnd;
        private int restantes;

        public Mazo(Random rnd)
        {
            this.rnd = rnd;
            Reiniciar();
        }

        public int Restantes
        {
            get { return restantes; }
        }

        private static Carta[] CrearBaraja()
        {
            Carta[] b = new Carta[52];
            int i = 0;
            foreach (Palo p in Enum.GetValues(typeof(Palo)))
                for (int rango = 1; rango <= 13; rango++)
                    b[i++] = new Carta(p, rango);
            return b;
        }

        /// <summary>Vuelve a poner las 52 cartas en el mazo.</summary>
        public void Reiniciar()
        {
            Array.Copy(baraja, cartas, 52);
            restantes = 52;
        }

        /// <summary>Saca una carta al azar. Si el mazo se acabó, lo arma de nuevo.</summary>
        public Carta Robar()
        {
            if (restantes == 0) Reiniciar();
            int j = rnd.Next(restantes);
            Carta c = cartas[j];
            cartas[j] = cartas[restantes - 1];
            restantes--;
            return c;
        }
    }
}
