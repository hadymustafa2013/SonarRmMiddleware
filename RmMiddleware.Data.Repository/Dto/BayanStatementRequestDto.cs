using Data.Repository.POCO;

namespace Data.Repository.Dto;

public class BayanStatementRequestDto
{
    public BayanStatementRequest? BayanStatementRequest { get; set; }
    public List<BayanStatement> BayanStatements { get; set; } = [];
}