using AutoTTU.Models;

namespace AutoTTU.Tests.Helpers;

public class TestDataBuilder
{
    public static Checkin CreateCheckin(
        int idCheckin = 1,
        int idMoto = 1,
        int idUsuario = 1,
        string ativoChar = "S",
        string observacao = "Observação de teste",
        DateTime? timeStamp = null,
        string imagensUrl = "https://example.com/image.jpg")
    {
        return new Checkin
        {
            IdCheckin = idCheckin,
            IdMoto = idMoto,
            IdUsuario = idUsuario,
            AtivoChar = ativoChar,
            Observacao = observacao,
            TimeStamp = timeStamp ?? DateTime.Now,
            ImagensUrl = imagensUrl
        };
    }

    public static Motos CreateMoto(
        int idMoto = 1,
        string modelo = "CG 160",
        string marca = "Honda",
        int ano = 2023,
        string placa = "ABC1234",
        string ativoChar = "S",
        string fotoUrl = "https://example.com/foto.jpg")
    {
        return new Motos
        {
            IdMoto = idMoto,
            Modelo = modelo,
            Marca = marca,
            Ano = ano,
            Placa = placa,
            AtivoChar = ativoChar,
            FotoUrl = fotoUrl
        };
    }

    public static Usuario CreateUsuario(
        int idUsuario = 1,
        string nome = "João Silva",
        string email = "joao@test.com",
        string senha = "123456",
        string telefone = "11999999999")
    {
        return new Usuario
        {
            IdUsuario = idUsuario,
            Nome = nome,
            Email = email,
            Senha = senha,
            Telefone = telefone
        };
    }

    public static Slot CreateSlot(
        int idSlot = 1,
        int idMoto = 1,
        string ativoChar = "S")
    {
        return new Slot
        {
            IdSlot = idSlot,
            IdMoto = idMoto,
            AtivoChar = ativoChar
        };
    }
}
