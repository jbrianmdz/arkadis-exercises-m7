using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;   // EPPlus 8

namespace HolaRevit
{
    // ==================================================================
    // CASO 3 - EXPORTAR A EXCEL
    // Crea el muro y exporta toda su información a un Excel en el
    // Escritorio del usuario.
    // ==================================================================
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        // Constructor estático: se ejecuta UNA SOLA VEZ por sesión.
        // En EPPlus 8 esta es la forma correcta de declarar la licencia
        // (la antigua ExcelPackage.LicenseContext está obsoleta).
        static Command()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Estudiante");
        }

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

            // Variables que llenaremos dentro de la transacción y
            // usaremos después para el Excel
            long idMuro = 0;
            string tipoNombre = "";
            string nivelNombre = "";
            double longitudM = 0, alturaM = 0, espesorMm = 0;
            double areaM2 = 0, volumenM3 = 0;

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
                    TaskDialog.Show("Error", "No se encontró nivel o tipo de muro.");
                    return Result.Failed;
                }

                Line linea = Line.CreateBound(
                    new XYZ(x1, y1, 0),
                    new XYZ(x2, y2, 0));

                Wall wall = Wall.Create(
                    doc, linea, wallType.Id, level.Id, altura, 0, false, false);

                doc.Regenerate();   // para que área y volumen se calculen

                // Recoger datos
                idMuro       = wall.Id.Value;
                tipoNombre   = wall.WallType.Name;
                nivelNombre  = level.Name;
                longitudM    = UnitUtils.ConvertFromInternalUnits(linea.Length, UnitTypeId.Meters);
                alturaM      = UnitUtils.ConvertFromInternalUnits(altura,        UnitTypeId.Meters);
                espesorMm    = UnitUtils.ConvertFromInternalUnits(wall.WallType.Width, UnitTypeId.Millimeters);
                areaM2       = UnitUtils.ConvertFromInternalUnits(
                                  wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble(),
                                  UnitTypeId.SquareMeters);
                volumenM3    = UnitUtils.ConvertFromInternalUnits(
                                  wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble(),
                                  UnitTypeId.CubicMeters);

                t.Commit();
            }

            // ===== EXPORTAR A EXCEL =====
            string ruta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Muro_Reporte.xlsx");

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Muro");

                // Encabezados (columna A) y valores (columna B)
                // Esto es un formato "vertical" — útil cuando exportas un
                // solo elemento. Para listas de muchos elementos, los
                // encabezados van en la fila 1 y los datos en filas 2+.
                ws.Cells["A1"].Value = "ID";              ws.Cells["B1"].Value = idMuro;
                ws.Cells["A2"].Value = "Tipo";            ws.Cells["B2"].Value = tipoNombre;
                ws.Cells["A3"].Value = "Nivel";           ws.Cells["B3"].Value = nivelNombre;
                ws.Cells["A4"].Value = "Inicio X (ft)";   ws.Cells["B4"].Value = x1;
                ws.Cells["A5"].Value = "Inicio Y (ft)";   ws.Cells["B5"].Value = y1;
                ws.Cells["A6"].Value = "Fin X (ft)";      ws.Cells["B6"].Value = x2;
                ws.Cells["A7"].Value = "Fin Y (ft)";      ws.Cells["B7"].Value = y2;
                ws.Cells["A8"].Value = "Longitud (m)";    ws.Cells["B8"].Value = Math.Round(longitudM, 2);
                ws.Cells["A9"].Value = "Altura (m)";      ws.Cells["B9"].Value = Math.Round(alturaM, 2);
                ws.Cells["A10"].Value = "Espesor (mm)";   ws.Cells["B10"].Value = Math.Round(espesorMm, 1);
                ws.Cells["A11"].Value = "Área (m²)";      ws.Cells["B11"].Value = Math.Round(areaM2, 2);
                ws.Cells["A12"].Value = "Volumen (m³)";   ws.Cells["B12"].Value = Math.Round(volumenM3, 3);

                // Auto-ajustar el ancho de las columnas
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                // Guardar el archivo en disco
                package.SaveAs(new FileInfo(ruta));
            }

            TaskDialog.Show("Crear Muro - Excel",
                $"Muro creado y exportado a:\n{ruta}");
            return Result.Succeeded;
        }
    }
}
