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
                Model.imagene oImage = new Model.imagene();
                oImage.nombre = txtNombre.Text.Trim();
                oImage.img = file;

                db.imagenes.Add(oImage);
                db.SaveChanges();
            }
        }
    }
}
