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
    public partial class Tunnusformi : Form
    {
        Form1 mainform;

        public Tunnusformi(Form1 mf)
        {
            InitializeComponent();
            mainform = mf;
            //Lisätään Rooli rivi datagridviewiin
            DataGridViewComboBoxColumn rooli = new DataGridViewComboBoxColumn();
            var list11 = new List<string>() { "Tarjoilija", "Keittiotyolainen", "Esimies"};
            rooli.DataSource = list11;
            rooli.HeaderText = "Rooli";
            rooli.DataPropertyName = "Rooli";
            dataGridView1.Columns.Add(rooli);

            dataGridView1.RowTemplate.Height = 22;
            
            //Päivitetään datagridviewissä näkyvät käyttäjätiedot
            PaivitaDataGridView();
        }
        
        //Kun uusi rivi lisätään, sille annetaan vakiona aina salasanaksi tyhjä salasana
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].IsNewRow == true)
            {
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = "046ed669a727ba9ee16d314248e654ac";
            }
        }


        private void PaivitaDataGridView()
        {
            //Luetaan tunnustiedosto
            mainform.LueTunnukset();
            dataGridView1.Rows.Clear();
            for (int i = 0; i < mainform.tunnusLista.Count; i++)
            {
                dataGridView1.Rows.Add(mainform.tunnusLista[i].kayttajatunnus, mainform.tunnusLista[i].salasana, mainform.tunnusLista[i].rooli);
            }
        }

        //Tietojen päivityspainike
        private void PäivitäTiedotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Lopettaa tietojen editoinnin
            dataGridView1.EndEdit();
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
            PaivitaTunnukset();
        }

        //Päivitä tunnukset metodi kirjoittaa datagridviewin tiedot takaisin tiedostoon
        private void PaivitaTunnukset()
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                //Jos rivejä on enemmän kuin tunnuslistan pituus luodaan uusia tunnuksia listaan
                if (i >= mainform.tunnusLista.Count)
                {
                    KirjautumisTiedot currenttunnus = new KirjautumisTiedot
                    {
                        kayttajatunnus = dataGridView1.Rows[i].Cells[0].Value.ToString(),
                        salasana = dataGridView1.Rows[i].Cells[1].Value.ToString(),
                        rooli = dataGridView1.Rows[i].Cells[2].Value.ToString()
                    };
                    mainform.tunnusLista.Add(currenttunnus);
                }
                //muutoin tunnuslistan tietoja muokataan
                else
                {
                    mainform.tunnusLista[i].kayttajatunnus = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    mainform.tunnusLista[i].rooli = dataGridView1.Rows[i].Cells[2].Value.ToString();
                }
            }
            //Lopuksia kirjoitetaan tunnuslistan tiedot tiedostoon
            mainform.KirjoitaTunnukset();

            //Seuraavat rivit koodia ovat ehkä turhia
            //Päivitetään ruokalista... 
            mainform.LueTunnukset();
            //Päivitetään datagridviewin tiedot sellaiseksi, mitä ne tiedostossa on 
            PaivitaDataGridView();
        }

    }
}
