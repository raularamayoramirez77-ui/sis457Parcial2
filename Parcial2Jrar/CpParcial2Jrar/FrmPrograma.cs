using ClnParcial2Lfms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpParcial2Jrar
{
    public partial class FrmPrograma : Form
    {
        private bool esNuevo = false;
        public FrmPrograma()
        {
            InitializeComponent();
        }
        private void listar()
        {
            var lista = ProgramaCln.listarPa(txtParametro.Text.Trim());
            dgvLista.DataSource = lista;
            dgvLista.Columns["id"].Visible = false;
            dgvLista.Columns["estado"].Visible = false;
            dgvLista.Columns["titulo"].HeaderText = "Título";
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            dgvLista.Columns["duracion"].HeaderText = "Duración (minutos)";
            dgvLista.Columns["productor"].HeaderText = "Productor";
            dgvLista.Columns["fechaEstreno"].HeaderText = "Fecha de Estreno";
            dgvLista.Columns["canal"].HeaderText = "Canal";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario de Registro";

            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["titulo"];
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }

        private void cargarCanal()
        {
            var listaCanales = CanalCln.listar();
            cbxCanal.DataSource = listaCanales;
            cbxCanal.DisplayMember = "nombre";
            cbxCanal.ValueMember = "id";
            cbxCanal.SelectedIndex = -1;
        }

        private void FrmPrograma_Load(object sender, EventArgs e)
        {
            cargarCanal();
            Size = new Size(615, 318);
            listar();
            gbxDatos.Enabled = false;
            pnlAcciones.Enabled = true;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            gbxDatos.Enabled = true;
            limpiar();
            txtTitulo.Focus();
            pnlAcciones.Enabled = false;
            Size = new Size(615, 464);

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validar()) return;
            if (cbxCanal.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un canal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxCanal.Focus();
                return;
            }
            var programa = new CadParcial2Jrar.Programa
            {
                titulo = txtTitulo.Text.Trim(),
                descripcion = txtDescripcion.Text.Trim(),
                duracion = (int)nudDuracion.Value,
                productor = txtProductor.Text.Trim(),
                fechaEstreno = dtpfechaEstreno.Value,
                idCanal = (int)cbxCanal.SelectedValue,
                usuarioRegistro = "admin",
                fechaRegistro = DateTime.Now,
                estado = 1
            };
            if (esNuevo)
            {
                ProgramaCln.insertar(programa);
                MessageBox.Show("Programa registrado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                programa.id = id;
                ProgramaCln.actualizar(programa);
                MessageBox.Show("Programa actualizado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            listar();
            Size = new Size(615, 318);
            pnlAcciones.Enabled = true;
            gbxDatos.Enabled = false;

            limpiar();
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Size = new Size(615, 318);
            gbxDatos.Enabled = false;
            limpiar();
            pnlAcciones.Enabled = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var resultado = MessageBox.Show("¿Está seguro de eliminar el programa seleccionado?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                ProgramaCln.eliminar(id, "admin");
                listar();
                MessageBox.Show("Programa eliminado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
        // Agregar el método limpiar en la clase FrmPrograma
        private void limpiar()
        {
            txtTitulo.Clear();
            txtDescripcion.Clear();
            nudDuracion.Value = nudDuracion.Minimum;
            txtProductor.Clear();
            dtpfechaEstreno.Value = DateTime.Now;
            cbxCanal.SelectedIndex = -1;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null) return;
            esNuevo = false;
            gbxDatos.Enabled = true;
            pnlAcciones.Enabled = false;
            Size = new Size(615, 464);

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var programa = ProgramaCln.obtenerUno(id);
            txtTitulo.Text = programa.titulo;
            txtDescripcion.Text = programa.descripcion;
            nudDuracion.Value = programa.duracion ?? nudDuracion.Minimum;
            txtProductor.Text = programa.productor;
            dtpfechaEstreno.Value = programa.fechaEstreno ?? DateTime.Now;
            cbxCanal.SelectedValue = programa.idCanal;
            txtTitulo.Focus();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        private void FrmPrograma_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) listar();

        }
        private bool validar()
        {
            bool esValido = true;
            erpTitulo.Clear();
            erpDescripcion.Clear();
            erpDuracion.Clear();
            erpProductor.Clear();
            erpCanal.Clear();
            erpfechaEstreno.Clear();

            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                erpTitulo.SetError(txtTitulo, "El título es obligatorio.");
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria.");
                esValido = false;
            }
            if (nudDuracion.Value <= 0)
            {
                erpDuracion.SetError(nudDuracion, "La duración debe ser mayor a cero.");
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtProductor.Text))
            {
                erpProductor.SetError(txtProductor, "El productor es obligatorio.");
                esValido = false;
            }
            if (cbxCanal.SelectedIndex == -1)
            {
                erpCanal.SetError(cbxCanal, "Debe seleccionar un canal.");
                esValido = false;
            }
            if (dtpfechaEstreno.Value > DateTime.Today)
            {
                erpfechaEstreno.SetError(dtpfechaEstreno, "La fecha de estreno no puede ser futura.");
                esValido = false;
            }
            return esValido;
        }
    }
}