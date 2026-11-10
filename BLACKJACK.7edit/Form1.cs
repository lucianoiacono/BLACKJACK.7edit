using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLACKJACK._7edit
{
    public partial class Form1 : Form
    {
        // ---- juego ----
        private readonly Random rnd = new Random();
        private readonly Mazo mazo;
        private readonly Mano jugador = new Mano();
        private readonly Mano crupier = new Mano();
        private readonly Estadisticas stats = new Estadisticas();
        private bool enCurso;
        private List<ResultadoEstrategia> ultimaSimulacion;

        // ---- controles: pestaña "Jugar" ----
        private TabControl tabs;
        private TabPage tabJugar, tabEstadistica;
        private PanelCartas panelCrupier, panelJugador;
        private Label lblCrupier, lblJugador, lblMensaje, lblMazo;
        private Label lblPartidas, lblGanadas, lblPerdidas, lblEmpates, lblBlackjacks, lblPromedio;
        private Button btnPedir, btnPlantarse, btnNueva, btnReiniciar, btnCsvPartidas;

        // ---- controles: pestaña "Estadística" ----
        private NumericUpDown nudPartidas;
        private Button btnSimular, btnCsvSimulacion;
        private Label lblEstado, lblConclusion;
        private DataGridView tabla;
        private GraficoBarras grafico;

        private static readonly Color Fondo = Color.FromArgb(22, 95, 62);

        public Form1()
        {
            InitializeComponent();
            mazo = new Mazo(rnd);
            ConstruirInterfaz();
            ActualizarPantalla();
            lblMensaje.Text = "Apretá \"Nueva partida\" para repartir.";
        }

        // =====================================================================
        //  Armado de la interfaz
        // =====================================================================
        private void ConstruirInterfaz()
        {
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabJugar = new TabPage("Jugar");
            tabEstadistica = new TabPage("Estadística (simulación)");
            tabs.TabPages.Add(tabJugar);
            tabs.TabPages.Add(tabEstadistica);
            Controls.Add(tabs);

            ConstruirTabJugar();
            ConstruirTabEstadistica();
        }

        private Label CrearLabel(Control padre, string texto, int x, int y, int ancho, int alto, float tam, bool negrita, Color color)
        {
            Label l = new Label();
            l.Text = texto;
            l.Location = new Point(x, y);
            l.Size = new Size(ancho, alto);
            l.Font = new Font("Segoe UI", tam, negrita ? FontStyle.Bold : FontStyle.Regular);
            l.ForeColor = color;
            l.BackColor = Color.Transparent;
            padre.Controls.Add(l);
            return l;
        }

        private Button CrearBoton(Control padre, string texto, int x, int y, int ancho, int alto, float tam, EventHandler click)
        {
            Button b = new Button();
            b.Text = texto;
            b.Location = new Point(x, y);
            b.Size = new Size(ancho, alto);
            b.Font = new Font("Segoe UI", tam, FontStyle.Bold);
            b.Click += click;
            padre.Controls.Add(b);
            return b;
        }

        private void ConstruirTabJugar()
        {
            tabJugar.UseVisualStyleBackColor = false;
            tabJugar.BackColor = Fondo;

            lblCrupier = CrearLabel(tabJugar, "CRUPIER", 20, 12, 620, 28, 13F, true, Color.White);
            panelCrupier = new PanelCartas();
            panelCrupier.Location = new Point(20, 42);
            panelCrupier.Size = new Size(620, 125);
            panelCrupier.BackColor = Fondo;
            panelCrupier.Mano = crupier;
            tabJugar.Controls.Add(panelCrupier);

            lblJugador = CrearLabel(tabJugar, "JUGADOR", 20, 185, 620, 28, 13F, true, Color.White);
            panelJugador = new PanelCartas();
            panelJugador.Location = new Point(20, 215);
            panelJugador.Size = new Size(620, 125);
            panelJugador.BackColor = Fondo;
            panelJugador.Mano = jugador;
            tabJugar.Controls.Add(panelJugador);

            lblMensaje = CrearLabel(tabJugar, "", 20, 360, 620, 40, 16F, true, Color.Gold);
            lblMensaje.TextAlign = ContentAlignment.MiddleLeft;

            btnPedir = CrearBoton(tabJugar, "PEDIR CARTA", 20, 420, 190, 50, 12F, btnPedir_Click);
            btnPlantarse = CrearBoton(tabJugar, "PLANTARSE", 225, 420, 190, 50, 12F, btnPlantarse_Click);
            btnNueva = CrearBoton(tabJugar, "NUEVA PARTIDA", 430, 420, 190, 50, 12F, btnNueva_Click);

            lblMazo = CrearLabel(tabJugar, "", 20, 490, 400, 24, 9.5F, false, Color.White);
            CrearLabel(tabJugar, "Reglas: el As vale 11 o 1, las figuras 10. El crupier pide carta hasta llegar a 17. " +
                "Un blackjack natural (As + figura/10) gana, salvo que el crupier también lo tenga (empate).",
                20, 515, 620, 50, 9.5F, false, Color.White);

            // cuadro de estadísticas
            GroupBox grupo = new GroupBox();
            grupo.Text = "TUS ESTADÍSTICAS";
            grupo.ForeColor = Color.White;
            grupo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grupo.Location = new Point(670, 20);
            grupo.Size = new Size(280, 235);
            tabJugar.Controls.Add(grupo);

            Color blanco = Color.White;
            lblPartidas = CrearLabel(grupo, "", 15, 30, 250, 26, 11F, false, blanco);
            lblGanadas = CrearLabel(grupo, "", 15, 62, 250, 26, 11F, false, blanco);
            lblPerdidas = CrearLabel(grupo, "", 15, 94, 250, 26, 11F, false, blanco);
            lblEmpates = CrearLabel(grupo, "", 15, 126, 250, 26, 11F, false, blanco);
            lblBlackjacks = CrearLabel(grupo, "", 15, 158, 250, 26, 11F, false, blanco);
            lblPromedio = CrearLabel(grupo, "", 15, 190, 250, 26, 11F, false, blanco);

            btnReiniciar = CrearBoton(tabJugar, "Reiniciar estadísticas", 670, 275, 280, 40, 10F, btnReiniciar_Click);
            btnCsvPartidas = CrearBoton(tabJugar, "Exportar mis partidas a CSV", 670, 325, 280, 40, 10F, btnCsvPartidas_Click);
        }

        private void ConstruirTabEstadistica()
        {
            CrearLabel(tabEstadistica, "Partidas a simular por estrategia:", 15, 14, 230, 26, 10F, false, Color.Black);

            nudPartidas = new NumericUpDown();
            nudPartidas.Minimum = 1000;
            nudPartidas.Maximum = 500000;
            nudPartidas.Increment = 10000;
            nudPartidas.Value = 100000;
            nudPartidas.Location = new Point(245, 11);
            nudPartidas.Size = new Size(110, 27);
            tabEstadistica.Controls.Add(nudPartidas);

            btnSimular = CrearBoton(tabEstadistica, "Simular estrategias (12 a 20)", 370, 8, 250, 34, 10F, btnSimular_Click);
            btnCsvSimulacion = CrearBoton(tabEstadistica, "Exportar CSV", 630, 8, 130, 34, 10F, btnCsvSimulacion_Click);
            lblEstado = CrearLabel(tabEstadistica, "", 770, 14, 190, 24, 9F, false, Color.DimGray);

            tabla = new DataGridView();
            tabla.Location = new Point(15, 55);
            tabla.Size = new Size(940, 230);
            tabla.ReadOnly = true;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.RowHeadersVisible = false;
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.BackgroundColor = Color.White;
            string[] columnas = { "Plantarse en", "Ganadas %", "Empates %", "Perdidas %", "Se pasó %", "Margen ± (95%)", "Blackjacks" };
            foreach (string c in columnas) tabla.Columns.Add(c, c);
            foreach (DataGridViewColumn col in tabla.Columns)
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tabEstadistica.Controls.Add(tabla);

            grafico = new GraficoBarras();
            grafico.Location = new Point(15, 295);
            grafico.Size = new Size(690, 275);
            tabEstadistica.Controls.Add(grafico);

            lblConclusion = CrearLabel(tabEstadistica, "", 720, 295, 235, 275, 9.5F, false, Color.Black);
        }

        // =====================================================================
        //  Juego
        // =====================================================================
        private void btnNueva_Click(object sender, EventArgs e)
        {
            string aviso = "";
            if (mazo.Restantes < 15)
            {
                mazo.Reiniciar();
                aviso = " (se mezcló un mazo nuevo)";
            }

            jugador.Vaciar();
            crupier.Vaciar();
            jugador.Agregar(mazo.Robar());
            crupier.Agregar(mazo.Robar());
            jugador.Agregar(mazo.Robar());
            crupier.Agregar(mazo.Robar());
            enCurso = true;
            lblMensaje.Text = "¿Pedís carta o te plantás?" + aviso;

            // si alguno tiene blackjack natural, la partida termina enseguida
            if (jugador.EsBlackjack || crupier.EsBlackjack)
                Terminar();
            else
                ActualizarPantalla();
        }

        private void btnPedir_Click(object sender, EventArgs e)
        {
            if (!enCurso) return;
            jugador.Agregar(mazo.Robar());
            if (jugador.SePaso)
                Terminar();
            else if (jugador.Total == 21)
                Plantarse(); // con 21 no tiene sentido seguir
            else
                ActualizarPantalla();
        }

        private void btnPlantarse_Click(object sender, EventArgs e)
        {
            if (!enCurso) return;
            Plantarse();
        }

        private void Plantarse()
        {
            if (!jugador.SePaso)
                Simulador.JugarCrupier(mazo, crupier);
            Terminar();
        }

        private void Terminar()
        {
            enCurso = false;
            Resultado res = Simulador.Resolver(jugador, crupier);
            stats.Registrar(jugador, crupier, res);

            if (res == Resultado.Gana)
                lblMensaje.Text = jugador.EsBlackjack ? "¡BLACKJACK! Ganaste." : "¡Ganaste!";
            else if (res == Resultado.Pierde)
                lblMensaje.Text = jugador.SePaso ? "Perdiste: te pasaste de 21." :
                                  crupier.EsBlackjack ? "Perdiste: el crupier tiene blackjack." : "Perdiste.";
            else
                lblMensaje.Text = "Empate.";

            ActualizarPantalla();
        }

        private void ActualizarPantalla()
        {
            panelCrupier.OcultarSegunda = enCurso;
            panelCrupier.Invalidate();
            panelJugador.Invalidate();

            if (jugador.Cartas.Count == 0)
            {
                lblCrupier.Text = "CRUPIER";
                lblJugador.Text = "JUGADOR";
            }
            else
            {
                lblCrupier.Text = enCurso
                    ? "CRUPIER  (muestra " + crupier.Cartas[0].Valor + ")"
                    : "CRUPIER  (total " + crupier.Total + ")";
                lblJugador.Text = "JUGADOR  (total " + jugador.Total + ")";
            }

            btnPedir.Enabled = enCurso;
            btnPlantarse.Enabled = enCurso;
            btnNueva.Enabled = !enCurso;
            lblMazo.Text = "Cartas en el mazo: " + mazo.Restantes;

            lblPartidas.Text = "Partidas: " + stats.Partidas;
            lblGanadas.Text = "Ganadas: " + stats.Ganadas + "  (" + stats.PctGanadas.ToString("0.0") + "%)";
            lblPerdidas.Text = "Perdidas: " + stats.Perdidas;
            lblEmpates.Text = "Empates: " + stats.Empates;
            lblBlackjacks.Text = "Blackjacks: " + stats.Blackjacks;
            lblPromedio.Text = "Total promedio: " + stats.PromedioJugador.ToString("0.0");
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            if (stats.Partidas > 0 &&
                MessageBox.Show("¿Borrar las estadísticas de las partidas jugadas?", "Reiniciar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            stats.Reiniciar();
            ActualizarPantalla();
        }

        private void btnCsvPartidas_Click(object sender, EventArgs e)
        {
            if (stats.Partidas == 0)
            {
                MessageBox.Show("Todavía no jugaste ninguna partida.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV (*.csv)|*.csv";
                dlg.FileName = "partidas_blackjack.csv";
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try { stats.ExportarCsv(dlg.FileName); }
                catch (IOException ex) { MessageBox.Show("No se pudo guardar el archivo:\n" + ex.Message); }
            }
        }

        // =====================================================================
        //  Simulación
        // =====================================================================
        private async void btnSimular_Click(object sender, EventArgs e)
        {
            int n = (int)nudPartidas.Value;
            btnSimular.Enabled = false;
            btnCsvSimulacion.Enabled = false;
            lblEstado.Text = "Simulando...";
            Stopwatch reloj = Stopwatch.StartNew();

            // se hace en otro hilo para que la ventana no se congele
            List<ResultadoEstrategia> res = await Task.Run(() => Simulador.SimularRango(12, 20, n, new Random()));
            reloj.Stop();
            ultimaSimulacion = res;

            tabla.Rows.Clear();
            ResultadoEstrategia mejor = res[0];
            foreach (ResultadoEstrategia r in res)
            {
                tabla.Rows.Add(r.Umbral, r.PctGanadas.ToString("0.00"), r.PctEmpates.ToString("0.00"),
                    r.PctPerdidas.ToString("0.00"), r.PctPasadas.ToString("0.00"),
                    "± " + r.MargenError95.ToString("0.00"), r.Blackjacks);
                if (r.PctGanadas > mejor.PctGanadas) mejor = r;
            }

            grafico.Datos = res;
            grafico.Invalidate();

            // estrategias cuya diferencia con la mejor cae dentro del margen de error
            List<string> parecidas = new List<string>();
            foreach (ResultadoEstrategia r in res)
                if (r != mejor && mejor.PctGanadas - r.PctGanadas <= mejor.MargenError95 + r.MargenError95)
                    parecidas.Add(r.Umbral.ToString());

            string texto =
                "Mayor % de ganadas: plantarse con " + mejor.Umbral + " o más (" +
                mejor.PctGanadas.ToString("0.00") + "% ± " + mejor.MargenError95.ToString("0.00") + ").\n\n";
            if (parecidas.Count > 0)
                texto += "Pero la diferencia con plantarse en " + string.Join(", ", parecidas.ToArray()) +
                         " es menor que el margen de error: con estos datos no se pueden distinguir.\n\n";
            texto +=
                "Cada estrategia se simuló con " + n.ToString("N0") + " partidas (mazo nuevo en cada una). " +
                "Las barras negras del gráfico son el margen de error al 95% de confianza.\n\n" +
                "Ninguna llega al 50%: si el jugador se pasa, pierde sin que el crupier tenga que jugar.";
            lblConclusion.Text = texto;

            lblEstado.Text = "Listo (" + (reloj.ElapsedMilliseconds / 1000.0).ToString("0.0") + " s)";
            btnSimular.Enabled = true;
            btnCsvSimulacion.Enabled = true;
        }

        private void btnCsvSimulacion_Click(object sender, EventArgs e)
        {
            if (ultimaSimulacion == null)
            {
                MessageBox.Show("Primero corré la simulación.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV (*.csv)|*.csv";
                dlg.FileName = "simulacion_blackjack.csv";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Plantarse en;Partidas;Ganadas;Empates;Perdidas;Se paso;Blackjacks;% Ganadas;Margen error 95%");
                foreach (ResultadoEstrategia r in ultimaSimulacion)
                    sb.AppendLine(string.Join(";", new string[] {
                        r.Umbral.ToString(), r.Partidas.ToString(), r.Ganadas.ToString(), r.Empates.ToString(),
                        r.Perdidas.ToString(), r.Pasadas.ToString(), r.Blackjacks.ToString(),
                        r.PctGanadas.ToString("0.00"), r.MargenError95.ToString("0.00") }));
                try { File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true)); }
                catch (IOException ex) { MessageBox.Show("No se pudo guardar el archivo:\n" + ex.Message); }
            }
        }
    }
}
