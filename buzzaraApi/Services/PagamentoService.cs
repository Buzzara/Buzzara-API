using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace buzzaraApi.Services
{
    public class PagamentoService
    {
        private readonly ApplicationDbContext _ctx;

        // 💵 Constantes de valores
        private const decimal VALOR_BASE_ANUNCIO = 30.0m;
        private const decimal VALOR_PACOTE_FOTOS = 10.0m;

        public PagamentoService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Finaliza o anúncio, calcula o valor a pagar e gera QR Code de pagamento.
        /// </summary>
        public async Task<PagamentoAnuncioDTO> FinalizarAnuncioAsync(int servicoId, int userId)
        {
            var servico = await _ctx.Servicos
                .Include(s => s.PerfilAcompanhante)
                .Include(s => s.Fotos)
                .FirstOrDefaultAsync(s => s.ServicoID == servicoId);

            if (servico == null)
                throw new KeyNotFoundException("Anúncio não encontrado.");

            if (servico.PerfilAcompanhante.UsuarioID != userId)
                throw new SecurityException("Não autorizado.");

            var fotosExtras = Math.Max(0, servico.Fotos.Count - 4);
            var pacotesExtras = (int)Math.Ceiling(fotosExtras / 4.0);

            decimal valorTotal = VALOR_BASE_ANUNCIO + (pacotesExtras * VALOR_PACOTE_FOTOS);

            // Gera o QR Code simulado
            var qrCodeUrl = GerarQrCodeFake(servicoId, valorTotal);

            var pagamento = new PagamentoAnuncio
            {
                ServicoID = servicoId,
                ValorTotal = valorTotal,
                Pago = false,
                QrCodeUrl = qrCodeUrl,
                DataCriacao = DateTime.UtcNow
            };

            _ctx.PagamentosAnuncios.Add(pagamento);
            await _ctx.SaveChangesAsync();

            return new PagamentoAnuncioDTO
            {
                PagamentoAnuncioID = pagamento.PagamentoAnuncioID,
                ServicoID = pagamento.ServicoID,
                ValorTotal = pagamento.ValorTotal,
                QrCodeUrl = pagamento.QrCodeUrl,
                Pago = pagamento.Pago,
                DataCriacao = pagamento.DataCriacao
            };
        }

        /// <summary>
        /// Confirma o pagamento manualmente (ex: após receber confirmação de pagamento).
        /// </summary>
        public async Task<bool> ConfirmarPagamentoAsync(int pagamentoId)
        {
            var pagamento = await _ctx.PagamentosAnuncios
                .FirstOrDefaultAsync(p => p.PagamentoAnuncioID == pagamentoId);

            if (pagamento == null)
                throw new KeyNotFoundException("Pagamento não encontrado.");

            if (pagamento.Pago)
                return false; // Já estava pago

            pagamento.Pago = true;
            await _ctx.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Busca detalhes do pagamento pelo ID.
        /// </summary>
        public async Task<PagamentoAnuncio?> BuscarPagamentoPorIdAsync(int pagamentoId)
        {
            return await _ctx.PagamentosAnuncios
                .FirstOrDefaultAsync(p => p.PagamentoAnuncioID == pagamentoId);
        }

        /// <summary>
        /// Simula geração de URL de QR Code (placeholder para integração real).
        /// </summary>
        private string GerarQrCodeFake(int servicoId, decimal valor)
        {
            return $"https://fake-qrcode.com/anuncio/{servicoId}/valor/{valor}";
        }
    }
}
