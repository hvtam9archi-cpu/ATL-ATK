using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using ATL_ATK.Models;
using ATL_ATK.Commands;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ATL_ATK.Services
{
    /// <summary>
    /// Service hỗ trợ cập nhật lại các bảng thống kê ATK đang có trong bản vẽ.
    /// </summary>
    public static class TableUpdateService
    {
        public static void UpdateAllAtkTablesWithData(Document doc, List<List<string>> fullTableData)
        {
            try
            {
                Database database = doc.Database;
                List<ObjectId> atkTables = new List<ObjectId>();
                
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(database.CurrentSpaceId, OpenMode.ForRead);
                    foreach (ObjectId id in btr)
                    {
                        if (id.ObjectClass.DxfName == "ACAD_TABLE")
                        {
                            Table table = tr.GetObject(id, OpenMode.ForRead) as Table;
                            if (table != null && table.XData != null)
                            {
                                var tvs = table.XData.AsArray();
                                if (tvs.Length > 0 && tvs[0].TypeCode == 1001 && tvs[0].Value.ToString() == "ATL_ATK_TABLE")
                                {
                                    atkTables.Add(id);
                                }
                            }
                        }
                    }
                    tr.Commit();
                }

                if (atkTables.Count == 0) return;

                // Cập nhật tất cả các bảng ATK trong bản vẽ
                foreach (ObjectId tableId in atkTables)
                {
                    TableGeneratorService.UpdateTableData(database, tableId, fullTableData);
                }
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage(
                    $"\n[ATL-ATK] TableUpdateService Error: {ex.Message}");
            }
        }
    }
}
