using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TP01__AED___Ordenações
{
    //PROGRAMA PRINCIPAL
    class Program
    {
        private static string[] sort = new string[] { "bolha", "seleção", "inserção", "quicksort", "mergesort", "mixedsort" };

        static void Main(string[] args)
        {
            Console.WriteLine("Ordenação - Análise de algoritmos\n");

            Console.Write("Qual caso? Melhor, médio ou pior? (b/m/p): ");
            char Caso = char.Parse(Console.ReadLine());

            Dados airbnb = new Dados();
            List<Dados> listaDados = airbnb.LerDadosArquivo();
            List<Resultado> resultados = new List<Resultado>();

            if (Caso == 'b' || Caso == 'p')
            {
                listaDados = listaDados.OrderBy(o => o.roomID).ToList();
                if (Caso == 'p')
                    listaDados.Reverse();
            }

            for (int i = 2000; i <= 128000; i *= 2)
            {
                for (int j = 1; j <= 5; j++)
                {
                    resultados.Add(new Resultado() { medição = j, amostra = i });
                    foreach (string tipo in sort)
                    {
                        long tempo = airbnb.Ordena(listaDados, i, tipo);
                        resultados[resultados.Count - 1].GetType().GetProperty(tipo).SetValue(resultados[resultados.Count - 1], tempo);

                        Console.WriteLine("Medição: {0}", j);
                        Console.WriteLine("Amostra: {0}", i);
                        Console.WriteLine("Tipo...: {0}", tipo);
                        if ((tempo / 1000.0) < 60)
                        {
                            Console.WriteLine("Tempo..: {0} segundos", tempo / 1000.0);
                        }
                        else
                        {
                            Console.WriteLine("Tempo..: {0} minutos", tempo / 60000.0);
                        }
                    }
                }
            }

            airbnb.EscreverResultados(resultados);
            Console.WriteLine("Feito! Os resultados foram gravados no arquivo tipo csv 'analise'.");
            Console.ReadKey();
        }
    }

    //CLASSE DADOS - FAZ LEITURA E ESCRITA EM ARQUIVOS, MEDIDA DE TEMPO E INSTÂNCIAS DOS DADOS DO AIRBNB
    class Dados
    {
        public int roomID { get; set; }
        public int hostID { get; set; }
        public string roomType { get; set; }
        public string contry { get; set; }
        public string city { get; set; }
        public string neighborhood { get; set; }
        public int reviews { get; set; }
        public float overallSatisfaction { get; set; }
        public int accommodates { get; set; }
        public float bedrooms { get; set; }
        public double price { get; set; }
        public string propertyType { get; set; }

        public List<Dados> LerDadosArquivo()
        {
            List<Dados> dados = new List<Dados>();
            StreamReader reader = new StreamReader(@"dados_airbnb.txt");
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string[] values = reader.ReadLine().Split('\t');

                Dados linha = new Dados();
                linha.roomID = int.Parse(values[0]);
                linha.hostID = int.Parse(values[1]);
                linha.roomType = values[2];
                linha.contry = values[3];
                linha.city = values[4];
                linha.neighborhood = values[5];
                linha.reviews = int.Parse(values[6]);
                linha.overallSatisfaction = float.Parse(values[7]);
                linha.accommodates = int.Parse(values[8]);
                linha.bedrooms = float.Parse(values[9]);
                linha.price = double.Parse(values[10]);
                linha.propertyType = values[11];

                dados.Add(linha);
            }

            reader.Close();
            return dados;
        }

        public void EscreverResultados(List<Resultado> lista)
        {
            StreamWriter writer = new StreamWriter(@"analise.csv");
            writer.WriteLine("medição, amostra, bolha, seleção, inserção, quicksort, mergesort, mixedsort");

            foreach (Resultado result in lista)
            {
                writer.WriteLine(
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                    result.medição,
                    result.amostra,
                    result.bolha,
                    result.seleção,
                    result.inserção,
                    result.quicksort,
                    result.mergesort,
                    result.mixedsort
                );
            }

            writer.Close();
        }

        public long Ordena(List<Dados> lista, int tam, string sort)
        {
            lista = lista.Take(tam).ToList();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            switch (sort)
            {
                case "bolha":
                    stopwatch.Start();
                    Ordenações.Bolha(lista);
                    stopwatch.Stop();
                    break;
                case "seleção":
                    stopwatch.Start();
                    Ordenações.Seleção(lista);
                    stopwatch.Stop();
                    break;
                case "inserção":
                    stopwatch.Start();
                    Ordenações.Inserção(lista);
                    stopwatch.Stop();
                    break;
                case "quicksort":
                    stopwatch.Start();
                    Ordenações.Quicksort(lista, 0, lista.Count - 1);
                    stopwatch.Stop();
                    break;
                case "mergesort":
                    stopwatch.Start();
                    Ordenações.Mergesort(lista, 0, lista.Count - 1);
                    stopwatch.Stop();
                    break;
                case "mixedsort":
                    stopwatch.Start();
                    Ordenações.Mixedsort(lista);
                    stopwatch.Stop();
                    break;
            }
            return stopwatch.ElapsedMilliseconds;
        }
    }

    //CLASSE ORDENAÇÕES - ONDE ESTÃO IMPLEMENTADAS TODAS AS FUNÇÕES E MÉTODOS DE ORDENAÇÃO
    static class Ordenações
    {
        public static void Bolha(List<Dados> lista)
        {
            for (int i = 0; i < lista.Count - 1; i++)
                for (int j = 0; j < lista.Count - i - 1; j++)
                    if (lista[j].roomID > lista[j + 1].roomID)
                    {
                        Dados aux = lista[j];
                        lista[j] = lista[j + 1];
                        lista[j + 1] = aux;
                    }
        }

        public static void Seleção(List<Dados> lista)
        {
            for (int i = 0; i < lista.Count - 1; i++)
            {
                int min = i;

                for (int j = i + 1; j < lista.Count; j++)
                    if (lista[j].roomID < lista[min].roomID)
                        min = j;

                Dados aux = lista[i];
                lista[i] = lista[min];
                lista[min] = aux;
            }
        }

        public static void Inserção(List<Dados> lista)
        {
            for (int i = 1; i < lista.Count; i++)
            {
                int j = i;
                while (j > 0 && lista[j].roomID < lista[j - 1].roomID)
                {
                    Dados aux = lista[j];
                    lista[j] = lista[j - 1];
                    lista[j - 1] = aux;
                    j--;
                }
            }
        }

        public static void Quicksort(List<Dados> lista, int início, int fim)
        {
            int i = início,
                j = fim,
                pivô = lista[(início + fim) / 2].roomID;

            while (i <= j)
            {
                while (lista[i].roomID < pivô && i < fim) i++;
                while (lista[j].roomID > pivô && j > início) j--;

                if (i <= j)
                {
                    Dados aux = lista[i];
                    lista[i] = lista[j];
                    lista[j] = aux;
                    i++;
                    j--;
                }
            }

            if (j > início)
                Quicksort(lista, início, j);

            if (i < fim)
                Quicksort(lista, i, fim);
        }

        public static void Mergesort(List<Dados> lista, int ínício, int fim)
        {
            if (ínício < fim)
            {
                int middle = (ínício + fim) / 2;
                Mergesort(lista, ínício, middle);
                Mergesort(lista, middle + 1, fim);
                merge(lista, ínício, middle, fim);
            }
        }

        private static void merge(List<Dados> lista, int início, int meio, int fim)
        {
            int n1 = meio - início + 1;
            int n2 = fim - meio;

            List<Dados> lista1 = new List<Dados>();
            List<Dados> lista2 = new List<Dados>();

            int i, j;
            for (i = 0; i < n1; i++)
                lista1.Add(lista[início + i]);
            for (j = 0; j < n2; j++)
                lista2.Add(lista[meio + j + 1]);

            lista1.Add(new Dados() { roomID = Int32.MaxValue });
            lista2.Add(new Dados() { roomID = Int32.MaxValue });

            i = j = 0;

            for (int k = início; k <= fim; k++)
                if (lista1[i].roomID <= lista2[j].roomID)
                    lista[k] = lista1[i++];
                else
                    lista[k] = lista2[j++];
        }

        public static void Mixedsort(List<Dados> lista)
        {
            int met = lista.Count / 2 + lista.Count % 2;
            List<Dados> lista1 = lista.GetRange(0, met);
            List<Dados> lista2 = lista.GetRange(met, lista.Count / 2);

            for (int i = 0; i < lista1.Count - 1; i++)
                for (int j = 0; j < lista1.Count - i - 1; j++)
                    if (lista1[j].roomID > lista1[j + 1].roomID)
                    {
                        Dados aux = lista1[j];
                        lista1[j] = lista1[j + 1];
                        lista1[j + 1] = aux;
                    }

            for (int i = 0; i < lista2.Count - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < lista2.Count; j++)
                    if (lista2[j].roomID < lista2[min].roomID)
                        min = j;

                Dados aux = lista2[i];
                lista2[i] = lista2[min];
                lista2[min] = aux;
            }

            lista1.Add(new Dados() { roomID = Int32.MaxValue });
            lista2.Add(new Dados() { roomID = Int32.MaxValue });

            int i1 = 0, i2 = 0;
            for (int k = 0; k <= lista.Count - 1; k++)
                if (lista1[i1].roomID <= lista2[i2].roomID)
                    lista[k] = lista1[i1++];
                else
                    lista[k] = lista2[i2++];
        }
    }

    //CLASSE RESULTADO - CRIADA COM O PROPÓSITO DE INSTANCIAR OS RESULTADOS QUE VÃO SER GRAVADOS EM UM ARQUIVO NO FINAL DA EXECUÇÃO DO PROGRAMA (VER CLASSE DADOS)
    class Resultado
    {
        public int medição { get; set; }
        public int amostra { get; set; }
        public long bolha { get; set; }
        public long seleção { get; set; }
        public long inserção { get; set; }
        public long quicksort { get; set; }
        public long mergesort { get; set; }
        public long mixedsort { get; set; }
    }
}
