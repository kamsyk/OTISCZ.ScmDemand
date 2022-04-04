
using iTextSharp.text;
using iTextSharp.text.pdf;
using OTISCZ.Report.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTISCZ.Report
{
    public class PdfReportsEvent : PdfPageEventHelper
    {
        private PdfTemplate m_totalPages;
        private PdfTemplate m_generatedDateTime;
        private BaseFont m_fontTotalPages = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        private float m_FooterTextSize = 8;

        public override void OnEndPage(PdfWriter writer, Document pdfDocument)
        {
            base.OnEndPage(writer, pdfDocument);

            CreateOrderFooter(pdfDocument, writer);
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
            m_totalPages = writer.DirectContent.CreateTemplate(100, 100);
            m_totalPages.BoundingBox = new Rectangle(-20, -20, 100, 100);

            m_generatedDateTime = writer.DirectContent.CreateTemplate(165, 100);
            m_generatedDateTime.BoundingBox = new Rectangle(-20, -20, 165, 100);

            m_fontTotalPages = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            //total pages
            m_totalPages.BeginText();
            m_totalPages.SetFontAndSize(m_fontTotalPages, m_FooterTextSize);
            m_totalPages.SetTextMatrix(0, 0);
            int pageNumber = writer.PageNumber - 1;
            m_totalPages.ShowText(Convert.ToString(pageNumber));
            m_totalPages.EndText();

            //generated time stamp
            m_generatedDateTime.BeginText();
            m_generatedDateTime.SetFontAndSize(m_fontTotalPages, m_FooterTextSize);
            m_generatedDateTime.SetTextMatrix(0, 0);
            m_generatedDateTime.ShowText(GetTimeStamp());
            m_generatedDateTime.EndText();

        }

        private void CreateOrderFooter(Document pdfDoc, PdfWriter writer)
        {
            
            int textBasePos = 30;
            

            PdfContentByte cb = writer.DirectContent;
            cb.SaveState();
            //string text = "Strana (Page) " + writer.PageNumber + " / ";
            string text =ReportResource.Page + " " + writer.PageNumber + " / ";
            float textBase = pdfDoc.Bottom;

            cb.BeginText();
            cb.SetFontAndSize(m_fontTotalPages, m_FooterTextSize);

            float pageFromAdjust = m_fontTotalPages.GetWidthPoint(text, m_FooterTextSize);
            float totalNrAdjust = m_fontTotalPages.GetWidthPoint(" 00", m_FooterTextSize);
            pageFromAdjust = pageFromAdjust + totalNrAdjust;

            cb.SetTextMatrix(pdfDoc.Right - pageFromAdjust, textBasePos);
            cb.ShowText(text);
            cb.EndText();

            

            cb.AddTemplate(m_totalPages, pdfDoc.Right - totalNrAdjust, textBasePos);

            float dateTimeStampWidth = m_fontTotalPages.GetWidthPoint(GetTimeStamp(), m_FooterTextSize);
            float dateTimeStampLeft = (pdfDoc.PageSize.Width / 2) - (dateTimeStampWidth / 2);
            cb.AddTemplate(m_generatedDateTime, dateTimeStampLeft, textBasePos);

            cb.RestoreState();

        }

        private string GetTimeStamp()
        {
            
            return ReportResource.Generated + " " + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

    }
}
