using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GestorTareas
{
    public partial class Form1 : Form
    {
        public class Tarea
        {
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public DateTime Fecha { get; set; }
            public string Lugar { get; set; }
            public string Estado { get; set; }
        }

        private List<Tarea> listaTareas = new List<Tarea>();
        private List<Tarea> listaOriginal = new List<Tarea>();

        public Form1()
        {
            InitializeComponent();
            cmbEstado.SelectedIndex = 0;
        }

        private void ActualizarGrid()
        {
            dgvTareas.DataSource = null;
            dgvTareas.DataSource = listaTareas;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Por favor complete los campos obligatorios: Código, Nombre y Estado.");
                return;
            }

            Tarea nueva = new Tarea()
            {
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                Fecha = dtpFecha.Value,
                Lugar = txtLugar.Text,
                Estado = cmbEstado.SelectedItem.ToString()
            };

            listaTareas.Add(nueva);
            listaOriginal = new List<Tarea>(listaTareas);
            ActualizarGrid();
            LimpiarCampos();
            MessageBox.Show("Tarea agregada correctamente.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvTareas.SelectedRows.Count > 0)
            {
                int index = dgvTareas.SelectedRows[0].Index;
                listaTareas[index].Codigo = txtCodigo.Text;
                listaTareas[index].Nombre = txtNombre.Text;
                listaTareas[index].Descripcion = txtDescripcion.Text;
                listaTareas[index].Fecha = dtpFecha.Value;
                listaTareas[index].Lugar = txtLugar.Text;
                listaTareas[index].Estado = cmbEstado.SelectedItem.ToString();

                listaOriginal = new List<Tarea>(listaTareas);
                ActualizarGrid();
                MessageBox.Show("Tarea editada correctamente.");
            }
            else
            {
                MessageBox.Show("Por favor seleccione una tarea para editar.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTareas.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("¿Está seguro de que desea eliminar esta tarea?",
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    int index = dgvTareas.SelectedRows[0].Index;
                    listaTareas.RemoveAt(index);
                    listaOriginal = new List<Tarea>(listaTareas);
                    ActualizarGrid();
                    LimpiarCampos();
                    MessageBox.Show("Tarea eliminada correctamente.");
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una tarea para eliminar.");
            }
        }

        private void dgvTareas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < listaTareas.Count)
            {
                txtCodigo.Text = listaTareas[e.RowIndex].Codigo;
                txtNombre.Text = listaTareas[e.RowIndex].Nombre;
                txtDescripcion.Text = listaTareas[e.RowIndex].Descripcion;
                dtpFecha.Value = listaTareas[e.RowIndex].Fecha;
                txtLugar.Text = listaTareas[e.RowIndex].Lugar;
                cmbEstado.SelectedItem = listaTareas[e.RowIndex].Estado;
            }
        }

        private void btnBuscarCodigo_Click(object sender, EventArgs e)
        {
            string codigo = txtBuscarCodigo.Text.Trim();
            if (!string.IsNullOrEmpty(codigo))
            {
                // CORRECCIÓN: Usar ToUpper() para búsqueda case-insensitive
                var resultado = listaOriginal.Where(t => t.Codigo.ToUpper().Contains(codigo.ToUpper())).ToList();
                listaTareas = resultado;
                ActualizarGrid();
            }
        }

        private void btnBuscarEstado_Click(object sender, EventArgs e)
        {
            string estado = txtBuscarEstado.Text.Trim();
            if (!string.IsNullOrEmpty(estado))
            {
                // CORRECCIÓN: Usar ToUpper() para búsqueda case-insensitive
                var resultado = listaOriginal.Where(t => t.Estado.ToUpper().Contains(estado.ToUpper())).ToList();
                listaTareas = resultado;
                ActualizarGrid();
            }
        }

        private void btnBuscarFecha_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpFechaDesde.Value.Date;
            DateTime hasta = dtpFechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            var resultado = listaOriginal.Where(t => t.Fecha >= desde && t.Fecha <= hasta).ToList();
            listaTareas = resultado;
            ActualizarGrid();
        }

        private void btnLimpiarBusqueda_Click(object sender, EventArgs e)
        {
            listaTareas = new List<Tarea>(listaOriginal);
            ActualizarGrid();
            txtBuscarCodigo.Clear();
            txtBuscarEstado.Clear();
            dtpFechaDesde.Value = DateTime.Now;
            dtpFechaHasta.Value = DateTime.Now.AddDays(30);
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtLugar.Clear();
            cmbEstado.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Now;
        }
    }
}