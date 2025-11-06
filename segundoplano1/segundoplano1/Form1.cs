using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

// SI LOS GRÁFICOS NO FUNCIONAN, COMENTA ESTA LÍNEA:
using System.Windows.Forms.DataVisualization.Charting;

namespace OrdenamientoMultihilo
{
    public partial class Form1 : Form
    {
        private List<int> listaOriginal;
        private List<int> listaBurbuja;
        private List<int> listaQuick;
        private List<int> listaMerge;
        private List<int> listaSelection;

        private Thread hiloBurbuja;
        private Thread hiloSelection;

        private Stopwatch relojBurbuja = new Stopwatch();
        private Stopwatch relojQuick = new Stopwatch();
        private Stopwatch relojMerge = new Stopwatch();
        private Stopwatch relojSelection = new Stopwatch();

        private bool ordenamientoEnProgreso = false;
        private bool cancelarOperacion = false;

        private List<string> iteracionesBurbuja = new List<string>();
        private List<string> iteracionesQuick = new List<string>();
        private List<string> iteracionesMerge = new List<string>();
        private List<string> iteracionesSelection = new List<string>();

        public Form1()
        {
            InitializeComponent();

            // Configurar gráficos si están disponibles
            ConfigurarChart();

            // Conectar eventos manualmente
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

            this.backgroundWorkerQuickSort.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerQuickSort_DoWork);
            this.backgroundWorkerQuickSort.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerQuickSort_ProgressChanged);
            this.backgroundWorkerQuickSort.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerQuickSort_RunWorkerCompleted);

            this.backgroundWorkerMergeSort.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMergeSort_DoWork);
            this.backgroundWorkerMergeSort.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerMergeSort_ProgressChanged);
            this.backgroundWorkerMergeSort.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMergeSort_RunWorkerCompleted);

            ActualizarControles();
        }

        private void ConfigurarChart()
        {
            try
            {
                // Limpiar el chart
                chartTiempos.Series.Clear();
                chartTiempos.Titles.Clear();

                // Configurar área del chart
                chartTiempos.ChartAreas[0].AxisX.Title = "Algoritmos";
                chartTiempos.ChartAreas[0].AxisY.Title = "Tiempo (ms)";
                chartTiempos.ChartAreas[0].AxisY.Interval = 100;

                // Título del chart
                chartTiempos.Titles.Add("COMPARACIÓN DE TIEMPOS DE ORDENAMIENTO");
                chartTiempos.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);
            }
            catch (Exception)
            {
                // Si hay error, simplemente continuar sin gráficos
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                int cantidad = (int)numElementos.Value;

                if (cantidad > 5000)
                {
                    DialogResult result = MessageBox.Show(
                        $"¿Estás seguro de generar {cantidad:N0} elementos?\n" +
                        "Para cantidades grandes:\n" +
                        "- Bubble Sort puede tomar mucho tiempo\n" +
                        "- Selection Sort también será lento\n" +
                        "- QuickSort y MergeSort son más eficientes",
                        "Confirmar Generación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                Random rnd = new Random();
                listaOriginal = new List<int>();

                for (int i = 0; i < cantidad; i++)
                    listaOriginal.Add(rnd.Next(0, Math.Min(cantidad * 10, 1000000)));

                MessageBox.Show($"Lista generada correctamente con {cantidad:N0} números.",
                              "Datos Generados",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                if (listaOriginal == null || listaOriginal.Count == 0)
                {
                    MessageBox.Show("Primero genera los datos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (listaOriginal.Count > 5000)
                {
                    DialogResult result = MessageBox.Show(
                        $"¿Estás seguro de ordenar {listaOriginal.Count:N0} elementos?\n" +
                        "Tiempos estimados:\n" +
                        "- Bubble Sort: Muy lento (> 30 segundos)\n" +
                        "- Selection Sort: Lento\n" +
                        "- QuickSort/MergeSort: Rápidos\n\n" +
                        "¿Continuar?",
                        "Confirmar Ordenamiento",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                if (ordenamientoEnProgreso)
                {
                    MessageBox.Show("Ya hay un ordenamiento en progreso.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cancelarOperacion = false;
                iteracionesBurbuja.Clear();
                iteracionesQuick.Clear();
                iteracionesMerge.Clear();
                iteracionesSelection.Clear();

                ReiniciarControles();

                listaBurbuja = new List<int>(listaOriginal);
                listaQuick = new List<int>(listaOriginal);
                listaMerge = new List<int>(listaOriginal);
                listaSelection = new List<int>(listaOriginal);

                ordenamientoEnProgreso = true;
                ActualizarControles();

                IniciarBurbuja();
                IniciarQuickSort();
                IniciarMergeSort();
                IniciarSelectionSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar ordenamiento: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReiniciarControles()
        {
            progressBurbuja.Value = 0;
            progressQuickSort.Value = 0;
            progressMergeSort.Value = 0;
            progressSelectionSort.Value = 0;

            lblBurbuja.Text = "Burbuja: 0%";
            lblQuickSort.Text = "QuickSort: 0%";
            lblMergeSort.Text = "MergeSort: 0%";
            lblSelectionSort.Text = "SelectionSort: 0%";

            lblTiempoBurbuja.Text = "Tiempo: Iniciando...";
            lblTiempoQuickSort.Text = "Tiempo: Iniciando...";
            lblTiempoMergeSort.Text = "Tiempo: Iniciando...";
            lblTiempoSelectionSort.Text = "Tiempo: Iniciando...";

            // Limpiar gráfico
            try
            {
                chartTiempos.Series.Clear();
            }
            catch
            {
                // Ignorar si hay error con el chart
            }
        }

        private void IniciarBurbuja()
        {
            hiloBurbuja = new Thread(new ThreadStart(OrdenarBurbuja));
            hiloBurbuja.IsBackground = true;
            hiloBurbuja.Start();
        }

        private void IniciarQuickSort()
        {
            if (!backgroundWorkerQuickSort.IsBusy)
                backgroundWorkerQuickSort.RunWorkerAsync(listaQuick);
        }

        private void IniciarMergeSort()
        {
            if (!backgroundWorkerMergeSort.IsBusy)
                backgroundWorkerMergeSort.RunWorkerAsync(listaMerge);
        }

        private void IniciarSelectionSort()
        {
            hiloSelection = new Thread(new ThreadStart(OrdenarSelectionSort));
            hiloSelection.IsBackground = true;
            hiloSelection.Start();
        }

        private void OrdenarBurbuja()
        {
            try
            {
                relojBurbuja.Restart();
                int n = listaBurbuja.Count;

                iteracionesBurbuja.Add($"=== ORDENAMIENTO BURBUJA ===");
                iteracionesBurbuja.Add($"Inicio con {n} elementos");

                for (int i = 0; i < n - 1; i++)
                {
                    if (cancelarOperacion) return;

                    for (int j = 0; j < n - i - 1; j++)
                    {
                        if (listaBurbuja[j] > listaBurbuja[j + 1])
                        {
                            int temp = listaBurbuja[j];
                            listaBurbuja[j] = listaBurbuja[j + 1];
                            listaBurbuja[j + 1] = temp;
                        }
                    }

                    int progreso = (int)((i / (float)(n - 1)) * 100);
                    ActualizarProgresoBurbuja(progreso);
                }

                iteracionesBurbuja.Add($"Completado en {relojBurbuja.ElapsedMilliseconds} ms");
                relojBurbuja.Stop();
                ActualizarCompletadoBurbuja();
            }
            catch (ThreadAbortException)
            {
                iteracionesBurbuja.Add("Cancelado por el usuario");
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Error en Burbuja: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void OrdenarSelectionSort()
        {
            try
            {
                relojSelection.Restart();
                int n = listaSelection.Count;

                iteracionesSelection.Add($"=== ORDENAMIENTO SELECTION SORT ===");
                iteracionesSelection.Add($"Inicio con {n} elementos");

                for (int i = 0; i < n - 1; i++)
                {
                    if (cancelarOperacion) return;

                    int minIndex = i;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (listaSelection[j] < listaSelection[minIndex])
                            minIndex = j;
                    }

                    int temp = listaSelection[minIndex];
                    listaSelection[minIndex] = listaSelection[i];
                    listaSelection[i] = temp;

                    int progreso = (int)((i / (float)(n - 1)) * 100);
                    ActualizarProgresoSelection(progreso);
                }

                iteracionesSelection.Add($"Completado en {relojSelection.ElapsedMilliseconds} ms");
                relojSelection.Stop();
                ActualizarCompletadoSelection();
            }
            catch (ThreadAbortException)
            {
                iteracionesSelection.Add("Cancelado por el usuario");
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Error en SelectionSort: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ActualizarProgresoSelection(int progreso)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ActualizarProgresoSelection), progreso);
                return;
            }

            progressSelectionSort.Value = Math.Min(progreso, 100);
            lblSelectionSort.Text = $"SelectionSort: {progreso}%";
            lblTiempoSelectionSort.Text = $"Tiempo: {relojSelection.Elapsed:mm\\:ss\\.fff}";
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

        private void ActualizarCompletadoSelection()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ActualizarCompletadoSelection));
                return;
            }

            progressSelectionSort.Value = 100;
            lblSelectionSort.Text = "SelectionSort: Completado";
            lblTiempoSelectionSort.Text = $"Tiempo: {relojSelection.ElapsedMilliseconds} ms";
            VerificarCompletadoTotal();
        }

        private void QuickSort(List<int> lista, int izquierda, int derecha, BackgroundWorker worker, List<string> iteraciones)
        {
            if (izquierda < derecha && !cancelarOperacion)
            {
                int pivot = Particionar(lista, izquierda, derecha, iteraciones);
                int progreso = (int)((derecha / (float)lista.Count) * 100);
                worker.ReportProgress(Math.Min(progreso, 100));
                QuickSort(lista, izquierda, pivot - 1, worker, iteraciones);
                QuickSort(lista, pivot + 1, derecha, worker, iteraciones);
            }
        }

        private int Particionar(List<int> lista, int izquierda, int derecha, List<string> iteraciones)
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

        private void MergeSort(List<int> lista, int izquierda, int derecha, BackgroundWorker worker, List<string> iteraciones)
        {
            if (izquierda < derecha && !cancelarOperacion)
            {
                int medio = (izquierda + derecha) / 2;
                MergeSort(lista, izquierda, medio, worker, iteraciones);
                MergeSort(lista, medio + 1, derecha, worker, iteraciones);
                Merge(lista, izquierda, medio, derecha, iteraciones);
                int progreso = (int)((derecha / (float)lista.Count) * 100);
                worker.ReportProgress(Math.Min(progreso, 100));
            }
        }

        private void Merge(List<int> lista, int izquierda, int medio, int derecha, List<string> iteraciones)
        {
            int n1 = medio - izquierda + 1;
            int n2 = derecha - medio;

            List<int> izquierdaArray = new List<int>();
            List<int> derechaArray = new List<int>();

            for (int i = 0; i < n1; i++)
                izquierdaArray.Add(lista[izquierda + i]);
            for (int j = 0; j < n2; j++)
                derechaArray.Add(lista[medio + 1 + j]);

            int x = 0, y = 0;
            int k = izquierda;

            while (x < n1 && y < n2)
            {
                if (izquierdaArray[x] <= derechaArray[y])
                    lista[k++] = izquierdaArray[x++];
                else
                    lista[k++] = derechaArray[y++];
            }

            while (x < n1)
                lista[k++] = izquierdaArray[x++];

            while (y < n2)
                lista[k++] = derechaArray[y++];
        }

        private void backgroundWorkerQuickSort_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                relojQuick.Restart();
                List<int> lista = (List<int>)e.Argument;
                iteracionesQuick.Add($"=== ORDENAMIENTO QUICKSORT ===");
                iteracionesQuick.Add($"Inicio con {lista.Count} elementos");
                QuickSort(lista, 0, lista.Count - 1, backgroundWorkerQuickSort, iteracionesQuick);
                iteracionesQuick.Add($"Completado en {relojQuick.ElapsedMilliseconds} ms");
                relojQuick.Stop();
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Error en QuickSort: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
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

        private void backgroundWorkerMergeSort_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                relojMerge.Restart();
                List<int> lista = (List<int>)e.Argument;
                iteracionesMerge.Add($"=== ORDENAMIENTO MERGESORT ===");
                iteracionesMerge.Add($"Inicio con {lista.Count} elementos");
                MergeSort(lista, 0, lista.Count - 1, backgroundWorkerMergeSort, iteracionesMerge);
                iteracionesMerge.Add($"Completado en {relojMerge.ElapsedMilliseconds} ms");
                relojMerge.Stop();
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Error en MergeSort: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void backgroundWorkerMergeSort_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressMergeSort.Value = e.ProgressPercentage;
            lblMergeSort.Text = $"MergeSort: {e.ProgressPercentage}%";
            lblTiempoMergeSort.Text = $"Tiempo: {relojMerge.Elapsed:mm\\:ss\\.fff}";
        }

        private void backgroundWorkerMergeSort_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressMergeSort.Value = 100;
            lblMergeSort.Text = "MergeSort: Completado";
            lblTiempoMergeSort.Text = $"Tiempo: {relojMerge.ElapsedMilliseconds} ms";
            VerificarCompletadoTotal();
        }

        private void backgroundWorkerSelectionSort_DoWork(object sender, DoWorkEventArgs e) { }
        private void backgroundWorkerSelectionSort_ProgressChanged(object sender, ProgressChangedEventArgs e) { }
        private void backgroundWorkerSelectionSort_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        private void VerificarCompletadoTotal()
        {
            if (progressBurbuja.Value == 100 && progressQuickSort.Value == 100 &&
                progressMergeSort.Value == 100 && progressSelectionSort.Value == 100)
            {
                ordenamientoEnProgreso = false;
                ActualizarControles();

                // ACTUALIZAR GRÁFICOS
                ActualizarChart();

                MessageBox.Show($"¡Ordenamiento completado!\n\n" +
                              $"Burbuja: {relojBurbuja.ElapsedMilliseconds} ms\n" +
                              $"QuickSort: {relojQuick.ElapsedMilliseconds} ms\n" +
                              $"MergeSort: {relojMerge.ElapsedMilliseconds} ms\n" +
                              $"SelectionSort: {relojSelection.ElapsedMilliseconds} ms",
                              "Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ActualizarChart()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ActualizarChart));
                return;
            }

            try
            {
                chartTiempos.Series.Clear();

                // Crear nueva serie para el gráfico de barras
                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Tiempos",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,
                    IsValueShownAsLabel = true
                };

                // Agregar los tiempos de cada algoritmo
                series.Points.AddXY("Burbuja", relojBurbuja.ElapsedMilliseconds);
                series.Points.AddXY("QuickSort", relojQuick.ElapsedMilliseconds);
                series.Points.AddXY("MergeSort", relojMerge.ElapsedMilliseconds);
                series.Points.AddXY("Selection", relojSelection.ElapsedMilliseconds);

                chartTiempos.Series.Add(series);

                // Personalizar colores
                series.Points[0].Color = Color.Red;      // Burbuja
                series.Points[1].Color = Color.Green;    // QuickSort
                series.Points[2].Color = Color.Blue;     // MergeSort
                series.Points[3].Color = Color.Orange;   // SelectionSort

                // Ajustar el eje Y para que se vea mejor
                chartTiempos.ChartAreas[0].AxisY.Interval = Math.Max(100, relojBurbuja.ElapsedMilliseconds / 10);
            }
            catch (Exception ex)
            {
                // Si hay error con el chart, simplemente continuar
                Console.WriteLine($"Error al actualizar chart: {ex.Message}");
            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            try
            {
                cancelarOperacion = true;
                ordenamientoEnProgreso = false;

                // Detener hilos
                if (hiloBurbuja != null && hiloBurbuja.IsAlive)
                    hiloBurbuja.Abort();

                if (hiloSelection != null && hiloSelection.IsAlive)
                    hiloSelection.Abort();

                // Detener background workers
                if (backgroundWorkerQuickSort.IsBusy)
                    backgroundWorkerQuickSort.CancelAsync();

                if (backgroundWorkerMergeSort.IsBusy)
                    backgroundWorkerMergeSort.CancelAsync();

                // Detener cronómetros
                relojBurbuja.Stop();
                relojQuick.Stop();
                relojMerge.Stop();
                relojSelection.Stop();

                ActualizarControles();

                MessageBox.Show("Operaciones canceladas.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al detener: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                if ((iteracionesBurbuja.Count == 0 && iteracionesQuick.Count == 0 &&
                     iteracionesMerge.Count == 0 && iteracionesSelection.Count == 0) ||
                    !ordenamientoEnProgreso)
                {
                    MessageBox.Show("No hay datos de iteraciones para exportar.", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Archivos de texto (*.txt)|*.txt";
                saveDialog.Title = "Exportar Iteraciones";
                saveDialog.FileName = $"Iteraciones_Ordenamiento_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        writer.WriteLine("=== ITERACIONES DE ALGORITMOS DE ORDENAMIENTO ===");
                        writer.WriteLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine($"Elementos: {listaOriginal?.Count ?? 0}");
                        writer.WriteLine();

                        // Escribir iteraciones de cada algoritmo
                        if (iteracionesBurbuja.Count > 0)
                        {
                            writer.WriteLine("=== BUBBLE SORT ===");
                            foreach (string linea in iteracionesBurbuja)
                                writer.WriteLine(linea);
                            writer.WriteLine();
                        }

                        if (iteracionesQuick.Count > 0)
                        {
                            writer.WriteLine("=== QUICKSORT ===");
                            foreach (string linea in iteracionesQuick)
                                writer.WriteLine(linea);
                            writer.WriteLine();
                        }

                        if (iteracionesMerge.Count > 0)
                        {
                            writer.WriteLine("=== MERGESORT ===");
                            foreach (string linea in iteracionesMerge)
                                writer.WriteLine(linea);
                            writer.WriteLine();
                        }

                        if (iteracionesSelection.Count > 0)
                        {
                            writer.WriteLine("=== SELECTION SORT ===");
                            foreach (string linea in iteracionesSelection)
                                writer.WriteLine(linea);
                            writer.WriteLine();
                        }

                        // Resumen de tiempos
                        writer.WriteLine("=== RESUMEN DE TIEMPOS ===");
                        writer.WriteLine($"Bubble Sort: {relojBurbuja.ElapsedMilliseconds} ms");
                        writer.WriteLine($"QuickSort: {relojQuick.ElapsedMilliseconds} ms");
                        writer.WriteLine($"MergeSort: {relojMerge.ElapsedMilliseconds} ms");
                        writer.WriteLine($"Selection Sort: {relojSelection.ElapsedMilliseconds} ms");
                    }

                    MessageBox.Show($"Iteraciones exportadas correctamente a:\n{saveDialog.FileName}",
                                  "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarControles()
        {
            btnIniciar.Enabled = !ordenamientoEnProgreso;
            btnGenerar.Enabled = !ordenamientoEnProgreso;
            numElementos.Enabled = !ordenamientoEnProgreso;
            btnDetener.Enabled = ordenamientoEnProgreso;
            btnExportar.Enabled = !ordenamientoEnProgreso && (listaOriginal != null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Detener todos los procesos antes de cerrar
            cancelarOperacion = true;
            ordenamientoEnProgreso = false;

            if (hiloBurbuja != null && hiloBurbuja.IsAlive)
                hiloBurbuja.Abort();

            if (hiloSelection != null && hiloSelection.IsAlive)
                hiloSelection.Abort();

            if (backgroundWorkerQuickSort.IsBusy)
                backgroundWorkerQuickSort.CancelAsync();

            if (backgroundWorkerMergeSort.IsBusy)
                backgroundWorkerMergeSort.CancelAsync();
        }
    }
}