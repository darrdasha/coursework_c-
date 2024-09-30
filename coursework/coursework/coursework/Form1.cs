using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using music;

namespace coursework
{
    public partial class Form1 : Form
    {
        private MusicBand musicBand;
        private DataTable dataTable;
        private DataTable copia;
        private string filePath;
        private bool count_click_delete = true;
        private bool count_click_update = true;
        private bool count_click_view = true;
        public Form1()
        {
            InitializeComponent();
            dataTable = new DataTable();
            copia = new DataTable();
            musicBand = new MusicBand();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            dataGridView1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                openFileDialog1.Title = "Оберіть файл";
                openFileDialog1.Filter = "Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";
                openFileDialog1.InitialDirectory = @"D:\";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                    musicBand.FromFile(filePath);
                    label1.Text += musicBand.name;
                    label2.Text+= musicBand.genre;
                    dataTable.Columns.Add("Учасник", typeof(string));
                    dataTable.Columns.Add("Стаж", typeof(int));
                    for (int i = 0; i < musicBand.members.Length; i++)
                    {
                        dataTable.Rows.Add();
                        dataTable.Rows[i][0] = musicBand.members[i].member;
                        dataTable.Rows[i][1] = musicBand.members[i].experience;
                    }
                    copia = dataTable.Copy();
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Refresh();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка зчитування файлу: " + ex.Message);
            }

        }
            
        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Оберіть файл";
            openFileDialog1.Filter = "Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";
            openFileDialog1.InitialDirectory = @"D:\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                musicBand.SaveToFile(filePath);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (count_click_delete)
            {
                textBox1.Visible = true;
                count_click_delete = false;
            }
            else if(textBox1.Text!="")
            {
                Musicant musicant = new Musicant() { surname = textBox1.Text.Split()[0], name = textBox1.Text.Split()[1] };
                musicBand.Remove(musicant);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][0].ToString() == musicant.ToString())
                    {
                        dataTable.Rows.RemoveAt(i);
                        break;
                    }
                }
                dataGridView1.DataSource = dataTable;
                dataGridView1.Refresh();
                textBox1.Visible = false;
                count_click_delete = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (count_click_update)
            {
                panel1.Visible = true;
                count_click_update = false;
            }
            else if (textBox2.Text!=""&&textBox3.Text!="")
            {
                Musicant musicant = new Musicant() { surname = textBox2.Text.Split()[0], name = textBox2.Text.Split()[1] };
                int experience = int.Parse(textBox3.Text);
                for (int i= 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][0].ToString() == musicant.ToString())
                    {
                        dataTable.Rows[i][1] = experience;
                        break;
                    }
                }
                for (int i=0; i<musicBand.members.Length;i++)
                {
                    if (musicBand.members[i].member.ToString()==musicant.ToString())
                    {
                        musicBand.members[i].experience=experience;
                    }
                }
                dataGridView1.DataSource = dataTable;
                dataGridView1.Refresh();
                panel1.Visible = false;
                count_click_update = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSessionFromDataTable();
            musicBand.SortByExperience();
            for (int i = 0; i < musicBand.members.Length; i++)
            {
                dataTable.Rows[i][0] = musicBand.members[i].member;
                dataTable.Rows[i][1] = musicBand.members[i].experience;
            }
            dataGridView1.DataSource = dataTable;
            dataGridView1.Refresh();
            copia = dataTable.Copy();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSessionFromDataTable();
            musicBand.SortByAlphabet();
            for (int i = 0; i < musicBand.members.Length; i++)
            {
                dataTable.Rows[i][0] = musicBand.members[i].member;
                dataTable.Rows[i][1] = musicBand.members[i].experience;
            }
            dataGridView1.DataSource = dataTable;
            dataGridView1.Refresh();
            copia = dataTable.Copy();
        }
        private void UpdateSessionFromDataTable()
        {
            if (!TableEqual(dataTable, copia))
            {
                if (dataTable.Rows.Count > copia.Rows.Count)
                {
                    for (int i = copia.Rows.Count; i < dataTable.Rows.Count; i++)
                    {
                        string line = dataTable.Rows[i][0].ToString();
                        Musicant musicant = new Musicant() { surname = line.Split()[0], name = line.Split()[1]};
                        int exexperience = int.Parse(dataTable.Rows[i][1].ToString());
                        musicBand += new Member<Musicant>(musicant, exexperience);
                    }
                }
            }
        }
        private bool TableEqual(DataTable table1, DataTable table2)
        {
            if (table1.Rows.Count != table2.Rows.Count || table1.Columns.Count != table2.Columns.Count)
            {
                return false;
            }

            for (int i = 0; i < table1.Rows.Count; i++)
            {
                for (int j = 0; j < table1.Columns.Count; j++)
                {
                    if (!table1.Rows[i][j].Equals(table2.Rows[i][j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (count_click_view)
            {
                panel2.Visible = true;
                count_click_view = false;
            }
            else if (checkBox1.Checked || checkBox2.Checked || checkBox3.Checked || checkBox4.Checked)
            {
                string message = "";
                musicBand.SortByExperience();
                if(checkBox1.Checked)
                {
                    message += "Найбільший стаж: "+musicBand.members[0]+"\n";
                }
                if(checkBox2.Checked)
                {
                    message += "Найменший стаж: " +musicBand.members[musicBand.members.Length-1] + "\n";
                }
                if(checkBox3.Checked)
                {
                    message+="Стаж більше "+textBox4.Text + " років:\n";
                    for(int i=0; i<musicBand.members.Length; i++)
                    {
                        if (musicBand.members[i].experience>int.Parse(textBox4.Text))
                        {
                            message+=musicBand.members[i].ToString()+"\n";
                        }
                    }
                }
                if (checkBox4.Checked)
                {
                    message += "Стаж менше " + textBox4.Text + " років:\n";
                    for (int i = 0; i < musicBand.members.Length; i++)
                    {
                        if (musicBand.members[i].experience < int.Parse(textBox4.Text))
                        {
                            message += musicBand.members[i].ToString() + "\n";
                        }
                    }
                }
                MessageBox.Show(message, "Виведення інформації");
                panel2.Visible = false;
                count_click_view = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Visible = true;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Visible = true;
        }
    }
}
