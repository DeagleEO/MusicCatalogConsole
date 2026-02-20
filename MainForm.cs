using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;  // ДОБАВЬТЕ ЭТУ СТРОКУ

namespace MusicCatalogConsole
{
    public partial class MainForm : Form
    {
        private DatabaseHelper db = null!;
        private DataGridView dataGrid = null!;
        private Button btnAdd = null!;
        private Button btnDelete = null!;

        public MainForm()
        {
            InitializeComponent();
            db = new DatabaseHelper();
            LoadReleases();
        }

        private void InitializeComponent()
        {
            this.Text = "Каталог музыкальной коллекции";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Создаем таблицу
            dataGrid = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(760, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Создаем кнопки
            btnAdd = new Button
            {
                Text = "Добавить релиз",
                Location = new Point(10, 420),
                Size = new Size(120, 30)
            };

            btnDelete = new Button
            {
                Text = "Удалить релиз",
                Location = new Point(140, 420),
                Size = new Size(120, 30)
            };

            // Добавляем обработчики событий
            btnAdd.Click += BtnAdd_Click!;
            btnDelete.Click += BtnDelete_Click!;

            // Добавляем элементы на форму
            this.Controls.AddRange(new Control[] { dataGrid, btnAdd, btnDelete });
        }

        private void LoadReleases()
        {
            var releases = db.GetAllReleases();
            dataGrid.DataSource = releases;

            // Скрываем служебные колонки
            if (dataGrid.Columns.Contains("Id"))
                dataGrid.Columns["Id"].Visible = false;
            if (dataGrid.Columns.Contains("ArtistId"))
                dataGrid.Columns["ArtistId"].Visible = false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // ИСПРАВЬТЕ ЭТОТ МЕТОД НА ТАКОЙ:
            using (var addForm = new AddReleaseForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Создаем исполнителя
                    var artist = new Models.Artist
                    {
                        Name = addForm.GetArtistName()
                    };
                    db.AddArtist(artist);

                    // Получаем ID добавленного исполнителя
                    var artists = db.GetAllArtists();
                    var lastArtist = artists.LastOrDefault();

                    if (lastArtist != null)
                    {
                        // Создаем релиз
                        var release = new Models.Release
                        {
                            ArtistId = lastArtist.Id,
                            Title = addForm.GetTitle(),
                            Year = addForm.GetYear(),
                            MediaType = addForm.GetMediaType()
                        };

                        db.AddRelease(release);
                        LoadReleases();
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGrid.CurrentRow != null &&
                dataGrid.CurrentRow.DataBoundItem is Models.Release release)
            {
                var result = MessageBox.Show(
                    $"Удалить релиз '{release.Title}'?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    db.DeleteRelease(release.Id);
                    LoadReleases();
                }
            }
        }
    }


    public class AddReleaseForm : Form
    {
        private DatabaseHelper db = new DatabaseHelper();
        private TextBox txtTitle = new TextBox();
        private NumericUpDown numYear = new NumericUpDown();
        private ComboBox cmbType = new ComboBox();
        private TextBox txtArtist = new TextBox();
        private TextBox txtDescription = new TextBox();

        public AddReleaseForm()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Добавить музыкальный релиз";
            this.Size = new Size(450, 320);
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
            new Label { Text = "Тип носителя:", Location = new Point(20, 150), Size = new Size(100, 20) },
            new Label { Text = "Описание:", Location = new Point(20, 190), Size = new Size(100, 20) }
        };

            // Поля ввода
            txtArtist.Location = new Point(130, 27);
            txtArtist.Size = new Size(250, 20);

            txtTitle.Location = new Point(130, 67);
            txtTitle.Size = new Size(250, 20);

            numYear.Location = new Point(130, 107);
            numYear.Size = new Size(100, 20);
            numYear.Minimum = 1900;
            numYear.Maximum = DateTime.Now.Year;
            numYear.Value = DateTime.Now.Year;

            // ОБНОВЛЕННЫЙ список типов носителей
            cmbType.Location = new Point(130, 147);
            cmbType.Size = new Size(150, 20);
            cmbType.Items.AddRange(new string[]
            {
            "CD",      // Компакт-диск
            "CDr",     // Записываемый компакт-диск
            "DVD",     // DVD (было DVD-Audio)
            "Винил",   // Виниловая пластинка
            "Кассета", // Аудиокассета
            "Цифровой" // Цифровой релиз
            });
            cmbType.SelectedIndex = 0;

            txtDescription.Location = new Point(130, 187);
            txtDescription.Size = new Size(250, 60);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;

            // Кнопки
            var btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(120, 260),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(220, 260),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            // Добавляем все на форму
            var controls = new Control[]
            {
            labels[0], labels[1], labels[2], labels[3], labels[4],
            txtArtist, txtTitle, numYear, cmbType, txtDescription,
            btnAdd, btnCancel
            };

            this.Controls.AddRange(controls);

            // Обработчик для кнопки Добавить
            btnAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtArtist.Text) ||
                    string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Заполните обязательные поля: Исполнитель и Название!",
                                  "Внимание",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                }
            };
        }

        public string GetArtistName() => txtArtist.Text;
        public string GetTitle() => txtTitle.Text;
        public int GetYear() => (int)numYear.Value;
        public string GetMediaType() => cmbType.SelectedItem?.ToString() ?? "CD";
        public string GetDescription() => txtDescription.Text;
    }
}