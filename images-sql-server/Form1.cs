using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; //libreria para abrir archivos
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace images_sql_server
{
    public partial class frmUploadImages : Form
    {
        public frmUploadImages()
        {
            InitializeComponent();
        }

        //se cargan/abren las imagenes
        private void btnAbrir_Click(object sender, EventArgs e)
        {
            //directorio inicial para busqueda
            openFileDialogBuscarImagen.InitialDirectory = "c:\\";
            //tipo de imagenes a cargar
            openFileDialogBuscarImagen.Filter = "Archivos jpg(*.jpg)|*.jpg|Archivos png(*.png)|*.png";
            openFileDialogBuscarImagen.FilterIndex = 1;

            //restaurar cache y que cargue el mismo directorio en caso de cargar varias veces
            openFileDialogBuscarImagen.RestoreDirectory = true;

            if(openFileDialogBuscarImagen.ShowDialog() == DialogResult.OK)
            {
                txtArchivo.Text = openFileDialogBuscarImagen.FileName;
            }
        }

        //recargar la lista con los datos
        private void Refresh()
        {
            using (Model.archivosDBEntities db = new Model.archivosDBEntities())
            {
                var lst = from d in db.imagenes1
                          select new { d.id, d.nombre };
                dgvLista.DataSource = lst.ToList();

                //limpiar cajas de texto
                txtArchivo.Text = "";
                txtNombre.Text = "";
            }
        }


        //se guardan las imagenes en la base de datos
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //valida que se haya asignado un nombre y seleccionado un archivo
            if (txtNombre.Text.Trim().Equals("") || txtArchivo.Text.Trim().Equals(""))
            {
                MessageBox.Show("Nombre y archivo son obligatorios");
                return;
            }

            //generar matriz de bytes
            byte[] file = null;
            Stream myStream = openFileDialogBuscarImagen.OpenFile();
            //convertir matriz de bytes
            using (MemoryStream ms = new MemoryStream())
            {
                myStream.CopyTo(ms);
                file = ms.ToArray();
            }

            //guardar en la base de datos
            using (Model.archivosDBEntities db = new Model.archivosDBEntities())
            {
                Model.imagenes oImage = new Model.imagenes();
                oImage.nombre = txtNombre.Text.Trim();
                oImage.img = file;

                db.imagenes1.Add(oImage);
                db.SaveChanges();
            }

            Refresh(); //Recargar la lista con los datos de la BD
        }

        //mostrar la imagen que se selecciona del grid
        private void btnMostrar_Click(object sender, EventArgs e)
        {
            if(dgvLista.Rows.Count > 0)
            {
                int id = int.Parse(dgvLista.Rows[dgvLista.CurrentRow.Index].Cells[0].Value.ToString());

                using (Model.archivosDBEntities db = new Model.archivosDBEntities())
                {
                    var oImage = db.imagenes1.Find(id);

                    Stream ms = new MemoryStream(oImage.img);
                    Bitmap bmp = new Bitmap(ms);
                    pbImg.Image = bmp;
                }
            }

            Refresh();
        }
    }
}
