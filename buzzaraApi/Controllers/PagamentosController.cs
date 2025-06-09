using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("pagamentos")]
    [Authorize(Roles = "Acompanhante,Admin")]
    public class PagamentosController : ControllerBase
    {
        private readonly PagamentoService _pagamentoService;
        private readonly QRCodeService _qrCodeService;

        // 🔥 Apenas 1 construtor
        public PagamentosController(PagamentoService pagamentoService, QRCodeService qrCodeService)
        {
            _pagamentoService = pagamentoService;
            _qrCodeService = qrCodeService;
        }

        private int GetUserId()
        {
            var rawId = User.FindFirstValue("nameid");
            if (string.IsNullOrEmpty(rawId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return int.Parse(rawId);
        }

        // POST /pagamentos/finalizar/{servicoId}
        [HttpPost("finalizar/{servicoId:int}")]
        public async Task<IActionResult> FinalizarAnuncio(int servicoId)
        {
            try
            {
                var userId = GetUserId();
                var pagamento = await _pagamentoService.FinalizarAnuncioAsync(servicoId, userId);
                return Ok(pagamento);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /pagamentos/qrcode/{pagamentoId}
        [HttpGet("qrcode/{pagamentoId:int}")]
        public async Task<IActionResult> GerarQrCodeVisual(int pagamentoId)
        {
            var pagamento = await _pagamentoService.BuscarPagamentoPorIdAsync(pagamentoId);

            if (pagamento == null)
                return NotFound(new { error = "Pagamento não encontrado." });

            var svg = _qrCodeService.GerarQRCodeSvg(pagamento.QrCodeUrl);

            return Content(svg, "image/svg+xml");
        }

        // POST /pagamentos/confirmar/{pagamentoId}
        [HttpPost("confirmar/{pagamentoId:int}")]
        public async Task<IActionResult> ConfirmarPagamento(int pagamentoId)
        {
            try
            {
                var sucesso = await _pagamentoService.ConfirmarPagamentoAsync(pagamentoId);
                return Ok(new { sucesso });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
