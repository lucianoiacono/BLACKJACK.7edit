using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLACKJACK._7edit
{
    public partial class Form1 : Form
    {
        int total = 0;
        Random r = new Random();
        int num1, num2, num3;
        public Form1()
        {
            InitializeComponent();
            cmbDificultad.SelectedIndex = 0;
            VerificarJuego();
            //while (total < 21)
            //{
            //    label5.Text = "en juego";
            //    cmbDificultad_SelectedIndexChanged(null, null);
            //}
            //do             {
            //    label5.Text = "en juego";
            //    cmbDificultad_SelectedIndexChanged(null, null);
            //} while (total < 21);
        }
        private void cmbDificultad_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbDificultad.SelectedItem.ToString())
            {
                case "facil":
                    num1 = r.Next(1, 7);
                    num2 = r.Next(1, 7);
                    num3 = r.Next(1, 7);
                    break;
                case "media":
                    num1 = r.Next(1, 7);
                    num2 = r.Next(1, 7);
                    num3 = r.Next(1, 12);
                    break;
                default:
                    num1 = r.Next(1, 12);
                    num2 = r.Next(1, 12);
                    num3 = r.Next(1, 12);
                    break;
            }
            label1.Text = num1.ToString();
            label2.Text = num2.ToString();
            label3.Text = num3.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            perdidas = 0;
            ganadas = 0;
            label5.Text = "😃🍀☘🎲¡MUCHA SUERTE!😃🍀🤞🏻☘🎲";
            label1.Text = num1.ToString();
            label2.Text = num2.ToString();
            label3.Text = num3.ToString();
            label4.Text = "TOTAL: 0";
            total = 0;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button1.Enabled = false;
            Random r = new Random();
            cmbDificultad_SelectedIndexChanged(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label5.Text = "";
            total += num1;
            label1.Text = num1.ToString();
            label2.Text = num2.ToString();
            label3.Text = num3.ToString();
            label4.Text = "TOTAL: " + total.ToString();
            cmbDificultad_SelectedIndexChanged(sender, e);
            VerificarJuego();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label5.Text = "";
            total += num2;
            label1.Text = num1.ToString();
            label2.Text = num2.ToString();
            label3.Text = num3.ToString();
            label4.Text = "TOTAL: " + total.ToString();
            cmbDificultad_SelectedIndexChanged(sender, e);
            VerificarJuego();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label5.Text = "";
            label1.Text = num1.ToString();
            label2.Text = num2.ToString();
            label3.Text = num3.ToString();
            total += num3;
            label4.Text = "TOTAL: " + total.ToString();
            cmbDificultad_SelectedIndexChanged(sender, e);
            VerificarJuego();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ganadas = 0;
            perdidas = 0;
            button5.Enabled = true;
            label5.Text = "😃🍀";
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
        }
        int ganadas = 0, perdidas = 0;
        private void button5_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            total = 0;
            
            while (total < 21)
            {
                total += r.Next(1,7); 
            }
            if (total == 21) ganadas++;
            else perdidas++;
            label5.Text = "SIMULACIÓN: " + ganadas + " GANADAS, " + perdidas + " PERDIDAS.";
            button5.Enabled = false;
        }

        private void VerificarJuego()
        {
            if (total == 21)
            {
                label5.Text = "🎉🎊😃FELICIDADES, GANASTE!😃🎊🎉";
                total = 0;
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
            else if (total > 21)
            {
                label5.Text = "😞💀PERDISTE, TE PASASTE DE 21!💀😞";
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                total = 0;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label4.Text = "TOTAL: 0";
            label5.Text = "VAMOS A JUGAR? QUE DIFICULTAD QUIERES?";
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }
    }
}
