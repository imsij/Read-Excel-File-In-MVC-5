using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReadExcelFile.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Uploads()
        {
            List<ExcelData> excelData = new List<ExcelData>();
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        var newName = fname.Split('.');
                        fname = newName[0] + "_" + DateTime.Now.Ticks.ToString() + "." + newName[1];
                        var uploadRootFolderInput = AppDomain.CurrentDomain.BaseDirectory + "\\Uploads";
                        Directory.CreateDirectory(uploadRootFolderInput);
                        var directoryFullPathInput = uploadRootFolderInput;
                        fname = Path.Combine(directoryFullPathInput, fname);
                        file.SaveAs(fname);
                        excelData = readXLS(fname);
                    }
                    return Json(excelData);
                }
                catch (Exception ex)
                {
                    return Json(excelData);
                }
            }
            else
            {
                return Json(excelData);
            }
        }
        public List<ExcelData> readXLS(string FilePath)
        {
            List<ExcelData> excelData = new List<ExcelData>();
            FileInfo existingFile = new FileInfo(FilePath);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.End.Row;
                for (int row = 2; row <= rowCount; row++)
                {
                    excelData.Add(new ExcelData()
                    {
                        Date = worksheet.Cells[row, 1].Value.ToString().Trim(),
                        Task = worksheet.Cells[row, 2].Value.ToString().Trim(),
                        Time = worksheet.Cells[row, 3].Value.ToString().Trim()
                    });
                }
            }
            return excelData;
        }
    }
    public class ExcelData
    {
        public string Date { get; set; }
        public string Task { get; set; }
        public string Time { get; set; }
    }
}