using System.Drawing;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using Wedding.Server.API.Controllers.Base;
using Wedding.Server.API.Controllers.DefaultResponses;
using Wedding.Server.API.Controllers.Requests.Guest;
using Wedding.Server.API.Controllers.Responses.Guest;
using Wedding.Server.API.Data.Repositories;
using Wedding.Server.API.Models;
using Wedding.Server.API.Services;

namespace Wedding.Server.API.Controllers;

public class GuestController : APIControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IGuestRepository _guestRepository;
    private readonly IGuestAccessRepository _guestAccessRepository;

    public GuestController(IGuestRepository guestRepository, ITokenService tokenService, IGuestAccessRepository guestAccessRepository)
    {
        _guestRepository = guestRepository;
        _tokenService = tokenService;
        _guestAccessRepository = guestAccessRepository;
    }

    [Authorize]
    [HttpGet("{code}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guest), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> OnGetAsync([FromRoute] string code)
    {
        var guest = await _guestRepository.SelectAsync(code);
        if (guest == null)
            return NotFoundResponse.CreateResponse();

        return Ok(guest);
    }

    [HttpPost("login")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GuestLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OnLoginAsync([FromBody] GuestLoginRequest request)
    {
        var guest = await _guestRepository.SelectAsync(request.Code);
        if (guest == null)
            return NotFoundResponse.CreateResponse();

        var token = _tokenService.CreateAccessToken(guest);
        await _guestAccessRepository.Insert(new GuestAccess { Guest = guest, CreatedAt = DateTime.Now });
        return Ok(new GuestLoginResponse { AccessToken = token });
    }

    [Authorize]
    [HttpGet("QrCode")]
    public async Task<IActionResult> GenerateQrCodes()
    {
        var guests = await _guestRepository.SelectAsync();
        Console.WriteLine($"Generating {guests.Count()} qrcodes");
        var qrCodes = guests.Select(x => new {
            FileName = $"qrcode-{x.Code}.png",
            Url = $"https://leguto.co/" + x.Code,
            Guest = x.Name
        }).ToList();

        string dirname = Path.Combine(Directory.GetCurrentDirectory(), "qrcodes");
        if (!Directory.Exists(dirname))
            Directory.CreateDirectory(dirname);

        var response = qrCodes.Select(x => {
            string filePath = $"{Path.Combine(dirname, x.FileName)}";
            GenerateQRCode(x.Url, filePath);

            return new {
                Url = x.Url,
                Guest = x.Guest,
                Path = filePath,
                Successfully = true,
            };
        });

        return Ok(response);
    }

    [Authorize]
    [HttpGet("pdf")]
    public async Task<IActionResult> GenerateInvites()
    {
        string qrCodeDirName = Path.Combine(Directory.GetCurrentDirectory(), "qrcodes");
        string inviteDirName = Path.Combine(Directory.GetCurrentDirectory(), "invites");
        var qrCodes = Directory.GetFiles(qrCodeDirName);
        if (!qrCodes?.Any() ?? false)
            return UnprocessableEntity("No qr code was found");
            
        if (!Directory.Exists(inviteDirName))
            Directory.CreateDirectory(inviteDirName);

        string baseInvitePath = Path.Combine(inviteDirName, "wedding-invite.pdf");
        if (!System.IO.File.Exists(baseInvitePath))
            return UnprocessableEntity("No base invite was found");

        string fontPath = "/home/augustohtpereira/.local/share/fonts/Raleway-Regular.ttf";

        var guests = await _guestRepository.SelectAsync();

        string file = "";
        guests.Where(x => x.Type == "guest").ToList().ForEach(guest => 
        {
            string code = guest.Code;
            string invitePath = Path.Combine(inviteDirName, $"invite-{code}.pdf");
            string qrCodePath = Path.Combine(qrCodeDirName, $"qrcode-{code}.png");
            file = invitePath;

            if (!System.IO.File.Exists(qrCodePath))
            {
                Console.WriteLine($"Guest {guest.Name} does not have qrcode");
            }
            else 
            {
                System.IO.File.Copy(baseInvitePath, invitePath, true);

                // Modify PDF located at "source" and save to "target"
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(baseInvitePath), new PdfWriter(invitePath));
                // Document to add layout elements: paragraphs, images etc
                Document document = new Document(pdfDocument);

                // Load image from disk
                ImageData imageData = ImageDataFactory.Create(qrCodePath);
                // Create layout image object and provide parameters. Page number = 1
                iText.Layout.Element.Image qrCode = new iText.Layout.Element.Image(imageData);
                qrCode.SetFixedPosition(194, 60);
                qrCode.SetWidth(35);
                qrCode.SetHeight(35);
                // This adds the image to the page
                document.Add(qrCode);

                PdfFont font = PdfFontFactory.CreateFont(fontPath, "UTF-8", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED, pdfDocument);
                var color = new iText.Kernel.Colors.DeviceRgb(75, 85, 104);

                iText.Layout.Element.Paragraph qrCodeLegend = new Paragraph("leguto.co/" + code);
                qrCodeLegend.SetFixedPosition(92, 50, 235);
                qrCodeLegend.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                qrCodeLegend.SetFontSize(7);
                qrCodeLegend.SetFont(font);
                qrCodeLegend.SetFontColor(color);
                document.Add(qrCodeLegend);

                iText.Layout.Element.Paragraph guestName = new Paragraph(RemoveSpecialChars(guest.Name));
                guestName.SetFixedPosition(92, 41, 235);
                guestName.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                guestName.SetFontSize(7);
                guestName.SetFont(font);
                guestName.SetFontColor(color);
                guestName.SetFontScript(iText.Commons.Utils.UnicodeScript.COPTIC);
                document.Add(guestName);

                Console.WriteLine($"Invite to {guest.Name} with code {guest.Code} was successfully generated.");

                // Don't forget to close the document.
                // When you use Document, you should close it rather than PdfDocument instance
                document.Close();
            }
        });

        string mergedFile = Path.Combine(inviteDirName, "all-invites.pdf");
        var pdf = new PdfDocument(new PdfWriter(mergedFile));
        PdfMerger merger = new PdfMerger(pdf);
        
        guests.Where(x => x.Type == "guest").ToList().ForEach(guest => {
            string invitePath = Path.Combine(inviteDirName, $"invite-{guest.Code}.pdf");
            var toMerge = new PdfDocument(new PdfReader(invitePath));
            merger.Merge(toMerge, 1, toMerge.GetNumberOfPages());
            toMerge.Close();
        });

        pdf.Close();

        Console.WriteLine($"The fully invite was saved in {mergedFile}.");

        return File(System.IO.File.ReadAllBytes(mergedFile), "application/pdf");
    }

    void GenerateQRCode(string url, string filePath)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(20);
        qrCodeImage.Save(filePath);
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