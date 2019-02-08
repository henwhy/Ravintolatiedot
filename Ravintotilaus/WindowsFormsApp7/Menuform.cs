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
    public partial class Menuform : Form
    {
        Form1 mainform;
        
        public Menuform(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            //Laitan datagridviewiin tiedot ruokalistasta
            PaivitaDataGridView();
        }

        //Päivittää datagridviewin näyttäämään kaikki ruokalistasta
        private void PaivitaDataGridView()
        {
            //Päivitän ruokalistan
            mainform.LueRuoat();
            //Datagridviewien tyhjennys, jotta ei tulisi useaan kertaan samoja rivejä
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            //Ruokalistan jokainen tuote hintoineen laitetaan datagridvieweihin
            for (int i = 0; i < mainform.ruokaLista.Count; i++)
            {
                //Jos valmiiksi tarjottavissa arvo on 0 se lisätään ruokien näkymään
                if (mainform.ruokaLista[i].valmiiksitarjottavissa == "0")
                {
                    dataGridView1.Rows.Add(mainform.ruokaLista[i].tuote, mainform.ruokaLista[i].hinta);
                }
                //muutoin se lisäätään juomanäkymään
                else
                {
                    dataGridView2.Rows.Add(mainform.ruokaLista[i].tuote, mainform.ruokaLista[i].hinta);
                }
            }
        }

        //Menun Päivitys näppäin
        //Kun tätä painetaan se päivittää ruokalista tekstitiedoston datagridviewin mukaisilla tiedoilla
        private void PaivitaMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Lopetetaan solujen tietojen muokkaus
            dataGridView1.EndEdit();
            dataGridView2.EndEdit();

            //Tarkistan, onko jokin solu jäänyt tyhjäksi
            //i-muuttuja on rivieille ja j-muuttuja on soluille
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                {
                    //Jos solu on tyhjänä, tietoja ei tallenneta
                    //Vaan palautetaan virhe viesti
                    if (dataGridView1.Rows[i].Cells[j].Value == null || dataGridView1.Rows[i].Cells[j].Value == DBNull.Value || String.IsNullOrWhiteSpace(dataGridView1.Rows[i].Cells[j].Value.ToString()))
                    {
                        MessageBox.Show("Et voi jättää tyhjiä kenttiä!");
                        return;
                    }
                }
            }
            //Tarkistan, onko jokin solu jäänyt tyhjäksi
            //i-muuttuja on rivieille ja j-muuttuja on soluille
            for (int i = 0; i < dataGridView2.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView2.Rows[i].Cells.Count; j++)
                {
                    //Jos solu on tyhjänä, tietoja ei tallenneta
                    //Vaan palautetaan virhe viesti
                    if (dataGridView2.Rows[i].Cells[j].Value == null || dataGridView2.Rows[i].Cells[j].Value == DBNull.Value || String.IsNullOrWhiteSpace(dataGridView2.Rows[i].Cells[j].Value.ToString()))
                    {
                        MessageBox.Show("Et voi jättää tyhjiä kenttiä!");
                        return;
                    }
                }
            }
            //Menun päivitys tiedostoon, mikäli tyhjiä soluja ei löytynyt
            PaivitaMenu();
            MessageBox.Show("Ruokalista päivitetty.", "Ilmoitus");
        }

        //Päivitetään Menutiedosto datagridviewien mukaisilla tiedoilla
        private void PaivitaMenu()
        {
            //Ohjelma yrittää kirjoittaa, jos epäonnistuu yritetään uudestaan
            try
            {
                using (StreamWriter file = new StreamWriter(mainform.ruokafile, false))
                {
                    //Kirjoitetaan loopissa jokaisen datagridview rivin tiedot
                    //datagridview1 rivit ovat ruokia joten annan niille "valmiusarvon" 0
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        string[] array = new string[3];
                        array[0] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        array[1] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                        array[2] = "0";
                        file.WriteLine(string.Join(";", array));
                    }
                    //Juomille valmiusarvo on 1 joten ne eivät mene keittiöön
                    for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                    {
                        string[] array = new string[3];
                        array[0] = dataGridView2.Rows[i].Cells[0].Value.ToString();
                        array[1] = dataGridView2.Rows[i].Cells[1].Value.ToString();
                        array[2] = "1";
                        file.WriteLine(string.Join(";", array));
                    }
                }
            }
            catch
            {
                PaivitaMenu();
            }
            
            //Seuraavat rivit koodia ovat ehkä turhia
            //Päivitetään ruokalista... 
            mainform.LueRuoat();
            //Päivitetään datagridviewin tiedot sellaiseksi, mitä ne tiedostossa on 
            PaivitaDataGridView();
        }

        //Estetään toisesta sarakkeesta muut kuin numero ja pilkku näppäimet
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 1) //Desired Column
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        //Vain yksi pilkku hyväksytään, muuten hyväksytään vain numeroita
        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            // hyväksy vain yksi pilkku
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void SuljeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        
    }
}
