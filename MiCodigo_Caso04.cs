using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Linq;
using System.Windows;            // WPF — requiere <UseWPF>true</UseWPF>
using System.Windows.Controls;
using System.Windows.Interop;    // Para que la ventana se quede sobre Revit

namespace PrimerEjercicio
{
    // ==================================================================
    // CASO 4 - VENTANA PARA DIGITAR EL NIVEL
    // Al ejecutar el comando aparece una ventana WPF con un TextBox
    // para que el usuario escriba el nombre del nivel donde crear el muro.
    // Construida 100% por código (sin XAML) para no añadir archivos.
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

            // ===== 1. CREAR LA VENTANA WPF =====
            Window win = new Window
            {
                Title = "Crear Muro - Elegir Nivel",
                Width = 380,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            // Hacer que la ventana sea propiedad de Revit (se queda encima)
            new WindowInteropHelper(win).Owner =
                System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            // ===== 2. LAYOUT (StackPanel = controles apilados verticalmente) =====
            StackPanel panel = new StackPanel { Margin = new Thickness(15) };

            panel.Children.Add(new TextBlock
            {
                Text = "Escribe el nombre EXACTO del nivel:",
                Margin = new Thickness(0, 0, 0, 5)
            });

            System.Windows.Controls.TextBox txtNivel = new System.Windows.Controls.TextBox
            {
                Text = "L1 - Block 35",   // valor por defecto
                Height = 25,
                Padding = new Thickness(3)
            };
            panel.Children.Add(txtNivel);

            System.Windows.Controls.Button btnOk = new System.Windows.Controls.Button
            {
                Content = "Crear Muro",
                Height = 30,
                Margin = new Thickness(0, 15, 0, 0)
            };
            panel.Children.Add(btnOk);

            // Variable donde guardamos lo que escribió el usuario.
            // Se usa fuera del lambda, por eso la declaramos antes.
            string nombreNivel = null;

            // Evento click del botón: guardar el texto y cerrar la ventana
            btnOk.Click += (s, e) =>
            {
                nombreNivel = txtNivel.Text;
                win.Close();
            };

            win.Content = panel;

            // ShowDialog() = MODAL (bloquea hasta que se cierre).
            // Es seguro llamarlo desde Execute, no necesitamos ExternalEvent.
            win.ShowDialog();

            // ===== 3. SI EL USUARIO NO ESCRIBIÓ NADA, SALIMOS =====
            if (string.IsNullOrWhiteSpace(nombreNivel))
            {
                return Result.Cancelled;
            }

            // ===== 4. CREAR EL MURO CON EL NIVEL DIGITADO =====
            double x1 = 100.0, y1 = -50.0;
            double x2 = 120.0, y2 = -50.0;
            double altura = 10.0;

            string resultado;

            using (Transaction t = new Transaction(doc, "Crear Muro"))
            {
                t.Start();

                // Buscar el nivel con el nombre que el usuario escribió
                Level level = new FilteredElementCollector(doc)
                                .OfClass(typeof(Level))
                                .Cast<Level>()
                                .FirstOrDefault(l => l.Name == nombreNivel);

                WallType wallType = new FilteredElementCollector(doc)
                                      .OfClass(typeof(WallType))
                                      .Cast<WallType>()
                                      .FirstOrDefault();

                if (level == null)
                {
                    t.RollBack();
                    resultado = $"No se encontró el nivel '{nombreNivel}'.\n\n" +
                                "Verifica el nombre exacto en el Project Browser.";
                }
                else if (wallType == null)
                {
                    t.RollBack();
                    resultado = "No hay tipos de muro disponibles en el proyecto.";
                }
                else
                {
                    Line linea = Line.CreateBound(
                        new XYZ(x1, y1, 0),
                        new XYZ(x2, y2, 0));

                    Wall wall = Wall.Create(
                        doc, linea, wallType.Id, level.Id, altura, 0, false, false);

                    t.Commit();

                    resultado = $"Muro creado en el nivel '{level.Name}'.\n" +
                                $"Tipo: {wall.WallType.Name}\n" +
                                $"Longitud: {linea.Length:F2} ft\n" +
                                $"Altura: {altura:F2} ft";
                }
            }

            Autodesk.Revit.UI.TaskDialog.Show("Crear Muro - Resultado", resultado);
            return Result.Succeeded;
        }
    }
}
