using buzzaraApi.DTOs;
using Microsoft.AspNetCore.Mvc;

public class UpdateServicoDTO
{
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string LugarEncontro { get; set; } = null!;
    public string Saidas { get; set; } = null!;
    [FromForm(Name = "servicoPrestado")]
    public required string ServicoPrestado { get; set; }
    [FromForm(Name = "servicoEspecial")]
    public required string ServicoEspecial { get; set; }
    public string? Disponibilidade { get; set; }

    public int? Idade { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Altura { get; set; }

    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Bairro { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public List<IFormFile>? NovasFotos { get; set; }
    public IFormFile? NovoVideo { get; set; }
    public SobreUsuarioDTO? SobreUsuario { get; set; }
    public List<ServicoCacheDTO>? Caches { get; set; }
    public bool MesmoHorarioTodosOsDias { get; set; }
    public HorarioAtendimentoDTO? HorarioUnico { get; set; }
    public List<HorarioAtendimentoDTO>? HorariosIndividuais { get; set; }

}
