using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Linq;
using System.Text;

namespace HolaRevit
{
    // ==================================================================
    // CASO 2 - DETALLADO
    // Crea el muro y muestra una ventana emergente (TaskDialog) con
    // TODA la información del muro: id, tipo, nivel, coordenadas,
    // longitud, altura, área y volumen — convertidos a metros y mm.
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

            double x1 = 100.0, y1 = -50.0;
            double x2 = 120.0, y2 = -50.0;
            double altura = 10.0;

            // StringBuilder = forma eficiente de armar texto en varias líneas
            StringBuilder info = new StringBuilder();

            using (Transaction t = new Transaction(doc, "Crear Muro"))
            {
                t.Start();

                Level level = new FilteredElementCollector(doc)
                                .OfClass(typeof(Level))
                                .Cast<Level>()
                                .FirstOrDefault(l => l.Name == "L1 - Block 35");

                WallType wallType = new FilteredElementCollector(doc)
                                      .OfClass(typeof(WallType))
                                      .Cast<WallType>()
                                      .FirstOrDefault();

                if (level == null || wallType == null)
                {
                    t.RollBack();
                    Autodesk.Revit.UI.TaskDialog.Show("Error", "No se encontró nivel o tipo de muro.");
                    return Result.Failed;
                }

                Line linea = Line.CreateBound(
                    new XYZ(x1, y1, 0),
                    new XYZ(x2, y2, 0));

                Wall wall = Wall.Create(
                    doc, linea, wallType.Id, level.Id, altura, 0, false, false);

                // Forzar regenerar el modelo para que los parámetros calculados
                // (área, volumen) tengan valor válido antes de leerlos
                doc.Regenerate();

                // ===== LEER PARÁMETROS DEL MURO =====

                // Id (long) — desde Revit 2024 IntegerValue está obsoleto
                long idMuro = wall.Id.Value;

                // Tipo de muro (espesor incluido)
                string tipoNombre = wall.WallType.Name;
                double espesorFt = wall.WallType.Width;

                // Longitud — viene de la curva base
                double longitudFt = linea.Length;

                // Altura del muro (parámetro de instancia)
                double alturaFt = wall.get_Parameter(
                    BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

                // Área y Volumen (parámetros calculados por Revit)
                double areaSqFt = wall.get_Parameter(
                    BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
                double volumenCuFt = wall.get_Parameter(
                    BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();

                // ===== CONVERTIR A UNIDADES SI =====
                // UnitUtils convierte de pies a metros, mm, m², m³, etc.
                double longitudM = UnitUtils.ConvertFromInternalUnits(longitudFt, UnitTypeId.Meters);
                double alturaM = UnitUtils.ConvertFromInternalUnits(alturaFt, UnitTypeId.Meters);
                double espesorMm = UnitUtils.ConvertFromInternalUnits(espesorFt, UnitTypeId.Millimeters);
                double areaM2 = UnitUtils.ConvertFromInternalUnits(areaSqFt, UnitTypeId.SquareMeters);
                double volumenM3 = UnitUtils.ConvertFromInternalUnits(volumenCuFt, UnitTypeId.CubicMeters);

                // ===== ARMAR EL MENSAJE =====
                info.AppendLine("=== MURO CREADO ===");
                info.AppendLine();
                info.AppendLine($"ID:        {idMuro}");
                info.AppendLine($"Tipo:      {tipoNombre}");
                info.AppendLine($"Nivel:     {level.Name}");
                info.AppendLine();
                info.AppendLine($"Inicio:    ({x1:F2}, {y1:F2}) ft");
                info.AppendLine($"Fin:       ({x2:F2}, {y2:F2}) ft");
                info.AppendLine();
                info.AppendLine($"Longitud:  {longitudM:F2} m");
                info.AppendLine($"Altura:    {alturaM:F2} m");
                info.AppendLine($"Espesor:   {espesorMm:F1} mm");
                info.AppendLine($"Área:      {areaM2:F2} m²");
                info.AppendLine($"Volumen:   {volumenM3:F3} m³");

                t.Commit();
            }

            Autodesk.Revit.UI.TaskDialog.Show("Crear Muro - Detallado", info.ToString());
            return Result.Succeeded;
        }
    }
}
