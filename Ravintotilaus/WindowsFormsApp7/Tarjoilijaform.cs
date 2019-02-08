using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Tarjoilijaform : Form
    {
        Laskutusform lasf;
        Form1 mainform;

        //Ajastin jolla päivitetään formin tietoja
        Timer timer;
        
        //Tallettaa löydetyn nykyisen tilausnumeron arvon,
        //Jos samassa pöydässä on aukinainen tilaus
        string nykyinentilaus;

        //Viimeisin tilausnumero
        //Luetaan erillisestä tiedostosta
        string viimeisintilaus;

        public Tarjoilijaform(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            lasf = new Laskutusform(mainform);
            //Päivitetään combobox näyttämään pöytien tietoja
            ComboBoxPaivitys();
            //Päivitetään listview näyttämään ruokien tietoja
            ListViewPaivitys();
            //Valitsen ensimmäisen pöydän comboboxista
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        //Päivitetään viimeisintilaus arvo tiedostosta
        private void LueViimeisinTilaus()
        {
            //Jos tiedostoa ei jo ole, se luodaan ja annetaan tilausnumeron arvoksi 1
            if (!File.Exists(mainform.viimeisintilausfile))
            {
                using (StreamWriter file = new StreamWriter(mainform.viimeisintilausfile))
                {
                    file.WriteLine("0000000001");
                }
            }
            //Lukemis looppi jos jokin käyttää tiedostoa samaan aikaan
            try
            {
                using (StreamReader file = new StreamReader(mainform.viimeisintilausfile, Encoding.Default))
                {
                    viimeisintilaus = file.ReadLine();
                }
            }
            catch
            {
                LueViimeisinTilaus();
            }
        }

        //Lisätään comboboxiin pöydät ja niiden tiedot
        private void ComboBoxPaivitys()
        {
            if (!comboBox1.DroppedDown)
            {
                //Päivitetään pöytätiedot vastaamaan tekstitiedostoa
                mainform.LuePoydat();
                //tallennan valitun combobox rivin index muuttujaksi, jonka myöhemmin valitsen uudestaan
                int index = comboBox1.SelectedIndex;
                comboBox1.Items.Clear();
                for (int i = 0; i < mainform.poytaLista.Count; i++)
                {
                    //Jos pöydän varattu arvo on "0" eli vapaa kirjoitetaan -->
                    if (mainform.poytaLista[i].varattu == "0")
                    {
                        comboBox1.Items.Add(mainform.poytaLista[i].poytanro + "    |    " + mainform.poytaLista[i].tuolimaara + "    |    Vapaa");
                    }
                    //Muutoin kirjoitetaan että pöytä on varattu
                    else
                    {
                        comboBox1.Items.Add(mainform.poytaLista[i].poytanro + "    |    " + mainform.poytaLista[i].tuolimaara + "    |    Varattu");
                    }
                }
                comboBox1.SelectedIndex = index;
            }
        }

        //Päivitetään listview näyttämään ruokalistaa
        private void ListViewPaivitys()
        {
            //Päivitetään ruokatiedot vastaamaan tekstitiedostoa
            mainform.LueRuoat();
            //Laitoin indexien arvon -1, koska listview index ei voi ikinä olla sellainen. 
            //0 se voi olla, muttei koskaan -1
            int index1 = -1;
            int index2 = -1;

            //Jos käyttäjä on valinnut enemmän kuin 0 indexiä ruokalistasta index1 arvo muuttuu valitun indexin kaltaiseksi
            if (listView1.SelectedIndices.Count > 0)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    index1 = listView1.Items.IndexOf(listView1.SelectedItems[0]);
                }
            }

            //Jos käyttäjä on valinnut enemmän kuin 0 indexiä juomalistasta index1 arvo muuttuu valitun indexin kaltaiseksi
            else if (listView2.SelectedIndices.Count > 0)
            {
                if (listView2.SelectedItems.Count > 0)
                {
                    index2 = listView2.Items.IndexOf(listView2.SelectedItems[0]);
                }
            }

            listView1.Items.Clear();
            listView2.Items.Clear();

            //Lisätään listvieweihin kaikki ruoat ja juomat tietoineen
            for (int i = 0; i < mainform.ruokaLista.Count; i++)
            {
                if (mainform.ruokaLista[i].valmiiksitarjottavissa == "0")
                {
                    ListViewItem lvi = new ListViewItem
                    {
                        Text = (mainform.ruokaLista[i].tuote)
                    };
                    lvi.SubItems.Add(mainform.ruokaLista[i].hinta + " €");
                    lvi.Tag = mainform.ruokaLista[i].valmiiksitarjottavissa;
                    listView1.Items.Add(lvi);
                }
                else
                {
                    ListViewItem lvi = new ListViewItem
                    {
                        Text = (mainform.ruokaLista[i].tuote)
                    };
                    lvi.SubItems.Add(mainform.ruokaLista[i].hinta + " €");
                    lvi.Tag = mainform.ruokaLista[i].valmiiksitarjottavissa;
                    listView2.Items.Add(lvi);
                }
            }
            //Jos index1 arvo on muuttunut valitaan listviewistä se tuote, mikä oli aiemmin valittu
            if (index1 != -1)
            {
                listView1.Items[index1].Selected = true;
            }
            //Jos index2 arvo on muuttunut valitaan listviewistä se tuote, mikä oli aiemmin valittu
            else if (index2 != -1)
            {
                listView2.Items[index2].Selected = true;
            }
        }
        
        //Tätä painiketta painamalla siirretään annos datagridviewiin
        private void Button1_Click(object sender, EventArgs e)
        {
            //Olen ohjelmoinut niin, että vain toisessa listviewissä voi olla kullakin hetkellä
            //valittuja item. Lisätään listviewistä item vain jos listviewissä on valittu item
            if (listView1.SelectedItems.Count > 0)
            {
                SiirräAnnosDataGridViewiin(listView1);
            }
            if (listView2.SelectedItems.Count > 0)
            {
                SiirräAnnosDataGridViewiin(listView2);
            }

            //Poistaa annos valinnan estääkseen vahinko lisäämisen
            listView1.SelectedItems.Clear();
            listView2.SelectedItems.Clear();
        }
        
        //Metodi jolla tuote siirtyy listviewistä datagridviewiin
        private void SiirräAnnosDataGridViewiin(ListView valittulistview)
        {
            //Vain jos käyttäjä on valinnut jonkun tuotteen listviewistä
            if (valittulistview.SelectedItems.Count > 0)
            {
                //Käydään kaikki datagridviewin rivit läpi, onko sinne lisätty jo sama tuote
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    //Tarkistaa onko datagridviewin rivin ensimmäinen solu tyhjä
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                    {
                        //Jos saman niminen tuote löytyy datagridviewistä sen rivin
                        //mistä tuote löytyy määrää ja hintaa kasvatetaan
                        if (valittulistview.SelectedItems[0].Text == dataGridView1.Rows[i].Cells[0].Value.ToString())
                        {
                            dataGridView1.Rows[i].Cells[2].Value = int.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()) + 1;
                            dataGridView1.Rows[i].Cells[1].Value = decimal.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) + decimal.Parse(valittulistview.SelectedItems[0].SubItems[1].Text.Remove(valittulistview.SelectedItems[0].SubItems[1].Text.Length - 2));
                            //Labelien summan päivitys
                            PaivitaSumma();
                            //Poistutaan metodista, jotta uutta riviä ei muodostuisi
                            return;
                        }
                    }
                }
                //Rivin lisäys, jos tuotetta ei oltu aiemmin lisätty tilattavaksi
                dataGridView1.Rows.Add(valittulistview.SelectedItems[0].Text, valittulistview.SelectedItems[0].SubItems[1].Text.Remove(valittulistview.SelectedItems[0].SubItems[1].Text.Length - 2), 1,valittulistview.SelectedItems[0].Tag);
                PaivitaSumma();
            }
        }
        
        //Päivittää hinta labelien tiedot kuvaamaan rivien tietoja
        private void PaivitaSumma()
        {
            decimal kokonaishinta = 0;
            //Laskee yhteen datagridviewin jokaisen rivin hinta arvon
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                kokonaishinta = kokonaishinta + decimal.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
            }
            //Päivitetään labelit
            //Ravintoloiden alvin mukaisesti 14%
            label6.Text = kokonaishinta.ToString() + " €";
            label5.Text = (kokonaishinta * 14 / 100).ToString() + " €";
            label4.Text = (kokonaishinta - (kokonaishinta * 14 / 100)).ToString() + " €";
        }

        //Kun annoksen poisto nappia painaa, rivi poistuu jos sen määrä on 1
        //muutoin määrä vähenee yhdellä
        private void Button2_Click(object sender, EventArgs e)
        {
            Annoksenpoisto();
            dataGridView1.ClearSelection();
        }
        
        //Annoksen poisto metodi vähentää tilattavien rivin määrää yhdellä,
        //jos määrä on enemmän kuin yksi. Jos määrä on 1, rivi poistetaan
        private void Annoksenpoisto()
        {
            //Tarkistus että datagridviewissä on rivejä
            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
            {
                //Kysytään käyttäjältä onko hän varma poistamisesta
                var confirmation = MessageBox.Show(
                    "Haluatko varmasti poistaa tämän annoksen tilauksesta?",
                    "Huomio", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                //Jos käyttäjä valitsee kyllä rivin tietoja muutetaan
                if (confirmation == DialogResult.Yes)
                {
                    //Rivi poistetaan, jos "määrä" arvo on "1"
                    if (dataGridView1.SelectedRows[0].Cells[2].Value.ToString() == "1")
                    {
                            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    }
                    //Muutoin rivin määrä arvoa vähennetään ja hintaa vähennetään
                    else
                    {
                        //Yhden tuotteen hinta on Rivin hinta jaettuna rivin tuotteiden määrällä
                        decimal yhdentuotteenhinta = decimal.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()) / decimal.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString());
                        //Vähenentään hinnasta yhden tuotteen määrä
                        dataGridView1.SelectedRows[0].Cells[1].Value = (decimal.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()) - yhdentuotteenhinta).ToString();
                        //Vähennetään määrää yhdellä
                        dataGridView1.SelectedRows[0].Cells[2].Value = (int.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString()) - 1).ToString();
                    }
                    //Päivitetään labelit näyttämään päivitettyä datagridviewia
                    PaivitaSumma();
                }
            }
            //Jos datagridviewissä ei ole valittu mitään riviä,
            //tulee virheilmoitus
            else
            {
                MessageBox.Show("Et valinnut mitään poistettavaa annosta", "Virhe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Painike jolla tilaustapahtuma tehdään
        private void Button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                var confirmation = MessageBox.Show(
                        "Haluatko varmasti lähettää tilauksen keittiöön?",
                        "Huomio", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );

                if (confirmation == DialogResult.Yes)
                {
                    //Päivittää tilausrivilistan
                    mainform.LueTilaukset();

                    //Lukee viimeisimmän tilausnumeron
                    LueViimeisinTilaus();

                    //Tarkistetaan onko pöydässä aukinaisia tilauksia
                    OnkoAukinaisiaTilauksia();

                    //Tilauksen tulostus tilaus tiedostoon
                    TilausTulostus();

                    //Varataan valittu pöytä
                    VaraaPoyta();

                    //Tyhjentää datagridviewin ja paivittää summat täsmäämään
                    dataGridView1.Rows.Clear();
                    PaivitaSumma();
                }
            }
            else
            {
                MessageBox.Show("Lisää annoksia tilaukseen!", "Huomio");
            }
        }

        private void VaraaPoyta()
        {
            //Splittaan comboboxin tekstin kolmeen osaan
            //ensimmäisestä lohkosta saan halutun pöytänumeron
            string[] comboBoxArray = comboBox1.Text.Split('|');
            
            mainform.poytaLista[int.Parse(comboBoxArray[0].Substring(4).Trim()) - 1].varattu = "1";

            //Pöytientiedot päivitetään tiedostoon
            mainform.KirjoitaPoydat();
        }

        //Metodi, jolla tarkistetaan tilauslista läpi onko pöydällä aukinaisia tilauksia
        public void OnkoAukinaisiaTilauksia()
        {
            //Metodin alussa laitetaan nykyinen tilaus arvo "":ksi,
            //Jos nykyinentilaus saa jonkin muun arvon tulostetaan tiedosto sillä arvolla
            nykyinentilaus = "";

            //Tämä taulukko jakaa comboboxin tekstin, jotta saadaan helposti valittu Pöytänumero
            string[] comboboxarray = comboBox1.Text.Split('|');

            //Käydään koko tilauslista läpi
            //Jos pöydältä löytyy aukinaisia tilauksia
            //Laitetaan nykyinen tilaus arvoksi se tilausnumero, mikä sillä tilausrivillä oli
            for (int i = 0; i < mainform.tilausLista.Count; i++)
            {
                //trim poistaa välilyönnit pöytänumeron perästä
                if (comboboxarray[0].Trim() == mainform.tilausLista[i].poytanro && mainform.tilausLista[i].tilauksentila != "2")
                {
                    //Nyt nykyinen tilausnumeron arvon on muuttunut löydetyn aukinaisen tilauksen kaltaiseksi
                    nykyinentilaus = mainform.tilausLista[i].tilausnro;
                }
            }
        }


        //Uusien tilausrivien kirjoitus tekstitiedostoon
        private void TilausTulostus()
        {
            //Loop jos tilauksen tulostus epäonnistuu
            try
            { 
            //Jaan comboboxin tekstin taulukoksi käyttäen '|'-merkkiä
            string[] comboboxarray = comboBox1.Text.Split('|');

                using (StreamWriter file = new StreamWriter(mainform.tilausfile, true))
                {
                    //Jos avointa tilausta ei löytynyt pöydältä otetaan uusi tilausnumero
                    if (nykyinentilaus == "")
                    {

                        //Tulostetaan jokaisen datagridview-rivin tiedot tiedostoon
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            string[] array = new string[7];
                            array[0] = viimeisintilaus;
                            //Poistetaan pöytänumeron perästä vielä välilyönnit Trim:illä
                            array[1] = comboboxarray[0].Trim();
                            array[2] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                            array[3] = dataGridView1.Rows[i].Cells[2].Value.ToString();
                            array[4] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                            array[5] = dataGridView1.Rows[i].Cells[3].Value.ToString();
                            //Tämä tulostaa tiedostorivi arvon
                            array[6] = (mainform.tilausLista.Count + i).ToString();
                            //Tulostetaan taulukko tiedoston riviksi laittamalla jokaisen solun väliin ";"-merkki
                            file.WriteLine(string.Join(";", array));
                        }
                        //Päivitetään uusi viimeisintilausnumero tieto
                        int uusitilausnro = Int32.Parse(viimeisintilaus) + 1;
                        viimeisintilaus = uusitilausnro.ToString().PadLeft(10, '0');
                        //Päivitetään se myös tiedostoon
                        Kirjoitatilausnro();
                    }
                    //Jos pöydälle on löytynyt aukinainen tilaus
                    else
                    {
                        //Tulostetaan jokaisen datagridview-rivin tiedot tiedostoon
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            string[] array = new string[7];
                            //Pöydän aukinaista tilausnumeroa käytetään nyt tilausnumerona
                            array[0] = nykyinentilaus;
                            //Poistetaan pöytänumeron perästä vielä välilyönnit Trim:illä
                            array[1] = comboboxarray[0].Trim();
                            array[2] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                            array[3] = dataGridView1.Rows[i].Cells[2].Value.ToString();
                            array[4] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                            array[5] = dataGridView1.Rows[i].Cells[3].Value.ToString();
                            //Tämä tulostaa tiedostorivi arvon
                            array[6] = (mainform.tilausLista.Count + i).ToString();
                            //Tulostetaan taulukko tiedoston riviksi laittamalla jokaisen solun väliin ";"-merkki
                            file.WriteLine(string.Join(";", array));
                        }
                    }
                }
            }
            catch
            {
                TilausTulostus();
            }
        }
        
        //Kirjoitetaan päivitetty viimeisin tilausnumero tiedostoon
        private void Kirjoitatilausnro()
        {
            try
            {
                using (StreamWriter file = new StreamWriter(mainform.viimeisintilausfile))
                {
                    file.WriteLine(viimeisintilaus);
                }
            }
            catch
            {
                Kirjoitatilausnro();
            }
        }

        //Button4 tyhjentää datagridviewin ja päivittää labelien tiedot
        private void Button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                var confirmation = MessageBox.Show(
                        "Haluatko varmasti perua tilauksen?",
                        "Huomio", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );
                if (confirmation == DialogResult.Yes)
                {
                    dataGridView1.Rows.Clear();
                    PaivitaSumma();
                }
            }
        }
        
        //Kun tämä formi suljetaan näytetää pääformi
        //Tietojen päivitys ajastin pysäytetään
        //Suljetaan laskutusformi
        private void Tarjoilijaform_FormClosing(object sender, FormClosingEventArgs e)
        {
            lasf.Close();
            timer.Stop();
            mainform.Show();
        }

        //Laskutusnäkymän avaus painike
        private void LaskutusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Jos laskutus näkymä on suljettu, se avataan uudestaan
            if (lasf.IsDisposed)
            {
                lasf = new Laskutusform(mainform);
            }
            lasf.Show();
        }

        private void SuljeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        //Jos listview2 valitaan jokin rivi, pyyhitään toisesta listviewistä valinnat
        private void ListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.SelectedItems.Clear();
        }
        
        //Jos listview1 valitaan jokin rivi, pyyhitään toisesta listviewistä valinnat
        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.SelectedItems.Clear();
        }

        //Tietojen päivitys ajastin
        private void Tarjoilijaform_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = (5 * 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        //Pöytänäkymä ja ruokanäkymä päivittyy säännöllisesti 5 sekunnin välein
        private void timer_Tick(object sender, EventArgs e)
        {
            ComboBoxPaivitys();
            ListViewPaivitys();
        }
    }

}