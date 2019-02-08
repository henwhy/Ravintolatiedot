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
    public partial class Esimiesform : Form
    {
        Form1 mainform;
        Tilauksetform tilf;
        Tunnusformi tunf;

        //Refresh/Formin päivitys ajastin
        Timer timer;

        //Lista tämän formin napeista, jotka esittävät ravintolan pöytätilannetta
        List<Button> Poytabuttonit = new List<Button>();
        
        //Koska tuolien määrän vaihtaminen pöydälle tapahtuu
        //Laittamalla ensin pöydän numeron ja sitten tuolien määrän
        //Olen asettanut tämän bool muuttujan erottamaan onko poytanumero annettu jo
        bool poytanroannettu;

        //Tämä numero muuttuja pitää sisällään valitun pöydän tuolimäärän vaihtamista varten
        int valittupoyta;

        public Esimiesform(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            tilf = new Tilauksetform(mainform);
            tunf = new Tunnusformi(mainform);
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox2.KeyPress += TextBox2_KeyPress;
            textBox3.KeyPress += TextBox3_KeyPress;
            //Päivitä pöydät lukee pöytälistan ja lisää sopivan verran painikkeita esittämään
            //Ravintolan pöytätilannetta
            PaivitaPoydat();
        }

        //Estän muiden merkkien kuin numeroiden painamisen seuraavissa tekstikentissä
        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            Painallus(sender, e);
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Painallus(sender, e);
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Painallus(sender, e);
        }

        //PAINALLUS METODI JOKA SALLII VAIN NUMEROT
        private void Painallus(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        //Päivitä pöydät lukee pöytälistan ja lisää sopivan verran painikkeita esittämään
        //Ravintolan pöytätilannetta
        public void PaivitaPoydat()
        {
            mainform.LuePoydat();
            //Lisää "pöytäbuttoneita" tarpeen vaatiessa ja tarpeen vaatiessa myös poistaa 
            PoydanLisays(mainform.poytaLista.Count);

            //Vaihtaa nappien tekstit kuvaamaan pöytien tietoja ja
            //Vaihtaa pöydän värin punaiseksi, jos pöydän varaus attribuutti on "0"
            for (int i = 0; i < mainform.poytaLista.Count; i++)
            {
                Poytabuttonit[i].Text = mainform.poytaLista[i].poytanro + "\n" + mainform.poytaLista[i].tuolimaara;
                if (mainform.poytaLista[i].varattu == "1")
                {
                    Poytabuttonit[i].BackColor = Color.Red;
                }
                else
                {
                    Poytabuttonit[i].BackColor = Color.LightGreen;
                }
            }
        }
        
        //Metodi joka lisää "Pöytänappeja"
        private void PoydanLisays(int poytaMaara)
        {
            //Näillä kordinaateilla määrätään, mihin ensimmäinen nappi sijoittuu
            int top = -30;
            int left = 5;

            //for-loop, joka lisää niin paljon nappeja poytabuttonit listaan,
            //kuinka paljon niitä puuttuu
            for (int i = 0; i < poytaMaara; i++)
            {
                //Jos muuttujan i / 5 jakojännös on nolla nappulat menevät seuraavalle riville
                if (i % 5 == 0)
                {
                    top += 57;
                    left = 20;
                }

                //Jos pöytä buttoneita ei ole niin paljon kuin on haluttu,
                //Ne lisätään
                if (i >= Poytabuttonit.Count)
                {
                    Button newButton = new Button
                    {
                        //newbuttonille annetaan uusi sijainti
                        Left = left,
                        Top = top
                    };
                    //Controls.add Laittaa nappulan näkymään tässä formissa
                    splitContainer1.Panel1.Controls.Add(newButton);
                    newButton.Width = 55;
                    newButton.Height = 55;
                    newButton.BackColor = Color.LightGreen;
                    newButton.Text = "Tbl " + (i + 1) + "\n0 chr";
                    //Lisään nyt luodut buttonit listaan,
                    //jotta niitä on mahdollista kutsua esim. for loopeissa
                    Poytabuttonit.Add(newButton);
                }

                //Lisätään pöytälistaan myös pöytätiedot, jos pöytiä on lisätty
                if (i >= mainform.poytaLista.Count)
                {
                    Poyta uusiPoyta = new Poyta
                    {
                        poytanro = "Tbl " + (i + 1),
                        tuolimaara = "0 chr",
                        varattu = "0"
                    };
                    mainform.poytaLista.Add(uusiPoyta);
                }
                //Joka napin välissä liikutaan vasemmalle
                left += 57;
            }
        }

        //Tämä painike lisää tai poistaa pöytäbuttoneita sen mukaan
        //kuinka paljon textBox1.Text arvo on
        private void ButtonLisays_Click_1(object sender, EventArgs e)
        {
            //Jos käyttäjä jättää tekstikentän tyhjäksi tai kirjoittaa luvuksi 0,
            //Annetaan virhe ilmoitus
            if (textBox1.Text == "" || int.Parse(textBox1.Text) <= 0)
            {
                MessageBox.Show("Virheellinen syöttö! Minimi pöytämäärä on 1.", "Huomio");
            }
            //Muutoin Lisätään tai poistetaan riippuen määrästä
            else
            {
                if (int.Parse(textBox1.Text) > Poytabuttonit.Count)
                {
                    PoydanLisays(int.Parse(textBox1.Text));
                }
                else
                {
                    PoydanPoisto();
                }
                //Tiedostoon tallennus lisäämisen tai poistamisen jälkeen
                mainform.KirjoitaPoydat();
                MessageBox.Show("Tiedot päivitetty.", "Ilmoitus");
                textBox1.Text = "";
            }
        }

        //Tätä metodia kutsutaan jos käyttäjä kirjoittaa pöytien arvoksi pienemmän kuin,
        //Mitä pöytämäärä on tällä hetkellä
        private void PoydanPoisto()
        {
            //Niin kauan kuin pöytäbuttoneita on enemmän
            //kuin annettu määrä. Buttoneita poistetaan
            for (int i = Poytabuttonit.Count; i > int.Parse(textBox1.Text); i--)
            {
                //Poistetaan myös pöytälistasta ja tämän formin näkymästä
                mainform.poytaLista.RemoveAt(i - 1);
                splitContainer1.Panel1.Controls.Remove(Poytabuttonit[i - 1]);
                Poytabuttonit.RemoveAt(i - 1);
            }
        }

        //Painike, jolla vaihdetaan tuolien määrää pöydässä
        private void Button2_Click_1(object sender, EventArgs e)
        {
            PaivitaPoydat();

            //Tämä arvo on aina aluksi false
            //kun pöytä numero on annettu se on true
            if (poytanroannettu)
            {
                //Maksimi tuolimääräksi olen laittanut 40kpl
                //Antaa virheilmoituksen, jos on jättänyt kentän tyhjäksi tai kirjoittanut 0:n
                if (textBox3.Text == "" || int.Parse(textBox3.Text) > 40 || int.Parse(textBox3.Text) <= 0)
                {
                    MessageBox.Show("Virheellinen syöttö! Tuolien määrä voi olla 1 - " + 40 + " väliltä.");
                }
                else
                {
                    //Vaihdan poytalistaan tuoli määrän
                    mainform.poytaLista[valittupoyta - 1].tuolimaara = textBox3.Text + " chr";
                    //Vaihdan poytanroannetun falseksi,
                    //jotta seuraavan kerran käyttäjä vaihtaa taas pöytänumeroa
                    poytanroannettu = false;
                    Poytabuttonit[valittupoyta - 1].Text = "Tbl " + valittupoyta + "\n" + int.Parse(textBox3.Text) + " chr";
                    //Päivittää tiedot tekstitiedostoon
                    mainform.KirjoitaPoydat();
                    MessageBox.Show("Tiedot päivitetty.", "Ilmoitus");
                    //Ohjetiedot muuttuu sopivaksi
                    label1.Text = "Kirjoita pöytänumero";
                }
            }
            else
            {
                //Taas tulee virhe ilmoitus, mikäli käyttäjä antaa virheellisiä arvoja
                if (textBox3.Text == "" || int.Parse(textBox3.Text) > Poytabuttonit.Count || int.Parse(textBox3.Text) <= 0)
                {
                    MessageBox.Show("Virheellinen syöttö! Antamasi arvo ylittää pöytien lukumäärän.\nAnna luku 1 - " + Poytabuttonit.Count + " väliltä.", "Huomio");
                }
                else
                {
                    valittupoyta = int.Parse(textBox3.Text);
                    poytanroannettu = true;
                    label1.Text = "Kirjoita tuolien lukumäärä";
                }
            }
            textBox3.Text = "";
        }

        //Tämä painike varaa valitun pöydän,
        //eli muuttaa sen punaiseksi näkymään
        private void Button1_Click_1(object sender, EventArgs e)
        {
            //Jos käyttäjä antaa tekstikenttään virheellisen arvon,
            //Tulee virheilmoitus
            if (textBox2.Text == "" || int.Parse(textBox2.Text) > Poytabuttonit.Count || int.Parse(textBox2.Text) <= 0)
            {
                MessageBox.Show("Virheellinen syöttö! Antamasi arvo ylittää pöytien lukumäärän.\nAnna luku 1 - " + Poytabuttonit.Count + " väliltä.", "Huomio");
            }
            else
            {
                //Valitun poydan varattu tieto muuttuu "1":ksi
                //ja tallennetaan paivitetty poytalista tiedostoon
                //Lopuksi päivitetään näkymä lukemalla tekstitiedosto uudestaan
                mainform.poytaLista[int.Parse(textBox2.Text) - 1].varattu = "1";
                mainform.KirjoitaPoydat();
                MessageBox.Show("Tiedot päivitetty.", "Ilmoitus");
                PaivitaPoydat();
                textBox2.Text = "";
            }
        }
        

        //Kun tämä formi suljetaan pääikkuna tulee taas näkyviin
        //suljetaan myös mahdollinen aukinainen tilaukset näkymä
        //Pysäytetään ajastin, joka päivittää tiedot tässä näkymässä
        private void Esimiesform_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            tilf.Close();
            mainform.Show();
        }

        //Sulkemisnäppäin toolstripissä
        private void SuljeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Siirtyminen tilausnäkymään
        private void TilauksiinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tilf.IsDisposed)
            {
                tilf = new Tilauksetform(mainform);
            }
            tilf.Show();
        }

        //Siirtyminen Käyttäjätietojen muokkausnäkymään
        private void LuoUusiaKäyttäjiäToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tunf.IsDisposed)
            {
                tunf = new Tunnusformi(mainform);
            }
            tunf.Show();
        }

        //Kun tämä form on avattu ajastin käynnistetään päivittämään tämän formin tietoja
        private void Esimiesform_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = (5 * 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        //Ajastin päivittää pöytätiedot
        private void timer_Tick(object sender, EventArgs e)
        {
            PaivitaPoydat();
        }
    }
}
