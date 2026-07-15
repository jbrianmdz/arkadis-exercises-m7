using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Linq;

namespace HolaRevit
{
    // ==================================================================
    // CASO 1 - BÁSICO
    // El más simple. Crea el muro y muestra info básica:
    // tipo de muro y dimensiones. Sin validaciones ni manejo de errores.
    // ==================================================================
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Coordenadas del muro (en pies)
            double x1 = 100.0, y1 = -50.0;
            double x2 = 120.0, y2 = -50.0;
            double altura = 10.0;

            // Variables para guardar info del muro y mostrarla después
            string tipoNombre = "";
            double longitudFt = 0;

            using (Transaction t = new Transaction(doc, "Crear Muro"))
            {
                t.Start();

                // Buscar nivel y tipo de muro
                Level level = new FilteredElementCollector(doc)
                                .OfClass(typeof(Level))
                                .Cast<Level>()
                                .FirstOrDefault(l => l.Name == "L1 - Block 35");

                WallType wallType = new FilteredElementCollector(doc)
                                      .OfClass(typeof(WallType))
                                      .Cast<WallType>()
                                      .FirstOrDefault();

                // Crear la línea base
                Line linea = Line.CreateBound(
                    new XYZ(x1, y1, 0),
                    new XYZ(x2, y2, 0));

                // Crear el muro
                Wall wall = Wall.Create(
                    doc, linea, wallType.Id, level.Id, altura, 0, false, false);

                // Guardar info para mostrarla después del Commit
                tipoNombre = wall.WallType.Name;
                longitudFt = linea.Length;   // longitud de la línea = longitud del muro

                t.Commit();
            }

            // Mensaje simple con la info clave
            string mensaje =
                $"Muro creado.\n\n" +
                $"Tipo: {tipoNombre}\n" +
                $"Longitud: {longitudFt:F2} ft\n" +
                $"Altura: {altura:F2} ft";

            Autodesk.Revit.UI.TaskDialog.Show("Crear Muro - Básico", mensaje);
            return Result.Succeeded;
        }
    }
}
