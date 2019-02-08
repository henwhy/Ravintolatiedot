using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Laskutusform : Form
    {
        Form1 mainform;

        //tämä ajastin on päivittää tämän näkymän vastaamaan tiedostoista luettua tietoa
        Timer timer;

        public Laskutusform(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            //Laittaa pöytätiedot comboboxiin
            ComboBoxPaivitys();
            //Valitsee ensimmäisen pöydän comboboxista
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            //Lukee ensimmäisen pöydän tilaukset
            LuePoydanTilaukset();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }

        //Päivittää Comboboxin näyttään pöytien tietoja
        private void ComboBoxPaivitys()
        {
            //Jos combobox on "avattu", tietoja ei päivitetä
            if (!comboBox1.DroppedDown)
            {
                //Päivittää pöytälistan
                mainform.LuePoydat();
                //index tallentaa valitun indexin tiedon, jotta se voidaan clearaamisen jälkeen valita uudelleen
                int index = comboBox1.SelectedIndex;
                //Tyhjentää comboboxin, jotta ei tulisi saman pöydän tietoja useaan kertaan
                comboBox1.Items.Clear();

                //Luodaan niin monta combobox itemiä,
                //kuin on poytalistassa poytia ja niiden tietoja
                for (int i = 0; i < mainform.poytaLista.Count; i++)
                {
                    //Jos pöydän varaus attribuutin arvo on "0", se on varaamaton,
                    //muutoin se on varattu
                    if (mainform.poytaLista[i].varattu == "0")
                    {
                        comboBox1.Items.Add(mainform.poytaLista[i].poytanro + "    |    " + mainform.poytaLista[i].tuolimaara + "    |    Vapaa");
                    }
                    else
                    {
                        comboBox1.Items.Add(mainform.poytaLista[i].poytanro + "    |    " + mainform.poytaLista[i].tuolimaara + "    |    Varattu");
                    }
                }
                comboBox1.SelectedIndex = index;
            }
        }

        //Jos comboboxista vaihtaa "riviä" tai "pöytää"
        //Luetaan pöydän avoinna oleva tilaus
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LuePoydanTilaukset();
        }
        
        //Luetaan pöydän avoinna oleva tilaus
        private void LuePoydanTilaukset()
        {
            mainform.LueTilaukset();
            //Tyhjentää datagridviewin, ettei aiempien pöytien tiedot jää näkyviin
            dataGridView1.Rows.Clear();

            //Katsoo jokaisen tilausrivin läpi,
            //jos sieltä löytyy laskuttamattomia tilauksia tälle pöydälle
            for (int i = 0; i < mainform.tilausLista.Count; i++)
            {
                //Pilkon comboboxin teksti arvon kolmeen osaan
                //ensimmäisessä osassa on aina pöydännumero tieto
                //käytän tätä numero tietoa seuraavassa if-lauseessa
                string[] comboboxarray = comboBox1.Text.Split('|');

                //Jos tilauksen tila ei ole "2" eli laskutettu
                //Poytanumero on sama kuin valitun comboboxin poyta
                if (mainform.tilausLista[i].tilauksentila != "2" && comboboxarray[0].Trim() == mainform.tilausLista[i].poytanro)
                {
                    //Laitetaan tilausrivi datagridviewiin
                    dataGridView1.Rows.Add(
                        mainform.tilausLista[i].tilausnro,
                        mainform.tilausLista[i].poytanro,
                        mainform.tilausLista[i].tilattutuote,
                        mainform.tilausLista[i].tilattumaara,
                        mainform.tilausLista[i].tilausrivinhinta,
                        mainform.tilausLista[i].tiedostorivi);
                }
            }
        }

        //Painamalla painiketta kuitti tulostuu ja 
        //Muokataan tilausrivien tiedot laskutetuiksi ja
        //kirjoitetaan uudelleen tekstitiedostoon
        //Datagridview tyhjenee
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                TulostaKuitti();

                //Muokkaa jokaisen tilausrivin tiedot laskutetuiksi
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    mainform.tilausLista[int.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString())].tilauksentila = "2";
                }
                //Kirjoittaa tilausListan takaisin tiedostoon
                mainform.KirjoitaTilaukset();
                //tyhjentää datagridviewin, ovathan ne jo laskutettu
                PoytaVarauksenPoisto();
                dataGridView1.Rows.Clear();
                MessageBox.Show("Kuitti Tulostettu.", "Ilmoitus");
            }
            else
            {
                MessageBox.Show("Pöytä on jo laskutettu!", "Huomio");
            }
        }


        //Kun lasku maksetaan pöydästä pöytä varaus poistuu
        private void PoytaVarauksenPoisto()
        {
            string poytastr = dataGridView1.Rows[0].Cells[1].Value.ToString();
            mainform.poytaLista[int.Parse(poytastr.Substring(4)) - 1].varattu = "0";
            mainform.KirjoitaPoydat();
        }

        //Kuitin tulostus metodi
        private void TulostaKuitti()
        {
            //Tiedoston halutaan olevan aina uusi,
            //joten lisään tekstitiedoston polkuun pätkän tässä
            string viimeisinkuitti = mainform.kuitti + dataGridView1.Rows[0].Cells[0].Value.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

            //Kuittiin tuleva yhteensä arvo
            decimal yhteensa = 0;


            if (!Directory.Exists(mainform.sijainti + "Kuitit"))
            {
                Directory.CreateDirectory(mainform.sijainti + "Kuitit");
            }

            using (StreamWriter kuitti = new StreamWriter(viimeisinkuitti))
            {
                //Baarin tietoja
                kuitti.WriteLine("Takapaikka Bar");
                kuitti.WriteLine();
                kuitti.WriteLine("Kuusamo Tikula");
                kuitti.WriteLine("Tikulankuja 32");
                kuitti.WriteLine("93680 KUUSAMO");
                kuitti.WriteLine("puh. 0800 192 129");
                kuitti.WriteLine();
                //Päivämäärä
                kuitti.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH':'mm':'ss "));
                kuitti.WriteLine();

                //Jokainen tilausrivi tulee omaksi rivikseen kuitissa
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    yhteensa += decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                    kuitti.WriteLine(dataGridView1.Rows[i].Cells[2].Value.ToString() + " x " + dataGridView1.Rows[i].Cells[3].Value.ToString() + "   "+ dataGridView1.Rows[i].Cells[4].Value.ToString() + " EUR");
                }
                kuitti.WriteLine();
                //Ravintoloiden alv 14%
                kuitti.WriteLine("ALV %         14,00");
                kuitti.WriteLine();
                kuitti.WriteLine("Veroton       " + (yhteensa * 86 / 100) + " EUR");
                kuitti.WriteLine("Vero          " + (yhteensa * 14 / 100) + " EUR");
                kuitti.WriteLine("Verollinen    " + yhteensa + " EUR");
                kuitti.WriteLine();
                kuitti.WriteLine("Yhteensä   " + yhteensa + " EUR");
                kuitti.WriteLine();
                kuitti.WriteLine("Merci et a bientot!");
                kuitti.WriteLine("Y.tunnus 2113019-2");
            }
            //Avaa kuitin
            Process.Start(viimeisinkuitti);
        }

        //Tämän formin käynnistyttyä luodaan ajastin, jolla päivitetään näytön tietoja 
        private void Laskutusform_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = (5 * 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        //Joka viides sekunti pöytätiedot päivitetään comboboxiin
        private void timer_Tick(object sender, EventArgs e)
        {
            ComboBoxPaivitys();
        }

        //Kun laskutusnäkymä suljetaan ajastin pysähtyy ja päivityksiä ei enää tehdä
        private void Laskutusform_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
        }
    }
}
