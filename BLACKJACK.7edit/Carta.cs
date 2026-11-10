using System;

namespace BLACKJACK._7edit
{
    public enum Palo { Corazones, Diamantes, Treboles, Picas }

    /// <summary>Una carta de la baraja francesa (52 cartas).</summary>
    public class Carta
    {
        public Palo Palo { get; private set; }

        /// <summary>1 = As, 2-10 = número, 11 = J, 12 = Q, 13 = K.</summary>
        public int Rango { get; private set; }

        public Carta(Palo palo, int rango)
        {
            Palo = palo;
            Rango = rango;
        }

        /// <summary>Valor en blackjack: el As vale 11 (la Mano lo baja a 1 si hace falta), las figuras 10.</summary>
        public int Valor
        {
            get
            {
                if (Rango == 1) return 11;
                if (Rango >= 10) return 10;
                return Rango;
            }
        }

        public string NombreRango
        {
            get
            {
                switch (Rango)
                {
                    case 1: return "A";
                    case 11: return "J";
                    case 12: return "Q";
                    case 13: return "K";
                    default: return Rango.ToString();
                }
            }
        }

        public string Simbolo
        {
            get
            {
                switch (Palo)
                {
                    case Palo.Corazones: return "\u2665";
                    case Palo.Diamantes: return "\u2666";
                    case Palo.Treboles: return "\u2663";
                    default: return "\u2660";
                }
            }
        }

        public bool EsRoja
        {
            get { return Palo == Palo.Corazones || Palo == Palo.Diamantes; }
        }

        public override string ToString()
        {
            return NombreRango + Simbolo;
        }
    }
}
