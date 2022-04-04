using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Kamsyk.ExcelOpenXml;
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.ExtendedModel;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace OTISCZ.ScmDemand.UI.Common
{
    public class ScmExcel
    {
       
        public byte[] GenerateDemandRequestExcelForm(
            string demandNr,
            ScmUser scmUser,
            ObservableCollection<NomenclatureExtend> nomenclatures,
            string supplierName) {

           

            Excel excel = new Excel();
            using (var xlsDoc = excel.GetNewXlsDocMemory()) {
                var wbPart = excel.AddWorkbook(xlsDoc);

                #region Styles
                var stylesPart = xlsDoc.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = new Stylesheet();

                // blank font list
                stylesPart.Stylesheet.Fonts = new Fonts();
                stylesPart.Stylesheet.Fonts.AppendChild(new Font());

                Font headeFont = new Font(new Bold());
                headeFont.Append(new FontSize() { Val = 20D });
                stylesPart.Stylesheet.Fonts.AppendChild(headeFont);

                Font certifFont = new Font(new Bold());
                certifFont.Append(new FontSize() { Val = 14D });
                stylesPart.Stylesheet.Fonts.AppendChild(certifFont);

                stylesPart.Stylesheet.Fonts.Count = 3;

                // create fills
                stylesPart.Stylesheet.Fills = new Fills();

                // create a solid blue fill
                var solidBlue = new PatternFill() { PatternType = PatternValues.Solid };
                solidBlue.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("BBDEFB") }; // red fill
                solidBlue.BackgroundColor = new BackgroundColor { Indexed = 64 };

                // create approval only solid green fill
                var approvalOnlyGreen = new PatternFill() { PatternType = PatternValues.Solid };
                approvalOnlyGreen.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("00b050") }; // red fill
                approvalOnlyGreen.BackgroundColor = new BackgroundColor { Indexed = 64 };

                stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
                stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
                stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = solidBlue });
                stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = approvalOnlyGreen });
                stylesPart.Stylesheet.Fills.Count = 4;

                // blank border list
                stylesPart.Stylesheet.Borders = new Borders();
                stylesPart.Stylesheet.Borders.AppendChild(new Border());

                stylesPart.Stylesheet.Borders.AppendChild(GenerateBorder());
                stylesPart.Stylesheet.Borders.Count = 2;

                // blank cell format list
                stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
                stylesPart.Stylesheet.CellStyleFormats.Count = 1;
                stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

                // cell format list
                stylesPart.Stylesheet.CellFormats = new CellFormats();
                // empty one for index 0, seems to be required
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());


                //****************************************************************************************************************************************

                //Header Style
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 0, FillId = 0, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left });

                //Demand Number Style
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 1, BorderId = 1, FillId = 0, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left });

                //Item Title
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 1, FillId = 2, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left, WrapText = true });

                //Item Value
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 1, FillId = 0, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left, WrapText = true });

                //Item Value, Wrap
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 0, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left, WrapText = true });

                //Certificate Style
                stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 2, BorderId = 0, FillId = 0, ApplyFill = false }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Left });

                stylesPart.Stylesheet.CellFormats.Count = 7;

                //**********************************************************************************************************************************************

                stylesPart.Stylesheet.Save();
                #endregion

                var wsPart = excel.AddSheet(wbPart, "Demand");
                //var sheetdata = wsPart.Worksheet.GetFirstChild<SheetData>();
                              

                wsPart.Worksheet = new Worksheet(new SheetViews(new SheetView() { WorkbookViewId = 0, ShowGridLines = new BooleanValue(false) }), new SheetData());
                wsPart.Worksheet.Save();

                var sheetdata = wsPart.Worksheet.GetFirstChild<SheetData>();
                wsPart.Worksheet.InsertBefore(GetDemandColumns(), sheetdata);
                var ws = wsPart.Worksheet;

                int iRow = 1;

                var logoBmp = new BitmapImage(new Uri(@"pack://application:,,,/images/Request/Logo.png", UriKind.Absolute));
                var imgBytes = VmBase.GetBytesFromBitmapSource(logoBmp);

                FileInfo fi = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
                string imgLogoPath = Path.Combine(fi.Directory.FullName, "OtisLogo.png");

                File.WriteAllBytes(imgLogoPath, imgBytes);
                
                ExcelImage.AddImage(wsPart, imgLogoPath, "OtisLogo", 2, 1);

                iRow = 2;
                string cellAddress = "G" + iRow;
                var cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 2;
                excel.SetCellValue(cell, demandNr);
                cellAddress = "H" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 2;
                cellAddress = "I" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 2;
                Excel.MergeTwoCells(ws, "G" + iRow, "I" + iRow);

                iRow = 5;
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 1;
                excel.SetCellValue(cell, ScmResource.DocRfqDocument);
                     
                
                iRow = 7;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocProjectNr);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocProjectName);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocSupplierName, supplierName);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocSupplierOfferNr);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocOfferDeadline);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocEUR1Request);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocOfferFollows);
                iRow++;
                AddDocHeaderItem(excel, ws, iRow, ScmResource.DocRequestedCertificates);

                iRow++;
                iRow++;

                #region Requestor
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocDemandRequestor);
                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                Excel.MergeTwoCells(ws, "A" + iRow, "B" + iRow);

                cellAddress = "C" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocPhoneNr);

                cellAddress = "D" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocDepartment);

                cellAddress = "E" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocDemandDeadline);

                iRow++;

                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                excel.SetCellValue(cell, scmUser.surname + " " + scmUser.first_name);
                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                Excel.MergeTwoCells(ws, "A" + iRow, "B" + iRow);

                cellAddress = "C" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                excel.SetCellValue(cell, scmUser.phone_nr);

                cellAddress = "D" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                excel.SetCellValue(cell, "SCM");

                cellAddress = "E" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;

                #endregion

                iRow++;
                iRow++;

                #region Supplier
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocOfferCreatedBy);
                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                Excel.MergeTwoCells(ws, "A" + iRow, "B" + iRow);

                cellAddress = "C" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocPhoneNr);

                cellAddress = "D" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocDepartment);

                cellAddress = "E" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocOfferDeadline);

                iRow++;

                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                Excel.MergeTwoCells(ws, "A" + iRow, "B" + iRow);

                cellAddress = "C" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                
                cellAddress = "D" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;
                
                cellAddress = "E" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 4;

                #endregion

                iRow++;
                iRow++;

                #region Nomenclatures
                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                
                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocNomenclature);

                cellAddress = "C" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocPartNane);

                cellAddress = "D" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocOtisDrawNr);

                cellAddress = "E" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocSupplierPartNr);

                cellAddress = "F" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocPcs);

                cellAddress = "G" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocMj);

                cellAddress = "F" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocLT);

                cellAddress = "G" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocMOQ);

                cellAddress = "H" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocPriceUnit);

                cellAddress = "I" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 3;
                excel.SetCellValue(cell, ScmResource.DocCurrency);

                iRow++;

                for (int i=0; i<nomenclatures.Count; i++) {
                    cellAddress = "A" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;
                    excel.SetCellValue(cell, (i + 1).ToString());

                    cellAddress = "B" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;
                    excel.SetCellValue(cell, nomenclatures[i].nomenclature_key);

                    cellAddress = "C" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;
                    excel.SetCellValue(cell, nomenclatures[i].name);

                    cellAddress = "D" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;

                    cellAddress = "E" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;

                    cellAddress = "F" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;

                    cellAddress = "G" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;

                    cellAddress = "H" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;

                    cellAddress = "I" + iRow;
                    cell = excel.InsertCellInWorksheet(ws, cellAddress);
                    cell.StyleIndex = 4;

                    iRow++;
                }
                #endregion

                iRow++;

                #region Delivery Terms
                cellAddress = "B" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 5;
                excel.SetCellValue(cell, ScmResource.DocDeliveryConditions);

                cellAddress = "B" + (iRow + 1);
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 5;

                Excel.MergeTwoCells(ws, "B" + iRow, "B" + (iRow + 1));

                cellAddress = "C" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 5;
                excel.SetCellValue(cell, ScmResource.DocDapOtis);

                cellAddress = "C" + (iRow + 1);
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 5;

                Excel.MergeTwoCells(ws, "C" + iRow, "C" + (iRow + 1));
                #endregion

                iRow++;
                iRow++;
                iRow++;

                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 6;
                excel.SetCellValue(cell, ScmResource.DocCertificates);

                iRow++;
                iRow++;
                iRow++;
                iRow++;

                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 6;
                excel.SetCellValue(cell, ScmResource.DocOtherConditions);

                iRow++;

                cellAddress = "A" + iRow;
                cell = excel.InsertCellInWorksheet(ws, cellAddress);
                cell.StyleIndex = 0;
                excel.SetCellValue(cell, ScmResource.DocPaletsConditions);
            }

            var outputStream = excel.GetWorkbookMemoryStream();
            byte[] bytes = outputStream.ToArray();


            //byte[] bytes = new byte[outputStream.Length];
            //outputStream.Read(bytes, 0, outputStream.Length);

            return bytes;

        }

        private void AddDocHeaderItem(Excel excel, Worksheet ws, int iRow, string strTitle, string strValue) {
            string cellAddress = "A" + iRow;
            var cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = 3;
            excel.SetCellValue(cell, strTitle);
            cellAddress = "B" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = 3;
            Excel.MergeTwoCells(ws, "A" + iRow, "B" + iRow);

            cellAddress = "C" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = 4;
            if (!String.IsNullOrWhiteSpace(strValue)) {
                excel.SetCellValue(cell, strValue);
            }

            cellAddress = "D" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = 4;

            cellAddress = "E" + iRow;
            cell = excel.InsertCellInWorksheet(ws, cellAddress);
            cell.StyleIndex = 4;

            

            Excel.MergeTwoCells(ws, "C" + iRow, "E" + iRow);

            //cellAddress = "E" + iRow;
            //cell = excel.InsertCellInWorksheet(ws, cellAddress);
            //cell.StyleIndex = 4;

            //Excel.MergeTwoCells(ws, "C" + iRow, "E" + iRow);
        }

        private void AddDocHeaderItem(Excel excel, Worksheet ws, int iRow, string strTitle) {
            AddDocHeaderItem(excel, ws, iRow, strTitle, null);
        }

        private Columns GetDemandColumns() {
            Columns columns = new Columns();
            uint iIndex = Convert.ToUInt32(1);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 5));

            iIndex = Convert.ToUInt32(2);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            iIndex = Convert.ToUInt32(3);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            iIndex = Convert.ToUInt32(4);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 25));

            iIndex = Convert.ToUInt32(5);
            columns.Append(Excel.CreateColumnData(iIndex, iIndex, 15));

            return columns;
        }

        public Border GenerateBorder() {
            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color1 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color1);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color2 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color2);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color3);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color4);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            return border2;
        }
    }
}
