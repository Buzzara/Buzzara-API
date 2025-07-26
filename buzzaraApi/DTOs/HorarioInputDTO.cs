using buzzaraApi.DTOs;

public class HorarioInputDTO
{
    public bool MesmoHorarioTodosOsDias { get; set; }
    public HorarioAtendimentoDTO? HorarioUnico { get; set; }
    public List<HorarioAtendimentoDTO>? HorariosIndividuais { get; set; }
}
