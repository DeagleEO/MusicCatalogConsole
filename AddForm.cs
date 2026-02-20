using MusicCatalogConsole.Models;
using MusicCatalogConsole;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MusicCatalogConsole
{
    public class AddForm : Form
    {
        private DatabaseHelper db = new DatabaseHelper();
        private ComboBox cmbArtist;
        private TextBox txtTitle;
        private NumericUpDown numYear;
        private ComboBox cmbMediaType;

        public AddForm()
        {
            InitializeForm();
            LoadArtists();
            LoadMediaTypes();
        }

        private void InitializeForm()
        {
            this.Text = "Добавить релиз";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Метки
            var labels = new[]
            {
                new Label { Text = "Исполнитель:", Location = new Point(20, 30), Size = new Size(100, 20) },
                new Label { Text = "Название:", Location = new Point(20, 70), Size = new Size(100, 20) },
                new Label { Text = "Год:", Location = new Point(20, 110), Size = new Size(100, 20) },
                new Label { Text = "Тип носителя:", Location = new Point(20, 150), Size = new Size(100, 20) }
            };

            // Поля ввода
            cmbArtist = new ComboBox { Location = new Point(130, 27), Size = new Size(200, 20) };
            txtTitle = new TextBox { Location = new Point(130, 67), Size = new Size(200, 20) };
            numYear = new NumericUpDown
            {
                Location = new Point(130, 107),
                Size = new Size(100, 20),
                Minimum = 1900,
                Maximum = DateTime.Now.Year,
                Value = DateTime.Now.Year
            };
            cmbMediaType = new ComboBox { Location = new Point(130, 147), Size = new Size(200, 20) };

            // Кнопки
            var btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(100, 180),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(200, 180),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            // Добавляем все на форму
            var controls = new Control[]
            {
                labels[0], labels[1], labels[2], labels[3],
                cmbArtist, txtTitle, numYear, cmbMediaType,
                btnAdd, btnCancel
            };

            this.Controls.AddRange(controls);

            // Обработчик для кнопки Добавить
            btnAdd.Click += (s, e) =>
            {
                if (cmbArtist.SelectedItem == null || string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    this.DialogResult = DialogResult.None;
                }
                else
                {
                    AddRelease();
                }
            };
        }

        private void LoadArtists()
        {
            var artists = db.GetAllArtists();
            cmbArtist.DataSource = artists;
            cmbArtist.DisplayMember = "Name";
            cmbArtist.ValueMember = "Id";
        }

        private void LoadMediaTypes()
        {
            cmbMediaType.Items.AddRange(new string[] { "CD", "Винил", "Цифровой" });
            cmbMediaType.SelectedIndex = 0;
        }

        private void AddRelease()
        {
            var release = new Release
            {
                ArtistId = (int)cmbArtist.SelectedValue,
                Title = txtTitle.Text,
                Year = (int)numYear.Value,
                MediaType = cmbMediaType.SelectedItem?.ToString() ?? "CD"
            };

            db.AddRelease(release);
        }
    }
}