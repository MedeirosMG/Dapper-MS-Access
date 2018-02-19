using Dapper;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperUtility
{
    // Classe utilizada para simular um Entity framework, e inserir no banco de acordo com a classe recebida
    // Definição do objeto utilizado:
    // Atributo KeyAttribute: PK do objeto
    // Atributo AttributeTable: Nome da tabela que o objeto referencia
    // Atributo AttributeExcluir: Campos do objeto que não irão para o banco
    // Definir campos da classe igual da tabela : Ex: id_cliente(objeto) = id_cliente(tabela)

    public class DBUtils<T> : IDisposable
    {
        private OleDbConnection conexao = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = D:/Medei/Desktop/Teste.accdb");

        public void Dispose()
        {
            conexao.Close();
        }

        public string formatValue(Object value, Type tipo)
        {
            if (tipo == typeof(string))
            {
                if (value == null)
                    return "''";
                else
                    return "'" + value.ToString() + "'";
            }

            if (tipo == typeof(int))
                return value.ToString();

            return null;
        }

        //Da update no objeto e retorna true or false
        public bool Update(T obj)
        {
            //Abre conexão e define variaveis iniciais
            try
            {
                conexao.Open();
                Type tipo = typeof(T);
                string pk = "";
                string tableName = "";
                string query = "";
                string pkValue = "";
                string values = "";

                //Verifica quantos objetos a excluir existem para tirar isso de virgulas da query
                int objectCount = 0;
                foreach (PropertyInfo info in tipo.GetProperties())
                {
                    System.Attribute tipoAtributo = info.GetCustomAttributes().FirstOrDefault();

                    if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("AttributeExcluir"))
                        objectCount++;
                }


                for (int i = 0; i < tipo.GetProperties().Count(); i++)
                {
                    //Verifica se atributo é PK para usar na query
                    System.Attribute tipoAtributo = tipo.GetProperties()[i].GetCustomAttributes().FirstOrDefault();
                    if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("KeyAttribute"))
                    {
                        pk = tipo.GetProperties()[i].Name;
                        pkValue = formatValue(tipo.GetProperties()[i].GetValue(obj), tipo.GetProperties()[i].GetValue(obj).GetType());
                    }
                    else if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("AttributeTable"))
                        tableName = tipo.GetProperties()[i].Name;
                    else
                    {
                        //Verifica atributo excluir, o mesmo não é para inserir no banco
                        if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("AttributeExcluir"))
                            continue;

                        //Preenche valores a inserir
                        values += tipo.GetProperties()[i].Name + "=" +formatValue(tipo.GetProperties()[i].GetValue(obj), tipo.GetProperties()[i].GetValue(obj).GetType());

                        //Adiciona virgula entre os valores ( -2 pois existem dois atributos não utilizados ID e table name) + Objetos não inseridos na query
                        if (i != tipo.GetProperties().Count() - (2 + objectCount))
                        {
                            values += ", ";
                        }
                    }
                }

                //Escreve a query para execução
                query += "Update " + tableName + " SET " + values + " WHERE " + pk + "=" + pkValue;

                //Executa query
                if(conexao.Execute(query) != 0)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        //Insere tabela e retorna Id da linha adicionada
        public int Insert(T obj)
        {
            try
            {
                //Abre conexão e define variaveis iniciais
                conexao.Open();
                Type tipo = typeof(T);
                string pk = "";
                string tableName = "";
                string query = "";
                string fields = "";
                string values = "";

                //Verifica quantos objetos a excluir existem para tirar isso de virgulas da query
                int objectCount = 0;
                foreach (PropertyInfo info in tipo.GetProperties())
                {
                    System.Attribute tipoAtributo = info.GetCustomAttributes().FirstOrDefault();

                    if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("AttributeExcluir"))
                        objectCount++;
                }


                for (int i = 0; i < tipo.GetProperties().Count(); i++)
                {
                    //Verifica se atributo é PK e armazena para dar select depois de inserir
                    System.Attribute tipoAtributo = tipo.GetProperties()[i].GetCustomAttributes().FirstOrDefault();
                    if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("KeyAttribute"))
                        pk = tipo.GetProperties()[i].Name;
                    else if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("AttributeTable"))
                        tableName = tipo.GetProperties()[i].Name;
                    else
                    {
                        //Verifica atributo excluir, o mesmo não é para inserir no banco
                        if (tipoAtributo != null && tipoAtributo.GetType().ToString().Contains("AttributeExcluir"))
                            continue;

                        //Preenche campos da tabela
                        fields += tipo.GetProperties()[i].Name;

                        //Preenche valores a inserir                        
                        values += formatValue(tipo.GetProperties()[i].GetValue(obj), tipo.GetProperties()[i].PropertyType);

                        //Adiciona virgula entre os valores ( -2 pois existem dois atributos não utilizados ID e table name) + Objetos não inseridos na query
                        if (i != tipo.GetProperties().Count() - (2 + objectCount))
                        {
                            fields += ",";
                            values += ",";
                        }
                    }
                }

                //Escreve a query para execução
                query += "Insert into " + tableName + "(" + fields + ") values(" + values + ")";

                //Executa query
                conexao.Execute(query);

                //Seleciona ID da linha adicionada
                query = "Select MAX(" + pk + ") from " + tableName;
                return conexao.Query<int>(query).FirstOrDefault();
            }
            catch
            {
                return -1;
            }
        }
    }
}
