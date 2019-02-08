using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Tilauksetform : Form
    {
        Timer timer;
        Form1 mainform;
        public Tilauksetform(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            //Laitetaan päivitetyt tilausrivitiedot datagridviewiin
            PaivitaDataGridViewit();
        }

        private void PaivitaDataGridViewit()
        {
            //Päivitetään tilausrivilista
            mainform.LueTilaukset();
            //Tyhjentää datagridviewit
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            //Käy kaikki tilausrivit läpi
            for (int i = 0; i < mainform.tilausLista.Count; i++)
            {
                //Jos tilausrivin tila on asetettu "2" eli laskutettu
                //Laitetaan tilausrivin tiedot datagridview2:seen
                if (mainform.tilausLista[i].tilauksentila == "2")
                {
                    bool loytyykoSamaTilausNro = false;
                    for (int j = 0; j < dataGridView2.RowCount; j++)
                    {
                        //Jos sama tilausnumero löytyy jo datagridviewistä,
                        //uutta riviä ei tehdä vaan se lisätään siihen datagridviewin riviin,
                        //Mistä sama tilausnro löytyi
                        if (dataGridView2.Rows[j].Cells[0].Value.ToString() == mainform.tilausLista[i].tilausnro)
                        {
                            dataGridView2.Rows[j].Cells[2].Value = (double.Parse(dataGridView2.Rows[j].Cells[2].Value.ToString()) + double.Parse(mainform.tilausLista[i].tilausrivinhinta)).ToString();
                            loytyykoSamaTilausNro = true;
                        }
                    }
                    //Jos tilausnumeroa ei löytynyt jo datagridview:istä,
                    //lisätään uusi rivi sinne
                    if (loytyykoSamaTilausNro != true)
                    {
                        dataGridView2.Rows.Add(
                            mainform.tilausLista[i].tilausnro,
                            mainform.tilausLista[i].poytanro,
                            mainform.tilausLista[i].tilausrivinhinta);
                    }
                }
                //Jollei tilausrivin tila ole "2"
                //Se laitetaan datagridview1:seen
                else
                {
                    bool testi = false;
                    for (int j = 0; j < dataGridView1.RowCount; j++)
                    {
                        //Jos sama tilausnumero löytyy jo datagridviewistä,
                        //uutta riviä ei tehdä vaan se lisätään siihen datagridviewin riviin,
                        //Mistä sama tilausnro löytyi
                        if (dataGridView1.Rows[j].Cells[0].Value.ToString() == mainform.tilausLista[i].tilausnro)
                        {
                            dataGridView1.Rows[j].Cells[2].Value = (double.Parse(dataGridView1.Rows[j].Cells[2].Value.ToString()) + double.Parse(mainform.tilausLista[i].tilausrivinhinta)).ToString();
                            testi = true;
                        }
                    }
                    //Jos tilausnumeroa ei löytynyt jo datagridview:istä,
                    //lisätään uusi rivi sinne
                    if (testi != true)
                    {
                        dataGridView1.Rows.Add(
                            mainform.tilausLista[i].tilausnro,
                            mainform.tilausLista[i].poytanro,
                            mainform.tilausLista[i].tilausrivinhinta);
                    }
                }
            }
            //Laskee tilausrivien yhteissummat ja päivittää tiedot labeleihin
            PaivitaLabelit();
        }

        //Päivittää labelit näyttämään tilausrivien summia
        private void PaivitaLabelit()
        {
            //Alustetaan arvot aluksi 0:aan
            decimal laskutettava = 0;
            decimal Laskutettu = 0;

            //Lasketaan molempien datagridviewien kokonaissumma omiin muuttujiinsa
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                laskutettava = laskutettava + decimal.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
            }
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                Laskutettu = Laskutettu + decimal.Parse(dataGridView2.Rows[i].Cells[2].Value.ToString());
            }

            //Laskuttamattomille tarkoitetut labelit
            label9.Text = (laskutettava * 86 / 100).ToString() + " €";
            label10.Text = (laskutettava * 14 / 100).ToString() + " €";
            label11.Text = (laskutettava).ToString() + " €";

            //Laskutetuille tarkoitetut labelit
            label12.Text = (Laskutettu * 86 / 100).ToString() + " €";
            label13.Text = (Laskutettu * 14 / 100).ToString() + " €";
            label14.Text = (Laskutettu).ToString() + " €";
        }

        //Tietojen päivitys ajastin
        private void Tilauksetform_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = (5 * 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        //Joka viides sekunti yritetään päivittää datagridview näkymää
        private void timer_Tick(object sender, EventArgs e)
        {
            PaivitaDataGridViewit();
        }

        //Kun näkymä suljetaan ajastin pysäytetään
        private void Tilauksetform_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
        }
    }
}
