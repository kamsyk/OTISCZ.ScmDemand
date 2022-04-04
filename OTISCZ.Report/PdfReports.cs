using OTISCZ.ScmDemand.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.VisualBasic;
using OTISCZ.Report.Resources;

namespace OTISCZ.Report
{
    public class PdfReports
    {

        #region Methods
        public string GenerateDemandReport(DemandReport demandReport) {


            //Otevření dokumentu
            string path = CleanReportFolder(demandReport);
            Document doc = new Document(PageSize.A4, 45f, 45f, 30f, 50f);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            writer.PageEvent = new PdfReportsEvent();
            doc.Open();


            //Fonty
            iTextSharp.text.Font mainFont = FontFactory.GetFont(FontFactory.HELVETICA, BaseFont.CP1250, 30, Font.NORMAL);
            iTextSharp.text.Font font2 = FontFactory.GetFont(FontFactory.HELVETICA, BaseFont.CP1250, 18, Font.NORMAL);
            iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.HELVETICA, BaseFont.CP1250, 12, Font.NORMAL);
            iTextSharp.text.Font font4 = FontFactory.GetFont(FontFactory.HELVETICA, BaseFont.CP1250, 12, Font.BOLD);
            iTextSharp.text.Font font5 = FontFactory.GetFont(FontFactory.HELVETICA, BaseFont.CP1250, 15, Font.BOLD);

            //Nadpisy a popisky
            Paragraph header = new Paragraph("SCM Demand Report", mainFont);
            Paragraph ReportNum = new Paragraph(ReportResource.Demand_num + ": " + demandReport.demand_nr, font2);
            Paragraph GeneratedDate = new Paragraph(ReportResource.Generated + ": " + DateTime.Now, font3);


            //   LOGO IMAGE

            if (!(Directory.Exists(GetPdfFilePath() + @"\logo")))
            {
                Directory.CreateDirectory(GetPdfFilePath() + @"\logo");

            }
            var oImgLogo = ReportResource.ResourceManager.GetObject("Logo");
            System.Drawing.Image imgLogo = (System.Drawing.Image)oImgLogo;
            imgLogo.Save(GetPdfFilePath() + @"\logo" + @"\Logo.pgn");
            Image img = Image.GetInstance(GetPdfFilePath() + "\\logo\\Logo.pgn");
            img.SetAbsolutePosition(46, 765);
            img.ScalePercent(75f, 75f);




            PdfPTable table1 = new PdfPTable(3);
            table1.WidthPercentage = 100;
          

            table1.DefaultCell.BorderWidth = 0;
            PdfPCell cell1 = new PdfPCell(new Phrase("SCM Referent: ", font3));
            cell1.BorderWidth = 0;
            table1.AddCell(cell1);
            cell1.BorderWidth = 0;
            table1.AddCell(demandReport.requestor_name);
            cell1.BorderWidth = 0;
            table1.AddCell("");
            cell1.BorderWidth = 0;
            table1.AddCell(ReportResource.Supplier);
            cell1.BorderWidth = 0;
            cell1 = new PdfPCell(new Phrase(demandReport.supplier_text, font3));
            cell1.BorderWidth = 0;
            cell1.Colspan = 2;
            table1.AddCell(cell1);
            cell1.BorderWidth = 0;
            table1.AddCell(ReportResource.Status + ": ");
            cell1.BorderWidth = 0;
            cell1 = new PdfPCell(new Phrase(demandReport.status_text, font3));
            cell1.BorderWidth = 0;
            table1.AddCell(cell1);
            cell1.BorderWidth = 0;
            table1.AddCell("");
            cell1.BorderWidth = 0;
            cell1 = new PdfPCell(new Phrase(ReportResource.Demand_created, font3));
            cell1.BorderWidth = 0;
            table1.AddCell(cell1);
            cell1.BorderWidth = 0;
            table1.AddCell(demandReport.created_date_text);
            cell1.BorderWidth = 0;
            table1.AddCell("");
            




            // Zarovnání textu
            header.Alignment = PdfPCell.ALIGN_CENTER;
            ReportNum.Alignment = PdfPCell.ALIGN_CENTER;
            GeneratedDate.Alignment = PdfCell.ALIGN_CENTER;
            table1.HorizontalAlignment = PdfCell.ALIGN_LEFT;


            // Tabulka
            PdfPTable table = new PdfPTable(4);
            table.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            table.WidthPercentage = 100;
            
            float[] columnWidth = new float[] { 30, 35, 15, 20 };

            table.SetWidths(columnWidth);

            PdfPCell cell = new PdfPCell(new Phrase(ReportResource.Nomenclatures, font4));
            cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(ReportResource.Name, font4));
            cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(ReportResource.Price_offer, font4));
            cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(ReportResource.Status, font4));
            cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
            table.AddCell(cell);


            foreach (var nomenclture in demandReport.demand_nomenclatures_extend) {
                cell = new PdfPCell(new Phrase(nomenclture.nomenclature_key, font3));
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(nomenclture.name, font3));
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(nomenclture.price_text + " " + nomenclture.currency_text, font3));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(nomenclture.status_text, font3));
                table.AddCell(cell);

            }


            //Vložení objektů do souboru
            table1.SpacingBefore = 30;
            table.SpacingBefore = 30;
            doc.Add(img);
            doc.Add(header);
            doc.Add(ReportNum);
            doc.Add(table1);
            doc.Add(table);

            GenerateHistory(doc, demandReport, font3, font4, font5);

            Remarks(doc,demandReport,font3,font4, font5);

            doc.Close();

            return path;
        }

        private void GenerateHistory(Document doc, DemandReport demandReport, Font font, Font fontBold, Font fontHeader) {

            foreach (var demNom in demandReport.demand_nomenclatures_extend)
            {
                PdfPTable HistoryTable = new PdfPTable(4);
                HistoryTable.WidthPercentage = 100;


                float[] columnWidth = new float[] { 30, 30, 30, 10 };
                HistoryTable.SetWidths(columnWidth);
                HistoryTable.SpacingBefore = 30;
                HistoryTable.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                //doc.Add(Chunk.NEWLINE);
                PdfPCell cell = new PdfPCell(new Phrase(ReportResource.Nomenclature + ": " + demNom.nomenclature_key, fontHeader));
                cell.Colspan = 4;
                cell.BorderWidth = 0;
                HistoryTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(ReportResource.Status, fontBold));
                cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
                HistoryTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(ReportResource.Date, fontBold));
                cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
                HistoryTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(ReportResource.Changed, fontBold));
                cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
                HistoryTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(ReportResource.Days, fontBold));
                cell.BackgroundColor = new iTextSharp.text.Color(196, 196, 196);
                HistoryTable.AddCell(cell);
                //doc.Add(HistoryTable);


                int nomenclatureId = demNom.nomenclature_id;
                var filterHistory = (from demNomDb in demandReport.demand_nomenclatures_history
                                     where demNomDb.nomenclature_id == nomenclatureId
                                     orderby demNomDb.modif_date ascending
                                     select demNomDb).ToList();
                DateTime MainDate = filterHistory[0].modif_date;
                foreach (var nomHistory in filterHistory) {

                    TimeSpan timeSpan = nomHistory.modif_date - MainDate;

                    cell = new PdfPCell(new Phrase(nomHistory.status_text, font));
                    HistoryTable.AddCell(cell);
                    string strDate = nomHistory.modif_date.ToString(ReportResource.Date_Format);
                    cell = new PdfPCell(new Phrase(strDate, font));
                    HistoryTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(nomHistory.modif_user_name, font));
                    HistoryTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase((timeSpan.Days).ToString(), font));
                    HistoryTable.AddCell(cell);

                }
                doc.Add(HistoryTable);
            }

        }
        private void Remarks(Document doc, DemandReport demandReport,Font font, Font fontBold, Font fontHeader)
        {
            if (demandReport.remarks != null) { 
                Paragraph header = new Paragraph(ReportResource.Remarks+": ",fontHeader);
                header.SpacingBefore = 20;
                doc.Add(header);
                foreach (var remark in demandReport.remarks)
                {
                    Paragraph remarkUserName = new Paragraph(remark.user_firstname + " " + remark.user_surname + " - " + remark.modif_date.ToString(ReportResource.Date_Format) + ": ", fontBold);
                    Paragraph remarkText = new Paragraph(remark.remark_text, font);
                    remarkUserName.SpacingBefore = 10;
                    doc.Add(remarkUserName);
                    doc.Add(remarkText);
                }
            }
        } 


        private string CleanReportFolder(DemandReport demandReport) {

            int i = 0;
            string path = GetPdfFilePath()+"\\Reports";
            string NewPath = "";

            // Vytvoření adresáře
            if (!(Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
            }
            var pole = Directory.GetFiles(path);

            
            // Smazání starých souborů
            for (int y = 0; y<pole.Length;y++) 
            {
                FileInfo fileInfo = new FileInfo(pole[y]);
                DateTime lastWriteTime = fileInfo.LastWriteTime;
                DateTime WeekAgo = DateTime.Today.AddDays(-7);
                if (lastWriteTime < WeekAgo)
                {
                    File.Delete(pole[y]);
                }
            }

            // Vytvoření kopií souborů
            if (File.Exists(path +"\\"+demandReport.demand_nr+".pdf")) 
            {
                for (int z = 0; z < pole.Length; z++)
                {
                    NewPath = path + "\\" + demandReport.demand_nr + "_" + (++i) + ".pdf";
                    if (!(File.Exists(NewPath)))
                    {
                        break;
                    }
                    if (File.Exists(NewPath))
                    {
                        NewPath = path;
                    }
                }
            }
            else
            {
                NewPath = path + "\\" + demandReport.demand_nr + ".pdf";
            }
            return NewPath;
        }

            
        private string GetPdfFilePath() {
            return Path.GetDirectoryName(Directory.GetCurrentDirectory())+@"\Debug";
        }
        

        #endregion

    }
}
