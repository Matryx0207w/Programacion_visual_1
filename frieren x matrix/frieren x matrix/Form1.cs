using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VisorImagenes
{
    public partial class Form1 : Form
    {
        private List<string> rutasImagenes = new List<string>();
        private int indiceActual = 0;
        private bool modoGrises = false;
        private PictureBoxSizeMode modoTamaño = PictureBoxSizeMode.Zoom;

        public Form1()
        {
            InitializeComponent();
            ConfigurarToolStripButtons();
        }

        private void ConfigurarToolStripButtons()
        {
            toolStripButtonNormal.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonNormal.Text = "Normal";

            toolStripButtonGrises.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonGrises.Text = "Grises";

            toolStripButtonNormalSize.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonNormalSize.Text = "Normal";

            toolStripButtonZoom.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonZoom.Text = "Zoom";

            toolStripButtonStretch.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonStretch.Text = "Stretch";

            toolStripButtonCenter.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonCenter.Text = "Center";

            toolStripButtonRotateLeft.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonRotateLeft.Text = "↶";

            toolStripButtonRotateRight.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonRotateRight.Text = "↷";

            toolStripButtonRotate360.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonRotate360.Text = "360°";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // --- Diagnóstico de rutas ---
            string startup = Application.StartupPath;
            string directorio = Path.Combine(startup, "E:\\Programacion visual 1\\frieren x matrix\\frieren x matrix\\img");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Diagnóstico de rutas ===");
            sb.AppendLine($"Application.StartupPath: {startup}");
            sb.AppendLine($"Buscando carpeta: {directorio}");
            sb.AppendLine("");

            if (Directory.Exists(directorio))
            {
                sb.AppendLine("La carpeta 'imgfrie' EXISTE. Archivos encontrados:");
                foreach (var f in Directory.GetFiles(directorio))
                    sb.AppendLine(" - " + Path.GetFileName(f));
            }
            else
            {
                sb.AppendLine("La carpeta 'imgfrie' NO existe en la ruta buscada.");
            }

            MessageBox.Show(sb.ToString(), "Diagnóstico - imgfrie");

            // --- Cargar imágenes si existe la carpeta ---
            if (!Directory.Exists(directorio))
            {
                MessageBox.Show(
                    $"El directorio {directorio} no existe. " +
                    "Crea la carpeta 'imgfrie' en el proyecto, agrega imágenes " +
                    "y marca 'Copy if newer' en Propiedades.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string[] archivos = Directory.GetFiles(directorio);

                foreach (string archivo in archivos)
                {
                    if (EsImagen(Path.GetExtension(archivo)))
                    {
                        rutasImagenes.Add(archivo);
                        comboBoxImagenes.Items.Add(Path.GetFileName(archivo));
                    }
                }

                if (rutasImagenes.Count > 0)
                {
                    comboBoxImagenes.SelectedIndex = 0;
                    CargarImagen(0);
                }
                else
                {
                    MessageBox.Show("No se encontraron imágenes en la carpeta 'imgfrie'.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el directorio: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool EsImagen(string extension)
        {
            string[] extensionesValidas = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };
            return Array.Exists(extensionesValidas,
                ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        private void CargarImagen(int indice)
        {
            if (indice < 0 || indice >= rutasImagenes.Count) return;

            indiceActual = indice;
            comboBoxImagenes.SelectedIndex = indice;

            try
            {
                if (pictureBox.Image != null)
                {
                    var oldImage = pictureBox.Image;
                    pictureBox.Image = null;
                    oldImage.Dispose();
                }

                using (FileStream fs = new FileStream(rutasImagenes[indice], FileMode.Open, FileAccess.Read))
                {
                    Image imagenOriginal = Image.FromStream(fs);

                    if (modoGrises)
                    {
                        using (Bitmap imagenGrises = ConvertirAGrises(imagenOriginal))
                        {
                            pictureBox.Image = (Image)imagenGrises.Clone();
                        }
                    }
                    else
                    {
                        pictureBox.Image = (Image)imagenOriginal.Clone();
                    }

                    imagenOriginal.Dispose();
                }

                pictureBox.SizeMode = modoTamaño;
                statusLabel.Text = rutasImagenes[indice];
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Bitmap ConvertirAGrises(Image imagen)
        {
            Bitmap bmp = new Bitmap(imagen.Width, imagen.Height);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(colorMatrix);
                        g.DrawImage(imagen, new Rectangle(0, 0, imagen.Width, imagen.Height),
                                    0, 0, imagen.Width, imagen.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
                return bmp;
            }
            catch
            {
                bmp.Dispose();
                throw;
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (rutasImagenes.Count == 0) return;
            int nuevoIndice = (indiceActual - 1 + rutasImagenes.Count) % rutasImagenes.Count;
            CargarImagen(nuevoIndice);
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (rutasImagenes.Count == 0) return;
            int nuevoIndice = (indiceActual + 1) % rutasImagenes.Count;
            CargarImagen(nuevoIndice);
        }

        private void btnPrimera_Click(object sender, EventArgs e)
        {
            if (rutasImagenes.Count == 0) return;
            CargarImagen(0);
        }

        private void btnUltima_Click(object sender, EventArgs e)
        {
            if (rutasImagenes.Count == 0) return;
            CargarImagen(rutasImagenes.Count - 1);
        }

        private void comboBoxImagenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxImagenes.SelectedIndex >= 0 &&
                comboBoxImagenes.SelectedIndex != indiceActual)
            {
                CargarImagen(comboBoxImagenes.SelectedIndex);
            }
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoGrises = false;
            normalToolStripMenuItem.Checked = true;
            escalaDeGrisesToolStripMenuItem.Checked = false;
            toolStripButtonNormal.Checked = true;
            toolStripButtonGrises.Checked = false;

            if (rutasImagenes.Count > 0) CargarImagen(indiceActual);
        }

        private void escalaDeGrisesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoGrises = true;
            normalToolStripMenuItem.Checked = false;
            escalaDeGrisesToolStripMenuItem.Checked = true;
            toolStripButtonNormal.Checked = false;
            toolStripButtonGrises.Checked = true;

            if (rutasImagenes.Count > 0) CargarImagen(indiceActual);
        }

        private void normalTamañoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoTamaño = PictureBoxSizeMode.Normal;
            pictureBox.SizeMode = modoTamaño;
            ActualizarSeleccionTamaño();
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoTamaño = PictureBoxSizeMode.Zoom;
            pictureBox.SizeMode = modoTamaño;
            ActualizarSeleccionTamaño();
        }

        private void stretchImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoTamaño = PictureBoxSizeMode.StretchImage;
            pictureBox.SizeMode = modoTamaño;
            ActualizarSeleccionTamaño();
        }

        private void centerImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoTamaño = PictureBoxSizeMode.CenterImage;
            pictureBox.SizeMode = modoTamaño;
            ActualizarSeleccionTamaño();
        }

        private void ActualizarSeleccionTamaño()
        {
            normalTamañoToolStripMenuItem.Checked = (modoTamaño == PictureBoxSizeMode.Normal);
            zoomToolStripMenuItem.Checked = (modoTamaño == PictureBoxSizeMode.Zoom);
            stretchImageToolStripMenuItem.Checked = (modoTamaño == PictureBoxSizeMode.StretchImage);
            centerImageToolStripMenuItem.Checked = (modoTamaño == PictureBoxSizeMode.CenterImage);

            toolStripButtonNormalSize.Checked = (modoTamaño == PictureBoxSizeMode.Normal);
            toolStripButtonZoom.Checked = (modoTamaño == PictureBoxSizeMode.Zoom);
            toolStripButtonStretch.Checked = (modoTamaño == PictureBoxSizeMode.StretchImage);
            toolStripButtonCenter.Checked = (modoTamaño == PictureBoxSizeMode.CenterImage);
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ImageFormat formato = ImageFormat.Jpeg;
                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();

                    switch (extension)
                    {
                        case ".png": formato = ImageFormat.Png; break;
                        case ".bmp": formato = ImageFormat.Bmp; break;
                        case ".gif": formato = ImageFormat.Gif; break;
                        case ".tiff": formato = ImageFormat.Tiff; break;
                        default: formato = ImageFormat.Jpeg; break;
                    }

                    pictureBox.Image.Save(saveFileDialog.FileName, formato);
                    MessageBox.Show("Imagen guardada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar la imagen: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void girar90IzquierdaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                using (Bitmap tempImage = new Bitmap(pictureBox.Image))
                {
                    tempImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Image)tempImage.Clone();
                }
            }
        }

        private void girar90DerechaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                using (Bitmap tempImage = new Bitmap(pictureBox.Image))
                {
                    tempImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Image)tempImage.Clone();
                }
            }
        }

        private void copiarAlPortapapelesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                Clipboard.SetImage(pictureBox.Image);
                MessageBox.Show("Imagen copiada al portapapeles.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButtonRotate360_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                using (Bitmap tempImage = new Bitmap(pictureBox.Image))
                {
                    tempImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Image)tempImage.Clone();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }
        }
    }
}
