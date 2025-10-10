using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace OrdenamientoMultihilo
{
    public partial class Form1 : Form
    {
        private List<int> listaOriginal;
        private List<int> listaBurbuja;
        private List<int> listaQuick;
        private Thread hiloBurbuja;
        private Stopwatch relojBurbuja = new Stopwatch();
        private Stopwatch relojQuick = new Stopwatch();
        private bool ordenamientoEnProgreso = false;

        public Form1()
        {
            InitializeComponent();
            ActualizarControles();
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            int cantidad = (int)numElementos.Value;
            Random rnd = new Random();
            listaOriginal = new List<int>();

            for (int i = 0; i < cantidad; i++)
                listaOriginal.Add(rnd.Next(0, cantidad * 10));

            MessageBox.Show($"Lista generada correctamente con {cantidad:N0} números.",
                          "Datos Generados",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (listaOriginal == null || listaOriginal.Count == 0)
            {
                MessageBox.Show("Primero genera los datos.",
                              "Advertencia",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            if (ordenamientoEnProgreso)
            {
                MessageBox.Show("Ya hay un ordenamiento en progreso.",
                              "Advertencia",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Reiniciar controles
            progressBurbuja.Value = 0;
            progressQuickSort.Value = 0;
            lblBurbuja.Text = "Burbuja: 0%";
            lblQuickSort.Text = "QuickSort: 0%";
            lblTiempoBurbuja.Text = "Tiempo: Iniciando...";
            lblTiempoQuickSort.Text = "Tiempo: Iniciando...";

            // Copiamos la lista para cada algoritmo
            listaBurbuja = new List<int>(listaOriginal);
            listaQuick = new List<int>(listaOriginal);

            ordenamientoEnProgreso = true;
            ActualizarControles();

            // Iniciar el hilo Burbuja
            hiloBurbuja = new Thread(new ThreadStart(OrdenarBurbuja));
            hiloBurbuja.IsBackground = true;
            hiloBurbuja.Start();

            // Iniciar el BackgroundWorker (QuickSort)
            backgroundWorkerQuickSort.RunWorkerAsync(listaQuick);
        }

        private void OrdenarBurbuja()
        {
            try
            {
                relojBurbuja.Restart();
                int n = listaBurbuja.Count;
                int totalIteraciones = n - 1;

                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = 0; j < n - i - 1; j++)
                    {
                        if (listaBurbuja[j] > listaBurbuja[j + 1])
                        {
                            int temp = listaBurbuja[j];
                            listaBurbuja[j] = listaBurbuja[j + 1];
                            listaBurbuja[j + 1] = temp;
                        }
                    }

                    // Reportar progreso (cada 100 iteraciones o en porcentajes específicos)
                    if (i % 100 == 0 || i == totalIteraciones - 1)
                    {
                        int progreso = (int)((i / (float)totalIteraciones) * 100);
                        ActualizarProgresoBurbuja(progreso);
                    }
                }

                relojBurbuja.Stop();
                ActualizarCompletadoBurbuja();
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Error en Burbuja: {ex.Message}",
                                  "Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }));
            }
        }

        private void ActualizarProgresoBurbuja(int progreso)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ActualizarProgresoBurbuja), progreso);
                return;
            }

            progressBurbuja.Value = Math.Min(progreso, 100);
            lblBurbuja.Text = $"Burbuja: {progreso}%";
            lblTiempoBurbuja.Text = $"Tiempo: {relojBurbuja.Elapsed:mm\\:ss\\.fff}";
        }

        private void ActualizarCompletadoBurbuja()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ActualizarCompletadoBurbuja));
                return;
            }

            progressBurbuja.Value = 100;
            lblBurbuja.Text = "Burbuja: Completado";
            lblTiempoBurbuja.Text = $"Tiempo: {relojBurbuja.ElapsedMilliseconds} ms";
            VerificarCompletadoTotal();
        }

        // Métodos auxiliares para QuickSort
        private void QuickSort(List<int> lista, int izquierda, int derecha, BackgroundWorker worker)
        {
            if (izquierda < derecha)
            {
                int pivot = Particionar(lista, izquierda, derecha);

                // Reportar progreso basado en el tamaño del segmento
                int progreso = (int)((derecha / (float)lista.Count) * 100);
                worker.ReportProgress(Math.Min(progreso, 100));

                QuickSort(lista, izquierda, pivot - 1, worker);
                QuickSort(lista, pivot + 1, derecha, worker);
            }
        }

        private int Particionar(List<int> lista, int izquierda, int derecha)
        {
            int pivote = lista[derecha];
            int i = izquierda - 1;

            for (int j = izquierda; j < derecha; j++)
            {
                if (lista[j] <= pivote)
                {
                    i++;
                    int temp = lista[i];
                    lista[i] = lista[j];
                    lista[j] = temp;
                }
            }

            int temp2 = lista[i + 1];
            lista[i + 1] = lista[derecha];
            lista[derecha] = temp2;
            return i + 1;
        }

        // Eventos del BackgroundWorker
        private void backgroundWorkerQuickSort_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                relojQuick.Restart();
                List<int> lista = (List<int>)e.Argument;
                QuickSort(lista, 0, lista.Count - 1, backgroundWorkerQuickSort);
                relojQuick.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en QuickSort: {ex.Message}",
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void backgroundWorkerQuickSort_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressQuickSort.Value = e.ProgressPercentage;
            lblQuickSort.Text = $"QuickSort: {e.ProgressPercentage}%";
            lblTiempoQuickSort.Text = $"Tiempo: {relojQuick.Elapsed:mm\\:ss\\.fff}";
        }

        private void backgroundWorkerQuickSort_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressQuickSort.Value = 100;
            lblQuickSort.Text = "QuickSort: Completado";
            lblTiempoQuickSort.Text = $"Tiempo: {relojQuick.ElapsedMilliseconds} ms";
            VerificarCompletadoTotal();
        }

        private void VerificarCompletadoTotal()
        {
            if (progressBurbuja.Value == 100 && progressQuickSort.Value == 100)
            {
                ordenamientoEnProgreso = false;
                ActualizarControles();

                MessageBox.Show($"Ordenamiento completado!\n\n" +
                              $"Burbuja: {relojBurbuja.ElapsedMilliseconds} ms\n" +
                              $"QuickSort: {relojQuick.ElapsedMilliseconds} ms",
                              "Completado",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }

        private void ActualizarControles()
        {
            btnIniciar.Enabled = !ordenamientoEnProgreso;
            btnGenerar.Enabled = !ordenamientoEnProgreso;
            numElementos.Enabled = !ordenamientoEnProgreso;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ordenamientoEnProgreso)
            {
                DialogResult result = MessageBox.Show("Hay un ordenamiento en progreso. ¿Estás seguro de que quieres salir?",
                                                    "Confirmar Salida",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                // Intentar detener los hilos de forma segura
                try
                {
                    if (hiloBurbuja != null && hiloBurbuja.IsAlive)
                        hiloBurbuja.Abort();

                    if (backgroundWorkerQuickSort.IsBusy)
                        backgroundWorkerQuickSort.CancelAsync();
                }
                catch
                {
                    // Ignorar excepciones al cerrar
                }
            }

            base.OnFormClosing(e);
        }
    }
}