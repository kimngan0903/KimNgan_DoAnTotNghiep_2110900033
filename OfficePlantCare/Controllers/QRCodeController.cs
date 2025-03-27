using Microsoft.AspNetCore.Mvc;

[Route("QrCode")]
public class QrCodeController : Controller
{
    [HttpGet("GenerateQr")]
    public IActionResult GenerateQr(string amount)
    {
        if (string.IsNullOrEmpty(amount))
        {
            return BadRequest("Số tiền không hợp lệ!");
        }

        string bankAccount = "9373993662";
        string bankName = "VCB";
        string content = "Thanh toan dich vu";

        string vietQrUrl = $"https://img.vietqr.io/image/{bankName}-{bankAccount}-qr_only.png?amount={amount}&addInfo={content}";

        return Redirect(vietQrUrl);
    }
}