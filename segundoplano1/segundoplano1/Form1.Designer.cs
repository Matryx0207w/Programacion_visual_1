namespace OrdenamientoMultihilo
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.NumericUpDown numElementos;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.ProgressBar progressBurbuja;
        private System.Windows.Forms.ProgressBar progressQuickSort;
        private System.Windows.Forms.Label lblBurbuja;
        private System.Windows.Forms.Label lblQuickSort;
        private System.Windows.Forms.Label lblTiempoBurbuja;
        private System.Windows.Forms.Label lblTiempoQuickSort;
        private System.ComponentModel.BackgroundWorker backgroundWorkerQuickSort;
        private System.Windows.Forms.Button btnDetener;
        private System.Windows.Forms.ProgressBar progressMergeSort;
        private System.Windows.Forms.Label lblMergeSort;
        private System.Windows.Forms.Label lblTiempoMergeSort;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMergeSort;
        private System.Windows.Forms.ProgressBar progressSelectionSort;
        private System.Windows.Forms.Label lblSelectionSort;
        private System.Windows.Forms.Label lblTiempoSelectionSort;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSelectionSort;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTiempos;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.numElementos = new System.Windows.Forms.NumericUpDown();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.progressBurbuja = new System.Windows.Forms.ProgressBar();
            this.progressQuickSort = new System.Windows.Forms.ProgressBar();
            this.lblBurbuja = new System.Windows.Forms.Label();
            this.lblQuickSort = new System.Windows.Forms.Label();
            this.lblTiempoBurbuja = new System.Windows.Forms.Label();
            this.lblTiempoQuickSort = new System.Windows.Forms.Label();
            this.backgroundWorkerQuickSort = new System.ComponentModel.BackgroundWorker();
            this.btnDetener = new System.Windows.Forms.Button();
            this.progressMergeSort = new System.Windows.Forms.ProgressBar();
            this.lblMergeSort = new System.Windows.Forms.Label();
            this.lblTiempoMergeSort = new System.Windows.Forms.Label();
            this.backgroundWorkerMergeSort = new System.ComponentModel.BackgroundWorker();
            this.progressSelectionSort = new System.Windows.Forms.ProgressBar();
            this.lblSelectionSort = new System.Windows.Forms.Label();
            this.lblTiempoSelectionSort = new System.Windows.Forms.Label();
            this.backgroundWorkerSelectionSort = new System.ComponentModel.BackgroundWorker();
            this.btnExportar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chartTiempos = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.numElementos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTiempos)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerar
            // 
            this.btnGenerar.Location = new System.Drawing.Point(180, 20);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(100, 30);
            this.btnGenerar.TabIndex = 0;
            this.btnGenerar.Text = "Generar Datos";
            this.btnGenerar.UseVisualStyleBackColor = true;
            // 
            // numElementos
            // 
            this.numElementos.Location = new System.Drawing.Point(20, 25);
            this.numElementos.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numElementos.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numElementos.Name = "numElementos";
            this.numElementos.Size = new System.Drawing.Size(150, 20);
            this.numElementos.TabIndex = 1;
            this.numElementos.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // btnIniciar
            // 
            this.btnIniciar.Location = new System.Drawing.Point(290, 20);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(120, 30);
            this.btnIniciar.TabIndex = 2;
            this.btnIniciar.Text = "Iniciar Ordenamiento";
            this.btnIniciar.UseVisualStyleBackColor = true;
            // 
            // progressBurbuja
            // 
            this.progressBurbuja.Location = new System.Drawing.Point(20, 100);
            this.progressBurbuja.Name = "progressBurbuja";
            this.progressBurbuja.Size = new System.Drawing.Size(300, 23);
            this.progressBurbuja.TabIndex = 3;
            // 
            // progressQuickSort
            // 
            this.progressQuickSort.Location = new System.Drawing.Point(20, 160);
            this.progressQuickSort.Name = "progressQuickSort";
            this.progressQuickSort.Size = new System.Drawing.Size(300, 23);
            this.progressQuickSort.TabIndex = 4;
            // 
            // lblBurbuja
            // 
            this.lblBurbuja.AutoSize = true;
            this.lblBurbuja.Location = new System.Drawing.Point(330, 105);
            this.lblBurbuja.Name = "lblBurbuja";
            this.lblBurbuja.Size = new System.Drawing.Size(59, 13);
            this.lblBurbuja.TabIndex = 5;
            this.lblBurbuja.Text = "Burbuja: 0%";
            // 
            // lblQuickSort
            // 
            this.lblQuickSort.AutoSize = true;
            this.lblQuickSort.Location = new System.Drawing.Point(330, 165);
            this.lblQuickSort.Name = "lblQuickSort";
            this.lblQuickSort.Size = new System.Drawing.Size(75, 13);
            this.lblQuickSort.TabIndex = 6;
            this.lblQuickSort.Text = "QuickSort: 0%";
            // 
            // lblTiempoBurbuja
            // 
            this.lblTiempoBurbuja.AutoSize = true;
            this.lblTiempoBurbuja.Location = new System.Drawing.Point(20, 130);
            this.lblTiempoBurbuja.Name = "lblTiempoBurbuja";
            this.lblTiempoBurbuja.Size = new System.Drawing.Size(98, 13);
            this.lblTiempoBurbuja.TabIndex = 7;
            this.lblTiempoBurbuja.Text = "Tiempo: Iniciando...";
            // 
            // lblTiempoQuickSort
            // 
            this.lblTiempoQuickSort.AutoSize = true;
            this.lblTiempoQuickSort.Location = new System.Drawing.Point(20, 190);
            this.lblTiempoQuickSort.Name = "lblTiempoQuickSort";
            this.lblTiempoQuickSort.Size = new System.Drawing.Size(98, 13);
            this.lblTiempoQuickSort.TabIndex = 8;
            this.lblTiempoQuickSort.Text = "Tiempo: Iniciando...";
            // 
            // backgroundWorkerQuickSort
            // 
            this.backgroundWorkerQuickSort.WorkerReportsProgress = true;
            this.backgroundWorkerQuickSort.WorkerSupportsCancellation = true;
            // 
            // btnDetener
            // 
            this.btnDetener.Enabled = false;
            this.btnDetener.Location = new System.Drawing.Point(420, 20);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(75, 30);
            this.btnDetener.TabIndex = 9;
            this.btnDetener.Text = "Detener";
            this.btnDetener.UseVisualStyleBackColor = true;
            // 
            // progressMergeSort
            // 
            this.progressMergeSort.Location = new System.Drawing.Point(20, 220);
            this.progressMergeSort.Name = "progressMergeSort";
            this.progressMergeSort.Size = new System.Drawing.Size(300, 23);
            this.progressMergeSort.TabIndex = 10;
            // 
            // lblMergeSort
            // 
            this.lblMergeSort.AutoSize = true;
            this.lblMergeSort.Location = new System.Drawing.Point(330, 225);
            this.lblMergeSort.Name = "lblMergeSort";
            this.lblMergeSort.Size = new System.Drawing.Size(73, 13);
            this.lblMergeSort.TabIndex = 11;
            this.lblMergeSort.Text = "MergeSort: 0%";
            // 
            // lblTiempoMergeSort
            // 
            this.lblTiempoMergeSort.AutoSize = true;
            this.lblTiempoMergeSort.Location = new System.Drawing.Point(20, 250);
            this.lblTiempoMergeSort.Name = "lblTiempoMergeSort";
            this.lblTiempoMergeSort.Size = new System.Drawing.Size(98, 13);
            this.lblTiempoMergeSort.TabIndex = 12;
            this.lblTiempoMergeSort.Text = "Tiempo: Iniciando...";
            // 
            // backgroundWorkerMergeSort
            // 
            this.backgroundWorkerMergeSort.WorkerReportsProgress = true;
            this.backgroundWorkerMergeSort.WorkerSupportsCancellation = true;
            // 
            // progressSelectionSort
            // 
            this.progressSelectionSort.Location = new System.Drawing.Point(20, 280);
            this.progressSelectionSort.Name = "progressSelectionSort";
            this.progressSelectionSort.Size = new System.Drawing.Size(300, 23);
            this.progressSelectionSort.TabIndex = 13;
            // 
            // lblSelectionSort
            // 
            this.lblSelectionSort.AutoSize = true;
            this.lblSelectionSort.Location = new System.Drawing.Point(330, 285);
            this.lblSelectionSort.Name = "lblSelectionSort";
            this.lblSelectionSort.Size = new System.Drawing.Size(91, 13);
            this.lblSelectionSort.TabIndex = 14;
            this.lblSelectionSort.Text = "SelectionSort: 0%";
            // 
            // lblTiempoSelectionSort
            // 
            this.lblTiempoSelectionSort.AutoSize = true;
            this.lblTiempoSelectionSort.Location = new System.Drawing.Point(20, 310);
            this.lblTiempoSelectionSort.Name = "lblTiempoSelectionSort";
            this.lblTiempoSelectionSort.Size = new System.Drawing.Size(98, 13);
            this.lblTiempoSelectionSort.TabIndex = 15;
            this.lblTiempoSelectionSort.Text = "Tiempo: Iniciando...";
            // 
            // backgroundWorkerSelectionSort
            // 
            this.backgroundWorkerSelectionSort.WorkerReportsProgress = true;
            this.backgroundWorkerSelectionSort.WorkerSupportsCancellation = true;
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(420, 350);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(200, 30);
            this.btnExportar.TabIndex = 17;
            this.btnExportar.Text = "Exportar Iteraciones a Word";
            this.btnExportar.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Número de elementos:";
            // 
            // chartTiempos
            // 
            chartArea1.Name = "ChartArea1";
            this.chartTiempos.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartTiempos.Legends.Add(legend1);
            this.chartTiempos.Location = new System.Drawing.Point(420, 100);
            this.chartTiempos.Name = "chartTiempos";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Tiempos";
            this.chartTiempos.Series.Add(series1);
            this.chartTiempos.Size = new System.Drawing.Size(400, 240);
            this.chartTiempos.TabIndex = 19;
            this.chartTiempos.Text = "chart1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 391);
            this.Controls.Add(this.chartTiempos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.lblTiempoSelectionSort);
            this.Controls.Add(this.lblSelectionSort);
            this.Controls.Add(this.progressSelectionSort);
            this.Controls.Add(this.lblTiempoMergeSort);
            this.Controls.Add(this.lblMergeSort);
            this.Controls.Add(this.progressMergeSort);
            this.Controls.Add(this.btnDetener);
            this.Controls.Add(this.lblTiempoQuickSort);
            this.Controls.Add(this.lblTiempoBurbuja);
            this.Controls.Add(this.lblQuickSort);
            this.Controls.Add(this.lblBurbuja);
            this.Controls.Add(this.progressQuickSort);
            this.Controls.Add(this.progressBurbuja);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.numElementos);
            this.Controls.Add(this.btnGenerar);
            this.MinimumSize = new System.Drawing.Size(850, 430);
            this.Name = "Form1";
            this.Text = "Ordenamiento Multihilo - 4 Algoritmos + Gráficos";
            ((System.ComponentModel.ISupportInitialize)(this.numElementos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTiempos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}