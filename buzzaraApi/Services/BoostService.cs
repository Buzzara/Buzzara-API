using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Services
{
    public class BoostService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly PreferenceService _prefSvc;

        public BoostService(
            ApplicationDbContext ctx,
            PreferenceService prefSvc)
        {
            _ctx = ctx;
            _prefSvc = prefSvc;
        }

        public async Task<List<BoostPlan>> ListarPlanosAsync()
            => await _ctx.BoostPlans
                         .OrderBy(p => p.Price)
                         .ToListAsync();

        public async Task<BoostOrderDto> CriarPedidoAsync(
            CreateBoostOrderDto dto,
            int userId,
            string userEmail)
        {
            // 1) valida dono do anúncio
            var servico = await _ctx.Servicos
                .Include(s => s.PerfilAcompanhante)
                .FirstOrDefaultAsync(s => s.ServicoID == dto.ServicoId)
                ?? throw new KeyNotFoundException("Serviço não encontrado.");
            if (servico.PerfilAcompanhante.UsuarioID != userId)
                throw new UnauthorizedAccessException();

            // 2) busca plano
            var plan = await _ctx.BoostPlans.FindAsync(dto.PlanId)
                ?? throw new KeyNotFoundException("Plano inválido.");

            // 3) datas
            var start = dto.StartDate.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(plan.DaysDuration - 1);

            // 4) monta preferência MP
            var prefDto = new CreatePreferenceDto
            {
                Title = $"Boost anúncio #{dto.ServicoId} • {plan.Name}",
                Quantity = 1,
                UnitPrice = plan.Price,
                PayerEmail = userEmail
            };
            var pref = await _prefSvc.CreateAsync(prefDto);

            // 5) salva BoostOrder
            var order = new BoostOrder
            {
                ServicoId = dto.ServicoId,
                UserId = userId,
                PlanId = dto.PlanId,
                StartDate = start,
                EndDate = end,
                FirstTime = dto.FirstTime.ToTimeSpan(),
                LastTime = dto.LastTime.ToTimeSpan(),
                TotalPrice = plan.Price,
                PreferenceId = pref.Id,
                InitPoint = pref.InitPoint!,
                SandboxInitPoint = pref.SandboxInitPoint!
            };
            _ctx.BoostOrders.Add(order);
            await _ctx.SaveChangesAsync();

            // 6) retorna DTO
            return new BoostOrderDto
            {
                OrderId = order.OrderId,
                TotalPrice = order.TotalPrice,
                InitPoint = order.InitPoint,
                SandboxInitPoint = order.SandboxInitPoint,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
