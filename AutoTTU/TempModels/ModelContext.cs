using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoTTU.TempModels;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("User Id=rm558575;Password=180604;Data Source=oracle.fiap.com.br:1521/orcl");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("RM558575")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasPrecision(10);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Senha).HasMaxLength(100);
        });
        modelBuilder.HasSequence("AGENDA_SEQ");
        modelBuilder.HasSequence("CONSERTO_SEQ");
        modelBuilder.HasSequence("DIAGNOSTICO_SEQ");
        modelBuilder.HasSequence("PESSOA_SEQ");
        modelBuilder.HasSequence("PROBLEMA_SEQ");
        modelBuilder.HasSequence("SEQ_AGUA");
        modelBuilder.HasSequence("SEQ_ENERGIA");
        modelBuilder.HasSequence("SEQ_GRAU");
        modelBuilder.HasSequence("SEQ_PESSOA");
        modelBuilder.HasSequence("SEQ_T_RHSTU_BAIRRO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_CIDADE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_CONSULTA");
        modelBuilder.HasSequence("SEQ_T_RHSTU_CONSULTA_FORMA_PAGTO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_CONTATO_PACIENTE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_EMAIL_PACIENTE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_ENDERECO_PACIENTE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_ESTADO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_FORMA_PAGAMENTO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_LOGRADOURO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_MEDICAMENTO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_MEDICO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_PACIENTE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_PACIENTE_PLANO_SAUDE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_PLANO_SAUDE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_PRESCISAO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_TELEFONE_PACIENTE");
        modelBuilder.HasSequence("SEQ_T_RHSTU_TIPO_CONTATO");
        modelBuilder.HasSequence("SEQ_T_RHSTU_UNID_HOSPITALAR");
        modelBuilder.HasSequence("SEQ_TAREFAS");
        modelBuilder.HasSequence("SEQ_USUARIO");
        modelBuilder.HasSequence("SEQ_VEICULO");
        modelBuilder.HasSequence("SQ_RHSTU_BAIRRO");
        modelBuilder.HasSequence("SQ_RHSTU_CIDADE");
        modelBuilder.HasSequence("SQ_RHSTU_CONSULTA_FP");
        modelBuilder.HasSequence("SQ_RHSTU_CONTATOP");
        modelBuilder.HasSequence("SQ_RHSTU_EMAIL_PACIENTE");
        modelBuilder.HasSequence("SQ_RHSTU_ENDERECO");
        modelBuilder.HasSequence("SQ_RHSTU_ESTADO");
        modelBuilder.HasSequence("SQ_RHSTU_LOGRADOURO");
        modelBuilder.HasSequence("SQ_RHSTU_MEDICAMENTO");
        modelBuilder.HasSequence("SQ_RHSTU_MEDICO");
        modelBuilder.HasSequence("SQ_RHSTU_PACIENTE");
        modelBuilder.HasSequence("SQ_RHSTU_PACIENTE_PS");
        modelBuilder.HasSequence("SQ_RHSTU_PM");
        modelBuilder.HasSequence("SQ_RHSTU_TELEFONE_PACIENTE");
        modelBuilder.HasSequence("SQ_RHSTU_TIPO_CONTATO");
        modelBuilder.HasSequence("SQ_RHSTU_UNIDADE");
        modelBuilder.HasSequence("T_MECHIA_AGENDA_SEQ");
        modelBuilder.HasSequence("T_MECHIA_CLIENTE_ID_CLIENTE");
        modelBuilder.HasSequence("T_MECHIA_CONSERTO_SEQ");
        modelBuilder.HasSequence("T_MECHIA_DIAGNOSTICO_SEQ");
        modelBuilder.HasSequence("T_MECHIA_PESSOA_SEQ");
        modelBuilder.HasSequence("T_MECHIA_PROBLEMA_SEQ");
        modelBuilder.HasSequence("T_MECHIA_VEICULO_SEQ");
        modelBuilder.HasSequence("T_RHSTU_BAIRRO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_CIDADE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_CONSULTA_FORMA_PAGTO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_CONSULTA_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_CONTATO_PACIENTE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_EMAIL_PACIENTE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_ENDERECO_PACIENTE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_ESTADO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_FORMA_PAGAMENTO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_MEDICAMENTO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_MEDICO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_PACIENTE_PLANO_SAUDE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_PACIENTE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_PRESCISAO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_TELEFONE_PACIENTE_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_TIPO_CONTATO_SEQUENCE");
        modelBuilder.HasSequence("T_RHSTU_UNID_HOSPITALAR_SEQUENCE");
        modelBuilder.HasSequence("T_SLV_AUTOR_SEQUENCE");
        modelBuilder.HasSequence("T_SLV_CATEGORIA_SEQUENCE");
        modelBuilder.HasSequence("T_SLV_LIVRO_SEQUENCE");
        modelBuilder.HasSequence("VEICULO_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
