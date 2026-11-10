using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace BLACKJACK._7edit
{
    public class RegistroPartida
    {
        public int Numero;
        public string ManoJugador;
        public int TotalJugador;
        public string ManoCrupier;
        public int TotalCrupier;
        public Resultado Resultado;
        public DateTime Fecha;
    }

    /// <summary>Estadísticas de las partidas que se juegan a mano.</summary>
    public class Estadisticas
    {
        private readonly List<RegistroPartida> historial = new List<RegistroPartida>();

        public IList<RegistroPartida> Historial { get { return historial; } }
        public int Partidas { get { return historial.Count; } }
        public int Ganadas { get; private set; }
        public int Perdidas { get; private set; }
        public int Empates { get; private set; }
        public int Blackjacks { get; private set; }

        public double PctGanadas
        {
            get { return Partidas == 0 ? 0 : 100.0 * Ganadas / Partidas; }
        }

        /// <summary>Promedio del total que tuvo el jugador al terminar cada partida.</summary>
        public double PromedioJugador
        {
            get
            {
                if (historial.Count == 0) return 0;
                double suma = 0;
                foreach (RegistroPartida r in historial) suma += r.TotalJugador;
                return suma / historial.Count;
            }
        }

        public void Registrar(Mano jugador, Mano crupier, Resultado resultado)
        {
            RegistroPartida r = new RegistroPartida();
            r.Numero = historial.Count + 1;
            r.ManoJugador = Texto(jugador);
            r.TotalJugador = jugador.Total;
            r.ManoCrupier = Texto(crupier);
            r.TotalCrupier = crupier.Total;
            r.Resultado = resultado;
            r.Fecha = DateTime.Now;
            historial.Add(r);

            if (resultado == Resultado.Gana) Ganadas++;
            else if (resultado == Resultado.Pierde) Perdidas++;
            else Empates++;
            if (jugador.EsBlackjack) Blackjacks++;
        }

        public void Reiniciar()
        {
            historial.Clear();
            Ganadas = Perdidas = Empates = Blackjacks = 0;
        }

        private static string Texto(Mano m)
        {
            List<string> partes = new List<string>();
            foreach (Carta c in m.Cartas) partes.Add(c.ToString());
            return string.Join(" ", partes.ToArray());
        }

        /// <summary>Guarda el historial en CSV (separado por ";" para que Excel en español lo abra bien).</summary>
        public void ExportarCsv(string ruta)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Partida;Mano jugador;Total jugador;Mano crupier;Total crupier;Resultado;Fecha");
            foreach (RegistroPartida r in historial)
            {
                sb.AppendLine(string.Join(";", new string[] {
                    r.Numero.ToString(), r.ManoJugador, r.TotalJugador.ToString(),
                    r.ManoCrupier, r.TotalCrupier.ToString(), r.Resultado.ToString(),
                    r.Fecha.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) }));
            }
            File.WriteAllText(ruta, sb.ToString(), new UTF8Encoding(true));
        }
    }
}
