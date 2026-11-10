using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BLACKJACK._7edit
{
    /// <summary>Panel que dibuja las cartas de una mano. Si OcultarSegunda es true, la 2da carta se ve boca abajo.</summary>
    public class PanelCartas : Panel
    {
        private const int Ancho = 70, Alto = 100, Separacion = 80;

        public Mano Mano { get; set; }
        public bool OcultarSegunda { get; set; }

        public PanelCartas()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Mano == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int x = 5;
            for (int i = 0; i < Mano.Cartas.Count; i++)
            {
                DibujarCarta(e.Graphics, Mano.Cartas[i], x, 10, OcultarSegunda && i == 1);
                x += Separacion;
            }
        }

        private static GraphicsPath Redondeado(Rectangle r, int radio)
        {
            int d = radio * 2;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static void DibujarCarta(Graphics g, Carta c, int x, int y, bool bocaAbajo)
        {
            Rectangle r = new Rectangle(x, y, Ancho, Alto);
            using (GraphicsPath path = Redondeado(r, 8))
            {
                if (bocaAbajo)
                {
                    using (Brush b = new SolidBrush(Color.FromArgb(40, 70, 160))) g.FillPath(b, path);
                    using (Pen p = new Pen(Color.White, 2)) g.DrawPath(p, path);
                    Rectangle interior = new Rectangle(x + 8, y + 8, Ancho - 16, Alto - 16);
                    using (Pen p = new Pen(Color.FromArgb(150, 255, 255, 255), 1)) g.DrawRectangle(p, interior);
                    return;
                }

                g.FillPath(Brushes.White, path);
                using (Pen borde = new Pen(Color.FromArgb(120, 120, 120), 1)) g.DrawPath(borde, path);

                Color color = c.EsRoja ? Color.FromArgb(200, 30, 30) : Color.FromArgb(30, 30, 30);
                using (Brush pincel = new SolidBrush(color))
                using (Font fRango = new Font("Segoe UI", 13F, FontStyle.Bold))
                using (Font fChico = new Font("Segoe UI Symbol", 11F))
                using (Font fGrande = new Font("Segoe UI Symbol", 28F))
                using (StringFormat centro = new StringFormat())
                {
                    centro.Alignment = StringAlignment.Center;
                    centro.LineAlignment = StringAlignment.Center;
                    g.DrawString(c.NombreRango, fRango, pincel, x + 5, y + 3);
                    g.DrawString(c.Simbolo, fChico, pincel, x + 5, y + 24);
                    g.DrawString(c.Simbolo, fGrande, pincel, new RectangleF(x, y + 35, Ancho, 60), centro);
                }
            }
        }
    }

    /// <summary>Gráfico de barras: % de partidas ganadas para cada estrategia simulada.</summary>
    public class GraficoBarras : Panel
    {
        public List<ResultadoEstrategia> Datos { get; set; }

        public GraficoBarras()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Font fTitulo = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Font fEje = new Font("Segoe UI", 8.5F))
            using (StringFormat centro = new StringFormat())
            {
                centro.Alignment = StringAlignment.Center;
                g.DrawString("% de partidas ganadas según cuándo se planta el jugador", fTitulo, Brushes.Black, 10, 6);

                if (Datos == null || Datos.Count == 0)
                {
                    g.DrawString("Apretá \"Simular estrategias\" para ver el gráfico.", fEje, Brushes.Gray, 10, 40);
                    return;
                }

                int izq = 50, der = 15, arriba = 35, abajo = 45;
                Rectangle area = new Rectangle(izq, arriba, Width - izq - der, Height - arriba - abajo);

                double max = 0, mejor = -1;
                int idxMejor = 0;
                for (int i = 0; i < Datos.Count; i++)
                {
                    if (Datos[i].PctGanadas > max) max = Datos[i].PctGanadas;
                    if (Datos[i].PctGanadas > mejor) { mejor = Datos[i].PctGanadas; idxMejor = i; }
                }
                double techo = Math.Ceiling((max + 2) / 10.0) * 10.0;

                // grilla y eje Y
                using (Pen grilla = new Pen(Color.FromArgb(225, 225, 225)))
                {
                    for (double v = 0; v <= techo + 0.001; v += 10)
                    {
                        float y = area.Bottom - (float)(v / techo * area.Height);
                        g.DrawLine(grilla, area.Left, y, area.Right, y);
                        g.DrawString(v.ToString("0") + "%", fEje, Brushes.Gray, 4, y - 8);
                    }
                }

                // barras
                float paso = (float)area.Width / Datos.Count;
                float anchoBarra = paso * 0.6f;
                using (Brush azul = new SolidBrush(Color.FromArgb(90, 130, 190)))
                using (Brush verde = new SolidBrush(Color.FromArgb(40, 150, 90)))
                using (Pen error = new Pen(Color.Black, 1.5f))
                {
                    for (int i = 0; i < Datos.Count; i++)
                    {
                        ResultadoEstrategia d = Datos[i];
                        float h = (float)(d.PctGanadas / techo * area.Height);
                        float x = area.Left + i * paso + (paso - anchoBarra) / 2;
                        float y = area.Bottom - h;
                        g.FillRectangle(i == idxMejor ? verde : azul, x, y, anchoBarra, h);

                        // barra de error (intervalo de confianza 95%)
                        float cx = x + anchoBarra / 2;
                        float dy = (float)(d.MargenError95 / techo * area.Height);
                        g.DrawLine(error, cx, y - dy, cx, y + dy);

                        g.DrawString(d.PctGanadas.ToString("0.0"), fEje, Brushes.Black,
                            new RectangleF(x - 10, y - dy - 17, anchoBarra + 20, 16), centro);
                        g.DrawString(d.Umbral.ToString(), fEje, Brushes.Black,
                            new RectangleF(x - 10, area.Bottom + 4, anchoBarra + 20, 16), centro);
                    }
                }
                g.DrawLine(Pens.Black, area.Left, area.Bottom, area.Right, area.Bottom);
                g.DrawString("El jugador pide carta mientras tenga menos de...", fEje, Brushes.Gray,
                    new RectangleF(area.Left, area.Bottom + 22, area.Width, 16), centro);
            }
        }
    }
}
