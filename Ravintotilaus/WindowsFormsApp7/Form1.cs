using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using CsvHelper;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        //Tiedostojen sijantitiedot
        public string sijainti = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\RavintolaTiedot\";
        public string poytafile = "poydat.csv";
        public string ruokafile = "Ruokalista.csv";
        public string tilausfile = @"Tilaukset\Tilaukset" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
        public string viimeisintilausfile = "Viimeisintilausnumero.txt";
        public string kuitti = @"kuitit\kuitti";
        public string tunnusfile = @"Tunnukset.csv";
        
        //ValittuForm arvo muuttuu sen mukaan minkä Formin buttonia painaa
        string valittuForm;

        //uusiKayttaja tallentaa käyttäjän nimen, jonka salasana menee oikein, mutta salasanakenttä on silti tyhjä
        //Tämä mahdollistaa tämän käyttäjän salasanan vaihdon
        string uusiKayttaja;

        //uuSisalasana saa true arvon, jos käyttäjän salasana menee oikein, mutta salasanakenttä on silti tyhjä
        bool uusiSalasana;
        

        //Olioita sisältäviä listoja
        //Luokat ovat omissa tabeissaan
        public List<Ruoka> ruokaLista;
        public List<Poyta> poytaLista;
        public List<TilausRivi> tilausLista;
        public List<KirjautumisTiedot> tunnusLista;

        public Form1()
        {
            InitializeComponent();

            //Sovelluksen käynnistyksen yhteydessä luodaan kansio työpöydälle, mikäli sitä ei jo ole siellä
            if (!Directory.Exists(sijainti))
            {
                Directory.CreateDirectory(sijainti);
                MessageBox.Show("Uusi kansio luotu sijaintiin:\n\n" + sijainti + "\n\nSinne tallentuvat kaikki ohjelman käyttämät ja tekemät tiedostot.", "Ilmoitus");
            }
            
            //Lisään tiedostojen polun jokaiseen tiedostonimeen
            //Jotta tiedostoa kirjoittaessa ei tarvitse kuin kirjoittaa esim. viimeisintilausfile
            poytafile = sijainti + poytafile;
            ruokafile = sijainti + ruokafile;
            tilausfile = sijainti + tilausfile;
            viimeisintilausfile = sijainti + viimeisintilausfile;
            kuitti = sijainti + kuitti;
            tunnusfile = sijainti + tunnusfile;

            //Vaihdan textbox2 käyttämään salasana merkistöä, eli salasana ei näy käyttäjälle
            textBox2.UseSystemPasswordChar = true;
        }
        
        //Tämä vaihtaa valitunFormin Tarjoilijaksi ja Enabloi nappulat
        private void Button1_Click_1(object sender, EventArgs e)
        {
            EnableKirjautuminen();
            valittuForm = "Tarjoilija";
            label1.Text = "Olet kirjautumassa tarjoilijana";
            button1.BackColor = Color.LightGreen;
            button2.BackColor = default(Color);
            button3.BackColor = default(Color);
        }

        //Tämä vaihtaa valitunFormin Keittiotyolaiseksi ja Enabloi nappulat
        private void Button2_Click_1(object sender, EventArgs e)
        {
            EnableKirjautuminen();
            valittuForm = "Keittiotyolainen";
            label1.Text = "Olet kirjautumassa kokkina";
            button2.BackColor = Color.LightGreen;
            button1.BackColor = default(Color);
            button3.BackColor = default(Color);
        }

        //Tämä vaihtaa valitunFormin Esimieheksi ja Enabloi nappulat
        private void Button3_Click_1(object sender, EventArgs e)
        {
            EnableKirjautuminen();
            valittuForm = "Esimies";
            label1.Text = "Olet kirjautumassa esimiehenä";
            button3.BackColor = Color.LightGreen;
            button1.BackColor = default(Color);
            button2.BackColor = default(Color);
        }

        
        //Metodi jota kutsutaan kun jotain Forminvalintaa painetaan
        //Enabloi kirjautumiskentät
        public void EnableKirjautuminen()
        {
            label2.Enabled = true;
            textBox1.Enabled = true;
            label3.Enabled = true;
            textBox2.Enabled = true;
            button4.Enabled = true;
        }


        //LueTunnukset on julkinen metodi, jotta sitä voidaan kutsua muista formeista.
        //Tämä päivittää listan tunnusLista 
        public void LueTunnukset()
        {
            //Jos tunnus tiedostoa ei ole luodaan oletus käyttäjäksi admin, jonka salasana kenttä on tyhjä, admin on esimies asemassa oleva henkilö
            //Jos admin ensimmäisen kirjautumisensa aikana vaihtaa salasanansa uuteen.
            if (!File.Exists(tunnusfile))
            {
                using (StreamWriter file = new StreamWriter(tunnusfile))
                {
                    file.WriteLine("admin;046ed669a727ba9ee16d314248e654ac;Esimies");
                }
            }
            //Lukee CSV tiedoston ja Kutsuu jokaisella rivillä KirjautumisTiedot luokan staattista metodia joka luo
            //siitä aina uuden olion tunnuslistaan
            //Try catch on asetettu tähän, koska muuten jos jokin muu kirjoitus tai lukutapahtuma on kesken
            //Tämä ei pääse siihen käsiksi
            try
            {
                tunnusLista = File.ReadAllLines(tunnusfile)
                                               .Select(v => KirjautumisTiedot.FromCsv(v))
                                               .ToList();
            }
            catch { }
        }


        //LueRuoat on julkinen metodi, jotta sitä voidaan kutsua muista formeista.
        //Tämä päivittää listan ruokaLista 
        public void LueRuoat()
        {
            //Kansio ja tiedosto luodaan, jos niitä ei jo ole
            if (!File.Exists(ruokafile))
            {
                var myFile = File.Create(ruokafile);
                myFile.Close();
            }
            //Lukee CSV tiedoston ja Kutsuu jokaisella rivillä Ruoka luokan staattista metodia joka luo
            //siitä aina uuden olion ruokaListaan
            //Try catch on asetettu tähän, koska muuten jos jokin muu kirjoitus tai lukutapahtuma on kesken
            //Tämä ei pääse siihen käsiksi
            try
            {
                ruokaLista = File.ReadAllLines(ruokafile)
                                               .Select(v => Ruoka.FromCsv(v))
                                               .ToList();
            }
            catch { }
        }
        

        //LueTilaukset on julkinen metodi, jotta sitä voidaan kutsua muista formeista.
        //Tämä päivittää listan tilausLista 
        public void LueTilaukset()
        {
            //Kansio ja tiedosto luodaan, jos niitä ei jo ole
            if (!Directory.Exists(sijainti + "Tilaukset"))
            {
                Directory.CreateDirectory(sijainti + "Tilaukset");
            }
            if (!File.Exists(tilausfile))
            {
                var myFile = File.Create(tilausfile);
                myFile.Close();
            }
            

            //Lukee CSV tiedoston ja Kutsuu jokaisella rivillä Tilaus luokan staattista metodia joka luo
            //siitä aina uuden olion tilausListaan
            //Try catch on asetettu tähän, koska muuten jos jokin muu kirjoitus tai lukutapahtuma on kesken
            //Tämä ei pääse siihen käsiksi
            try
            {
                tilausLista = File.ReadAllLines(tilausfile)
                                               .Select(v => TilausRivi.FromCsv(v))
                                               .ToList();
            }
            catch  { }
        }


        //LueTilaukset on julkinen metodi, jotta sitä voidaan kutsua muista formeista.
        //Tämä päivittää listan tilausLista 
        public void LuePoydat()
        {
            //Jos pöytä tiedostoa ei jo ole, luodaan 1 pöytä vakio tiedoilla
            if (!File.Exists(poytafile))
            {
                using (StreamWriter file = new StreamWriter(poytafile))
                {
                    file.WriteLine("Tbl 1;0 chr;0");
                }
            }


            //Lukee CSV tiedoston ja Kutsuu jokaisella rivillä Poyta luokan staattista metodia joka luo
            //siitä aina uuden olion poytaListaan
            //Try catch on asetettu tähän, koska muuten jos jokin muu kirjoitus tai lukutapahtuma on kesken
            //Tämä ei pääse siihen käsiksi
            try
            {
                poytaLista = File.ReadAllLines(poytafile)
                                               .Select(v => Poyta.FromCsv(v))
                                               .ToList();
            }
            catch  { }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //Tarkistetaan onko käyttäjä syöttänyt käyttäjänimi kenttään käyttäjänimeä
            if (textBox1.Text != "")
            {
                //Päivittää tunnuslistan tiedostosta
                LueTunnukset();
                //Käyttäjänsyötteen tarkistaminen, löytyykö käyttäjän asettamat salasana ja käyttäjänimi järjestelmästä
                //ja onko käyttäjä hyväksytty valittuun näkymään
                TarkistaKayttajanSyote();
            }
            else
            {
                MessageBox.Show("Anna Käyttäjänimi ja salasana", "Huomio");
            }
        }
        
        //Metodi, jota kutsutaan kun tarkistetaan käyttäjän syöttämiä käyttäjätietoja
        private void TarkistaKayttajanSyote()
        {
            //Salasanat on tallennettu md5 hashina tiedostoon
            //Luodut salasanat sisältävät myös suolauksen molemmin puolin salasanaa
            string suola = "dsmDQIrjh89masio";
            MD5CryptoServiceProvider md5Kryptaaja = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(suola + textBox2.Text + suola);
            data = md5Kryptaaja.ComputeHash(data);
            string md5tiiviste = "";
            for (int j = 0; j < data.Length; j++)
            {
                md5tiiviste += data[j].ToString("x2").ToLower();
            }
            //md5tiiviste sisältää nyt suolattuna käyttäjän antaman syötteen md5 hash muodossa

            //Käyttäjän salasana on tarkoitus vaihtaa jos salasanana on tyhjä kenttä
            //Jos käyttäjällä on jo salasana uusiSalasana on epätosi
            if (uusiSalasana != true)
            {
                //Hyväksytty kirjautuminen on oletuksena false
                bool hyvaksyttykirjautuminen = false;

                //Käydään tunnuslista läpi täsmääkö käyttäjänimi ja salasana joidenkin tietojen kanssa ja onko käyttäjällä oikeuksia valittuunFormiin
                //Esimiehellä on oikeudet kaikkiin Formeihin
                for (int i = 0; i < tunnusLista.Count; i++)
                {
                    if (tunnusLista[i].kayttajatunnus.ToLower() == textBox1.Text.ToLower() && md5tiiviste == tunnusLista[i].salasana && (tunnusLista[i].rooli == valittuForm || tunnusLista[i].rooli == "Esimies"))
                    {
                        //Jos käyttäjän tiedot ovat oikein, mutta salasana kenttä on tyhjä hänelle annetaan mahdollisuus
                        //antaa uusisalasana
                        if (textBox2.Text == "")
                        {
                            VaihdaSalasanaa();
                            return;
                        }
                        //Käyttäjätiedot ovat oikeat
                        hyvaksyttykirjautuminen = true;
                    }
                }

                //Jos käyttäjätiedot eivät täsmänneet tunnusListan kanssa, estetään käyttäjän formeihin pääsy
                if (hyvaksyttykirjautuminen != true)
                {
                    MessageBox.Show("Käyttäjätunnus tai salasana väärin tai sinulla ei ole oikeuksia tähän näkymään!", "Huomio");
                    return;
                }

                //Jos valittu Formi oli tarjoilija avataan se
                if (valittuForm == "Tarjoilija")
                {
                    Tarjoilijaform tf = new Tarjoilijaform(this);
                    tf.Show();
                    Hide();
                }
                
                //Jos valittu Formi oli keittio avataan se
                else if (valittuForm == "Keittiotyolainen")
                {
                    Keittioform kf = new Keittioform(this);
                    kf.Show();
                    Hide();
                }
                
                //Jos valittu Formi oli esimies avataan se
                else
                {
                    Esimiesform ef = new Esimiesform(this);
                    ef.Show();
                    Hide();
                }
                //Laitetaan näkymä disabloidaan kirjautumiskentät ja nappi,
                //eli laitetaan näkymä samanlaiseksi kuin ohjelmän käynnistyessä
                PyyhiValinnat();
                
            }
            else
            {
                //Etsii tunnuslistasta käyttäjänimeen sopivan käyttäjänimen ja vaihtaa hänen salasanakseen tekstikentän
                //tiedon suolattuna ja md5 hashattuna
                for (int i = 0; i < tunnusLista.Count; i++)
                {
                    if (textBox1.Text == uusiKayttaja && tunnusLista[i].kayttajatunnus.ToLower() == textBox1.Text.ToLower())
                    {
                        data = System.Text.Encoding.ASCII.GetBytes(suola + textBox2.Text + suola);
                        data = md5Kryptaaja.ComputeHash(data);

                        string md5tiiviste1 = "";

                        for (int j = 0; j < data.Length; j++)
                        {
                            md5tiiviste1 += data[j].ToString("x2").ToLower();
                        }

                        tunnusLista[i].salasana = md5tiiviste1;
                        uusiSalasana = false;

                        //Päivitetään tunnustiedosto
                        KirjoitaTunnukset();
                        
                        MessageBox.Show("Salasana Vaihdettu", "Ilmoitus");

                        //Palauttaa ohjelman näkymän samanlaiseksi kuin se käynnistäessä on
                        PyyhiValinnat();
                    }
                }
            }
        }

        //Palauttaa Form1 näkymän samannäköiseksi kuin se käynnistäessä on
        private void PyyhiValinnat()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            valittuForm = "";
            label2.Enabled = false;
            textBox1.Enabled = false;
            label3.Enabled = false;
            textBox2.Enabled = false;
            button4.Enabled = false;
            button1.BackColor = default(Color);
            button2.BackColor = default(Color);
            button3.BackColor = default(Color);
            label1.Text = "Valitse käyttäjänäkymä";
        }

        //Metodi jota kutsutaan kun tunnusLista halutaan tallentaa tiedostoon
        //Käy jokaisen olion listassa läpi ja tulostaa olion tiedot käyttäen olion metodia
        public void KirjoitaTunnukset()
        {
            try
            {
                var csv = new StringBuilder();

                foreach (var tunnus in tunnusLista)
                {
                    csv.AppendLine(tunnus.ToString());
                }
                File.WriteAllText(tunnusfile, csv.ToString());
            }
            catch
            {
                KirjoitaTunnukset();
            }
        }

        //Metodi jota kutsutaan kun poytaLista halutaan tallentaa tiedostoon
        //Käy jokaisen olion listassa läpi ja tulostaa olion tiedot käyttäen olion metodia
        public void KirjoitaPoydat()
        {
            try
            {
                var csv = new StringBuilder();

                foreach (var poyta in poytaLista)
                {
                    csv.AppendLine(poyta.ToString());
                }
                File.WriteAllText(poytafile, csv.ToString());
            }
            catch
            {
                KirjoitaPoydat();
            }
        }
        
        //Metodi jota kutsutaan kun poytaLista halutaan tallentaa tiedostoon
        //Käy jokaisen olion listassa läpi ja tulostaa olion tiedot käyttäen olion metodia
        public void KirjoitaTilaukset()
        {
            try
            {
                var csv = new StringBuilder();

                foreach (var tilaus in tilausLista)
                {
                    csv.AppendLine(tilaus.ToString());
                }
                File.WriteAllText(tilausfile, csv.ToString());
            }
            catch
            {
                KirjoitaTilaukset();
            }
        }

        //Metodi, jota kutsutaan kun salasana on mennyt oikein, mutta tekstikenttä on tyhjä
        private void VaihdaSalasanaa()
        {
            label1.Text = "Anna uusi salasana";
            uusiKayttaja = textBox1.Text;
            uusiSalasana = true;
        }
        
    }
}
