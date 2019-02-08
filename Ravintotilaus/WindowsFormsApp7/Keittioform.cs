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
    public partial class Keittioform : Form
    {
        Form1 mainform;
        Menuform menuf;

        //Tämän formin refresh ajastin
        //Tämä formi yrittää joka viides sekunti lukea tilauslistaa 
        //ja päivittää datagridviewin esittämään tehtäviä tilauksia
        Timer timer;

        public Keittioform(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            //Tehdessäni menuformin annan sille yhteyden mainformiin ja sen metodeihin
            menuf = new Menuform(mainform);
            //LueTilaukset on metodi jolla saadaan näkyviin tekemättömät tilaukset
            DataGridViewPaivitys();
        }

        private void DataGridViewPaivitys()
        {
            //indexLista säilöö päivityksen ajaksi mitkä indexit käyttäjä oli valinnut
            //myöhemmin ohjelma valitsee samat indexit uudestaan
            List<int> indexLista = new List<int>();
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
            {
                indexLista.Add(dataGridView1.SelectedRows[i].Index);
            }

            //Tyhjennän datagridviewin
            dataGridView1.Rows.Clear();
            //Päivitän tilauslistan
            mainform.LueTilaukset();

            //Käyn jokaisen tilausrivin läpi
            for (int i = 0; i < mainform.tilausLista.Count; i++)
            {
                //Jos tilausrivin tilauksentila on "0", lisään sen tilaus olion tähän näkyviin
                if (mainform.tilausLista[i].tilauksentila == "0")
                {
                    dataGridView1.Rows.Add(mainform.tilausLista[i].tilausnro, mainform.tilausLista[i].poytanro, mainform.tilausLista[i].tilattutuote, mainform.tilausLista[i].tilattumaara, mainform.tilausLista[i].tiedostorivi);
                }
            }
            //Ensimmäinen riviä ei valita automaattisesti
            if (dataGridView1.RowCount > 0)
            {
                dataGridView1.Rows[0].Selected = false;
            }
            //Valitaan kaikki rivit, mitkä oli ennen päivitystä valittuina
            for (int i = 0; i < indexLista.Count; i++)
            {
                dataGridView1.Rows[indexLista[i]].Selected = true;
            }
        }

        //Kun tämä formi suljetaan mainformi tulee taas näkyviin
        //Tämän formin päivitys ajastin pysäytetään
        private void Keittioform_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            menuf.Close();
            mainform.Show();
        }

        //Tämä avaa ruokalistan muokkaamis formin
        private void muokkaaMenuaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Jos menuformi on suljettu se luodaan uudelleen
            if (menuf.IsDisposed)
            {
                //Tehdessäni menuformin annan sille yhteyden mainformiin ja sen metodeihin
                menuf = new Menuform(mainform);
            }
            menuf.Show();
        }

        //Painaessa kaikkien valittujen rivien tilauksentila muutetaan "1" eli tehdyksi
        private void button1_Click(object sender, EventArgs e)
        {
            TiedostoonKirjoitus();
        }

        //Muokataan valitut rivit valmitetuiksi tiedostosta
        //Jos joku muu käyttää tiedostoja samaan aikaan loopataan, kunnes kirjoitus tapahtu onnistuu
        private void TiedostoonKirjoitus()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    //Luen kaikki tiedoston rivit
                    //Jokainen rivi lisätään yksiuloitteiseen taulukkoon omana solunaan
                    string[] Tiedostorivit = File.ReadAllLines(mainform.tilausfile);

                    //Käyn kaikki valitut rivit läpi ja muokkaan niiden tilauksentilan "1":ksi eli valmistetuksi
                    for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                    {
                        string[] Valitturivi = Tiedostorivit[int.Parse(dataGridView1.SelectedRows[i].Cells[4].Value.ToString())].Split(';');
                        Valitturivi[5] = "1";
                        Tiedostorivit[int.Parse(dataGridView1.SelectedRows[i].Cells[4].Value.ToString())] = string.Join(";", Valitturivi);
                    }

                    //Kirjoitan nyt muokatun taulukon tekstitiedostoon
                    File.WriteAllLines(mainform.tilausfile, Tiedostorivit);

                    //Päivitetään näkymä datagridviewissä
                    DataGridViewPaivitys();
                }
                else
                {
                    MessageBox.Show("Valitse valmistetut rivit!", "Huomio");
                }
            }
            catch
            {
                //Jos kirjoitus tapahtuma ei onnistunut, loopataan uudestaan kirjoitustapahtuma
                TiedostoonKirjoitus();
            }
        }

        //Suljetaan kaikki keittiöä
        private void suljeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Kun tämä formi avataan ajastin käynnistetään, joka päivittää formissa näkyviä tietoja
        private void Keittioform_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = (5 * 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        //Ajastin päivittää datagridviewiin uudet tiedot 5 sekunnin välein
        private void timer_Tick(object sender, EventArgs e)
        {
            mainform.LueTilaukset();
            DataGridViewPaivitys();
        }
    }
}
