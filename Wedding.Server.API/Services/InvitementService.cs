using System.Drawing;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using QRCoder;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Services;

public interface IInvitementService
{
    string GenerateInvites(IEnumerable<Guest> guests, InvitementType type);
}

public enum InvitementType
{
    A4, A5
}

public class InvitementService : IInvitementService
{
    private readonly string A4_INVITE_BASE_PATH;
    private readonly string A4_INVITE_PATH;
    private readonly string A5_INVITE_BASE_PATH;
    private readonly string A5_INVITE_PATH;
    private readonly string QRCODE_INVITE_BASE_PATH;
    private readonly string FONT_PATH;

    public InvitementService()
    {
        A4_INVITE_BASE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Invitement", "Base", "a4.pdf");
        A5_INVITE_BASE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Invitement", "Base", "a5.pdf");
        QRCODE_INVITE_BASE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Invitement", "qr");
        A4_INVITE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Invitement", "a4");
        A5_INVITE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Invitement", "a5");

        #region Ensure path is created
        if (!Directory.Exists(QRCODE_INVITE_BASE_PATH))
            Directory.CreateDirectory(QRCODE_INVITE_BASE_PATH);
        if (!Directory.Exists(A4_INVITE_PATH))
            Directory.CreateDirectory(A4_INVITE_PATH);
        if (!Directory.Exists(A5_INVITE_PATH))
            Directory.CreateDirectory(A5_INVITE_PATH);
        #endregion
    }

    public string GenerateInvites(IEnumerable<Guest> guests, InvitementType type)
    {
        switch (type)
        {
            case InvitementType.A4: return GenerateInviteA4(guests);
            case InvitementType.A5: return GenerateInviteA5(guests);
            default: return "";
        }
    }

    private string GenerateInviteA4(IEnumerable<Guest> guests)
    {
        var files = Directory.GetFiles(A4_INVITE_PATH);
        if (files.Length > 0)
            files.ToList().ForEach(file => File.Delete(file));

        var array = guests.OrderBy(x => x.Name).ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (i % 2 != 0)
                continue;

            var left = array[i];
            var right = array.Length > i + 1 ? array[i + 1] : null;
            string invitePath = Path.Combine(A4_INVITE_PATH, $"invite-{(left?.Code ?? "undefined")}-{(right?.Code ?? "undefined")}.pdf");
            System.IO.File.Copy(A4_INVITE_BASE_PATH, invitePath, true);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(A4_INVITE_BASE_PATH), new PdfWriter(invitePath));
            Document document = new Document(pdfDocument);

            #region Add font to the document
            PdfFont fontLink = PdfFontFactory.CreateFont("C:/Users/Augusto Pereira/AppData/Local/Microsoft/Windows/Fonts/Raleway-Regular.ttf", "UTF-8", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED, pdfDocument);
            PdfFont fontGuestName = PdfFontFactory.CreateFont("C:/Users/Augusto Pereira/AppData/Local/Microsoft/Windows/Fonts/Raleway-SemiBold.ttf", "UTF-8", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED, pdfDocument);
            #endregion

            if (left != null)
            {
                EnsureQrCodeExists(left.Code);
                string leftQrCode = Path.Combine(QRCODE_INVITE_BASE_PATH, $"{left.Code}.png");

                #region Add qrcode
                ImageData imageData = ImageDataFactory.Create(leftQrCode);
                iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imageData);
                qrCode.SetFixedPosition(15 + 50, 422 + (420 - 35) / 2);
                qrCode.SetWidth(35);
                qrCode.SetHeight(35);
                document.Add(qrCode);
                #endregion

                #region Add qrcode legend
                iText.Layout.Element.Paragraph qrCodeLegend = new Paragraph("leguto.co/" + left.Code);
                qrCodeLegend.SetFixedPosition(15 + 35, 422 + 422 - 15, 390);
                qrCodeLegend.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                qrCodeLegend.SetFontSize(7);
                qrCodeLegend.SetFont(fontLink);
                qrCodeLegend.SetRotationAngle((Math.PI / 180) * -90);
                document.Add(qrCodeLegend);
                #endregion

                #region Add guest name
                if (left.Extensive)
                {
                    iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(left.Name));
                    guestName.SetFixedPosition(15 + 20, 422 + 422 - 15, 390);
                    guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    guestName.SetFontSize(8);
                    guestName.SetFont(fontGuestName);
                    guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                    guestName.SetRotationAngle((Math.PI / 180) * -90);
                    document.Add(guestName);

                    iText.Layout.Element.Paragraph extensivo = new Paragraph("EXTENSIVO AOS FILHOS CASADOS");
                    extensivo.SetFixedPosition(15 + 12, 422 + 422 - 15, 390);
                    extensivo.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    extensivo.SetFontSize(7);
                    extensivo.SetFont(fontLink);
                    extensivo.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                    extensivo.SetRotationAngle((Math.PI / 180) * -90);
                    document.Add(extensivo);
                }
                else
                {
                    iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(left.Name));
                    guestName.SetFixedPosition(15 + 20, 422 + 422 - 15, 390);
                    guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    guestName.SetFontSize(8);
                    guestName.SetFont(fontGuestName);
                    guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                    guestName.SetRotationAngle((Math.PI / 180) * -90);
                    document.Add(guestName);
                }
                #endregion
            }

            if (right != null)
            {
                EnsureQrCodeExists(right.Code);
                string rightQrCode = Path.Combine(QRCODE_INVITE_BASE_PATH, $"{right.Code}.png");

                #region Add qrcode
                ImageData imageData = ImageDataFactory.Create(rightQrCode);
                iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imageData);
                qrCode.SetFixedPosition(15 + 50, (420 - 35) / 2);
                qrCode.SetWidth(35);
                qrCode.SetHeight(35);
                document.Add(qrCode);
                #endregion

                #region Add qrcode legend
                iText.Layout.Element.Paragraph qrCodeLegend = new Paragraph("leguto.co/" + right.Code);
                qrCodeLegend.SetFixedPosition(15 + 35, 422 - 15, 390);
                qrCodeLegend.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                qrCodeLegend.SetFontSize(7);
                qrCodeLegend.SetFont(fontLink);
                qrCodeLegend.SetRotationAngle((Math.PI / 180) * -90);
                document.Add(qrCodeLegend);
                #endregion

                #region Add guest name
                if (right.Extensive)
                {
                    iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(right.Name));
                    guestName.SetFixedPosition(15 + 20, 422 - 15, 390);
                    guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    guestName.SetFontSize(8);
                    guestName.SetFont(fontGuestName);
                    guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                    guestName.SetRotationAngle((Math.PI / 180) * -90);
                    document.Add(guestName);

                    iText.Layout.Element.Paragraph extensivo = new Paragraph("EXTENSIVO AOS FILHOS CASADOS");
                    extensivo.SetFixedPosition(15 + 12, 422 - 15, 390);
                    extensivo.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    extensivo.SetFontSize(7);
                    extensivo.SetFont(fontLink);
                    extensivo.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                    extensivo.SetRotationAngle((Math.PI / 180) * -90);
                    document.Add(extensivo);
                }
                else
                {
                    iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(right.Name));
                    guestName.SetFixedPosition(15 + 20, 422 - 15, 390);
                    guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    guestName.SetFontSize(8);
                    guestName.SetFont(fontGuestName);
                    guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                    guestName.SetRotationAngle((Math.PI / 180) * -90);
                    document.Add(guestName);
                }
                #endregion
            }

            document.Close();
            Console.WriteLine($"Invitement to guest {left?.Code} and {(right?.Code ?? "undefined")} was successfully generated.");
        }

        #region Merge files
        string mergedFile = Path.Combine(A4_INVITE_PATH, "all-invites.pdf");
        var pdf = new PdfDocument(new PdfWriter(mergedFile));
        PdfMerger merger = new PdfMerger(pdf);

        var filesToMerge = Directory.GetFiles(A4_INVITE_PATH, "invite-*.pdf");
        foreach (var file in filesToMerge)
        {
            Console.WriteLine($"Merging file '{file}'.");
            var toMerge = new PdfDocument(new PdfReader(file));
            merger.Merge(toMerge, 1, toMerge.GetNumberOfPages());
            toMerge.Close();
        }
        pdf.Close();
        Console.WriteLine($"{filesToMerge.Length} files was successfully merged.");
        #endregion

        return mergedFile;
    }

    private string GenerateInviteA5(IEnumerable<Guest> guests)
    {
        var files = Directory.GetFiles(A5_INVITE_PATH);
        if (files.Length > 0)
            files.ToList().ForEach(file => File.Delete(file));

        foreach (var guest in guests.OrderBy(x => x.Name))
        {
            EnsureQrCodeExists(guest.Code);

            string invitePath = Path.Combine(A5_INVITE_PATH, "invite-" + guest.Code + ".pdf");
            string guestQrCode = Path.Combine(QRCODE_INVITE_BASE_PATH, $"{guest.Code}.png");

            System.IO.File.Copy(A5_INVITE_BASE_PATH, invitePath, true);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(A5_INVITE_BASE_PATH), new PdfWriter(invitePath));
            Document document = new Document(pdfDocument);

            #region Add font to the document
            PdfFont fontLink = PdfFontFactory.CreateFont("C:/Users/Augusto Pereira/AppData/Local/Microsoft/Windows/Fonts/Raleway-Regular.ttf", "UTF-8", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED, pdfDocument);
            PdfFont fontGuestName = PdfFontFactory.CreateFont("C:/Users/Augusto Pereira/AppData/Local/Microsoft/Windows/Fonts/Raleway-SemiBold.ttf", "UTF-8", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED, pdfDocument);
            #endregion

            #region Add qrcode
            ImageData imageData = ImageDataFactory.Create(guestQrCode);
            iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imageData);
            qrCode.SetFixedPosition(194, 60);
            qrCode.SetWidth(35);
            qrCode.SetHeight(35);
            document.Add(qrCode);
            #endregion

            #region Add qrcode legend
            iText.Layout.Element.Paragraph qrCodeLegend = new Paragraph("leguto.co/" + guest.Code);
            qrCodeLegend.SetFixedPosition(92, 50, 235);
            qrCodeLegend.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            qrCodeLegend.SetFontSize(7);
            qrCodeLegend.SetFont(fontLink);
            document.Add(qrCodeLegend);
            #endregion

            #region Add guest name
            if (guest.Extensive)
            {
                iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(guest.Name));
                guestName.SetFixedPosition(15, 36, 390);
                guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                guestName.SetFontSize(8);
                guestName.SetFont(fontGuestName);
                guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                document.Add(guestName);

                iText.Layout.Element.Paragraph extensivo = new Paragraph("EXTENSIVO AOS FILHOS CASADOS");
                extensivo.SetFixedPosition(15, 28, 390);
                extensivo.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                extensivo.SetFontSize(7);
                extensivo.SetFont(fontLink);
                extensivo.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                document.Add(extensivo);
            }
            else
            {
                iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(guest.Name));
                guestName.SetFixedPosition(15, 38, 390);
                guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                guestName.SetFontSize(8);
                guestName.SetFont(fontGuestName);
                guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                document.Add(guestName);
            }
            #endregion

            document.Close();
        }

        #region Merge files
        string mergedFile = Path.Combine(A5_INVITE_PATH, "all-invites.pdf");
        var pdf = new PdfDocument(new PdfWriter(mergedFile));
        PdfMerger merger = new PdfMerger(pdf);

        var filesToMerge = Directory.GetFiles(A5_INVITE_PATH, "invite-*.pdf");
        Console.WriteLine($"{filesToMerge.Length} files to merge.");
        foreach (var file in filesToMerge)
        {
            Console.WriteLine($"Merging file '{file}'.");
            var toMerge = new PdfDocument(new PdfReader(file));
            merger.Merge(toMerge, 1, toMerge.GetNumberOfPages());
            toMerge.Close();
        }
        pdf.Close();
        #endregion

        return mergedFile;
    }

    private void EnsureQrCodeExists(string code)
    {
        string url = "https://leguto.co/" + code;
        string path = Path.Combine(QRCODE_INVITE_BASE_PATH, $"{code}.png");
        if (!File.Exists(path))
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(path);
            Console.WriteLine($"Qr code with url {url} was successfully created.");
        }
    }

    private string RemoveSpecialChars(string value)
    {
        string comAcentos = "ÀÁÂÃÄÅÂÇÈÉÊËÌÍÎÏÒÓÔÕÖÙÚÛÜàáâãäåçèéêëìíîïòóôõöùúûüºª°&%/º-–()²”";
        string semAcentos = "AAAAAAACEEEEIIIIOOOOOUUUUaaaaaaceeeeiiiiooooouuuuoaoE          ";

        for (int i = 0; i < comAcentos.Length; i++)
        {
            value = value.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
        }
        value = value.Replace(System.Environment.NewLine, " ");
        return value;
    }
}