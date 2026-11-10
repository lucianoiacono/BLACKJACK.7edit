using System;
using System.Collections.Generic;

namespace BLACKJACK._7edit
{
    public enum Resultado { Gana, Pierde, Empata }

    /// <summary>Resultados acumulados de simular muchas partidas con una misma estrategia.</summary>
    public class ResultadoEstrategia
    {
        public int Umbral;      // el jugador pide carta mientras tenga menos que esto
        public int Partidas;
        public int Ganadas;
        public int Perdidas;
        public int Empates;
        public int Blackjacks;  // blackjacks naturales del jugador
        public int Pasadas;     // veces que el jugador se pasó de 21

        public double PctGanadas { get { return Partidas == 0 ? 0 : 100.0 * Ganadas / Partidas; } }
        public double PctPerdidas { get { return Partidas == 0 ? 0 : 100.0 * Perdidas / Partidas; } }
        public double PctEmpates { get { return Partidas == 0 ? 0 : 100.0 * Empates / Partidas; } }
        public double PctPasadas { get { return Partidas == 0 ? 0 : 100.0 * Pasadas / Partidas; } }

        /// <summary>Margen de error (en puntos porcentuales) del % de ganadas, con 95% de confianza.</summary>
        public double MargenError95
        {
            get
            {
                if (Partidas == 0) return 0;
                double p = (double)Ganadas / Partidas;
                return 100.0 * 1.96 * Math.Sqrt(p * (1 - p) / Partidas);
            }
        }
    }

    /// <summary>Reglas del juego y simulación. Las usa tanto la pantalla de juego como la de estadística.</summary>
    public static class Simulador
    {
        public const int PlantarseCrupier = 17; // el crupier pide hasta llegar a 17 o más

        /// <summary>El crupier pide cartas mientras tenga menos de 17.</summary>
        public static void JugarCrupier(Mazo mazo, Mano crupier)
        {
            while (crupier.Total < PlantarseCrupier)
                crupier.Agregar(mazo.Robar());
        }

        /// <summary>Decide quién gana. Se llama con la mano del crupier ya completa.</summary>
        public static Resultado Resolver(Mano jugador, Mano crupier)
        {
            if (jugador.EsBlackjack && crupier.EsBlackjack) return Resultado.Empata;
            if (jugador.EsBlackjack) return Resultado.Gana;
            if (crupier.EsBlackjack) return Resultado.Pierde;
            if (jugador.SePaso) return Resultado.Pierde;
            if (crupier.SePaso) return Resultado.Gana;
            if (jugador.Total > crupier.Total) return Resultado.Gana;
            if (jugador.Total < crupier.Total) return Resultado.Pierde;
            return Resultado.Empata;
        }

        /// <summary>Simula "partidas" manos con la estrategia "pedir mientras tenga menos de umbral".</summary>
        public static ResultadoEstrategia Simular(int umbral, int partidas, Random rnd)
        {
            ResultadoEstrategia res = new ResultadoEstrategia();
            res.Umbral = umbral;
            Mazo mazo = new Mazo(rnd);
            Mano jugador = new Mano();
            Mano crupier = new Mano();

            for (int i = 0; i < partidas; i++)
            {
                mazo.Reiniciar(); // cada partida arranca con un mazo nuevo y mezclado
                jugador.Vaciar();
                crupier.Vaciar();
                jugador.Agregar(mazo.Robar());
                crupier.Agregar(mazo.Robar());
                jugador.Agregar(mazo.Robar());
                crupier.Agregar(mazo.Robar());

                if (!jugador.EsBlackjack && !crupier.EsBlackjack)
                {
                    while (jugador.Total < umbral)
                        jugador.Agregar(mazo.Robar());
                    if (!jugador.SePaso)
                        JugarCrupier(mazo, crupier);
                }

                res.Partidas++;
                if (jugador.EsBlackjack) res.Blackjacks++;
                if (jugador.SePaso) res.Pasadas++;
                switch (Resolver(jugador, crupier))
                {
                    case Resultado.Gana: res.Ganadas++; break;
                    case Resultado.Pierde: res.Perdidas++; break;
                    default: res.Empates++; break;
                }
            }
            return res;
        }

        /// <summary>Simula varias estrategias (umbral desde..hasta) con la misma cantidad de partidas.</summary>
        public static List<ResultadoEstrategia> SimularRango(int desde, int hasta, int partidas, Random rnd)
        {
            List<ResultadoEstrategia> lista = new List<ResultadoEstrategia>();
            for (int u = desde; u <= hasta; u++)
                lista.Add(Simular(u, partidas, rnd));
            return lista;
        }
    }
}
