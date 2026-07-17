using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;   // WinForms — requiere <UseWindowsForms>true</UseWindowsForms>

namespace PrimerEjercicio
{
    // ==================================================================
    // CASO 5 - DROPDOWN DE NIVELES CON WinForms
    // Al ejecutar el comando aparece una ventana WinForms con un
    // ComboBox (desplegable) que muestra TODOS los niveles del proyecto.
    // El usuario selecciona uno y se crea el muro en ese nivel.
    //
    // NOTA: como el proyecto tiene WPF y WinForms activos a la vez,
    // los tipos ambiguos (Label, Button) van calificados con
    // System.Windows.Forms. para evitar el error CS0104.
    // ==================================================================
    [Transaction(TransactionMode.Manual)]
    public class MiCodigo : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // ===== 1. OBTENER TODOS LOS NIVELES DEL PROYECTO =====
            // Los ordenamos por elevación (de abajo arriba) para que
            // aparezcan en orden lógico en el desplegable.
            List<Level> niveles = new FilteredElementCollector(doc)
                                    .OfClass(typeof(Level))
                                    .Cast<Level>()
                                    .OrderBy(l => l.Elevation)
                                    .ToList();

            if (niveles.Count == 0)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Error", "No hay niveles en el proyecto.");
                return Result.Failed;
            }

            // ===== 2. CREAR EL FORMULARIO WinForms =====
            System.Windows.Forms.Form form = new System.Windows.Forms.Form
            {
                Text = "Crear Muro - Seleccionar Nivel",
                Width = 380,
                Height = 200,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Etiqueta arriba
            System.Windows.Forms.Label lbl = new System.Windows.Forms.Label
            {
                Text = "Selecciona el nivel:",
                Left = 15,
                Top = 15,
                Width = 330
            };
            form.Controls.Add(lbl);

            // ComboBox (desplegable)
            System.Windows.Forms.ComboBox combo = new System.Windows.Forms.ComboBox
            {
                Left = 15,
                Top = 40,
                Width = 330,
                // DropDownList = el usuario solo puede ELEGIR opciones,
                // no puede escribir texto libre.
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Llenar el combo con los nombres de los niveles
            foreach (Level l in niveles)
            {
                combo.Items.Add(l.Name);
            }
            combo.SelectedIndex = 0;   // seleccionar el primero por defecto
            form.Controls.Add(combo);

            // Botón "Crear"
            System.Windows.Forms.Button btnOk = new System.Windows.Forms.Button
            {
                Text = "Crear Muro",
                Left = 15,
                Top = 85,
                Width = 100
            };
            form.Controls.Add(btnOk);

            // Botón "Cancelar"
            System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button
            {
                Text = "Cancelar",
                Left = 245,
                Top = 85,
                Width = 100
            };
            form.Controls.Add(btnCancel);

            // Variable donde guardamos el nivel elegido por el usuario
            Level nivelSeleccionado = null;

            // Eventos de los botones
            btnOk.Click += (s, e) =>
            {
                // combo.SelectedIndex es la posición del item elegido (0, 1, 2...)
                // niveles[indice] nos da el Level real
                nivelSeleccionado = niveles[combo.SelectedIndex];
                form.Close();
            };

            btnCancel.Click += (s, e) =>
            {
                form.Close();   // sin asignar nivelSeleccionado, queda en null
            };

            // ShowDialog = modal, bloquea hasta que se cierre
            form.ShowDialog();

            // ===== 3. SI EL USUARIO CANCELÓ, SALIR =====
            if (nivelSeleccionado == null)
            {
                return Result.Cancelled;
            }

            // ===== 4. CREAR EL MURO EN EL NIVEL ELEGIDO =====
            double x1 = 100.0, y1 = -50.0;
            double x2 = 120.0, y2 = -50.0;
            double altura = 10.0;

            string resultado;

            using (Transaction t = new Transaction(doc, "Crear Muro"))
            {
                t.Start();

                WallType wallType = new FilteredElementCollector(doc)
                                      .OfClass(typeof(WallType))
                                      .Cast<WallType>()
                                      .FirstOrDefault();

                if (wallType == null)
                {
                    t.RollBack();
                    resultado = "No hay tipos de muro disponibles.";
                }
                else
                {
                    Line linea = Line.CreateBound(
                        new XYZ(x1, y1, 0),
                        new XYZ(x2, y2, 0));

                    Wall wall = Wall.Create(
                        doc, linea, wallType.Id, nivelSeleccionado.Id,
                        altura, 0, false, false);

                    t.Commit();

                    resultado = $"Muro creado en el nivel '{nivelSeleccionado.Name}'.\n" +
                                $"Tipo: {wall.WallType.Name}\n" +
                                $"Longitud: {linea.Length:F2} ft\n" +
                                $"Altura: {altura:F2} ft\n" +
                                $"ID: {wall.Id.Value}";
                }
            }

            Autodesk.Revit.UI.TaskDialog.Show("Crear Muro - Resultado", resultado);
            return Result.Succeeded;
        }
    }
}
