-- Script para adicionar a coluna Telefone na tabela Usuario
ALTER TABLE "Usuario" ADD "Telefone" NVARCHAR2(15) DEFAULT '' NOT NULL;

