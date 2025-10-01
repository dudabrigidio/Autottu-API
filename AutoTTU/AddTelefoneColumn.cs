using Oracle.ManagedDataAccess.Client;
using System;

namespace AutoTTU
{
    public class AddTelefoneColumn
    {
        public static void Main(string[] args)
        {
            string connectionString = "User Id=rm558575;Password=180604;Data Source=oracle.fiap.com.br:1521/orcl";
            
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    
                    // Verificar se a coluna já existe
                    string checkColumnQuery = @"
                        SELECT COUNT(*) 
                        FROM USER_TAB_COLUMNS 
                        WHERE TABLE_NAME = 'Usuario' AND COLUMN_NAME = 'Telefone'";
                    
                    using (var checkCommand = new OracleCommand(checkColumnQuery, connection))
                    {
                        int columnExists = Convert.ToInt32(checkCommand.ExecuteScalar());
                        
                        if (columnExists == 0)
                        {
                            // Adicionar a coluna
                            string addColumnQuery = @"
                                ALTER TABLE ""Usuario"" ADD ""Telefone"" NVARCHAR2(15) DEFAULT '' NOT NULL";
                            
                            using (var addCommand = new OracleCommand(addColumnQuery, connection))
                            {
                                addCommand.ExecuteNonQuery();
                                Console.WriteLine("Coluna Telefone adicionada com sucesso!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Coluna Telefone já existe na tabela Usuario.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }
}

