using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgGenetico
{
    class Program
    {
        /// <summary>
        /// struct para conter os dados de cada indivíduo
        /// </summary>
        public struct stINDIVIDUO
        {
            public char[] cIndividuo;                           // cromossomo do indivíduo
            public Int64 nAvaliacao;                              // aptidão ou avaliação deste indivíduo
        }

        /// <summary>
        /// população de soluções do problema
        /// </summary>
        public static stINDIVIDUO[] populacao;                  // população ainda sem instanciar
        /// <summary>
        /// struct de trabalho para conter um individuo
        /// </summary>
        public static stINDIVIDUO stTrabalho;                   // struct para conter um indivíduo
        /// <summary>
        /// vetor de pesos 
        /// </summary>
        public static Int64[] pesos;                            // pesos para conversão binário/decimal

        /// <summary>
        /// dimensionamento do programa para resolver a seguinte equação: 3x + 2z - 2y = valor
        /// </summary>
        const int QTDE_INDIVIDUOS_POPULACAO = 100,              // qtde de indivíduos da população
            SEMENTE_RANDOM = 200,                               // semente para números randômicos
            QTDE_BITS_CADA_VARIAVEL = 32,                       // quantidade de bits para cada variável
            QTDE_VARIAVEIS = 3,                                 // x, y, z - três variáveis
            QTDE_BITS_CADA_INDIVIDUO =
                QTDE_BITS_CADA_VARIAVEL * QTDE_VARIAVEIS,       // qtde bits do cromossomo
            INDICE_X = 0,                                       // índice da variável x
            INDICE_Y = INDICE_X + QTDE_BITS_CADA_VARIAVEL,      // índice da vairável y
            INDICE_Z = INDICE_Y + QTDE_BITS_CADA_VARIAVEL,      // índice da variável z
            PERCENTUAL_ELITISMO = 20,                           // percentual de elitismo
            PERCENTUAL_MUTACAO = 3,                             // percentual de mutação
            QTDE_MUTACAO =
                (QTDE_BITS_CADA_INDIVIDUO * PERCENTUAL_MUTACAO)
                    / 100,                                      // quantidade de bits a serem mutados
            INDICE_PRIMEIRO_FILHO = PERCENTUAL_ELITISMO,        // indice do primeiro filho do cruzamento
            QTDE_CRUZAMENTOS =
                (QTDE_INDIVIDUOS_POPULACAO - PERCENTUAL_ELITISMO) / 2, // qtde de cruzamentos
            QTDE_INDIVIDUOS_RODEIO = 5,                         // quantidade de participantes do rodeio
            QTDE_MAXIMA_GERACOES = 3000;                        // qtde máxima de gerações

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">parâmetros da linha de comando</param>
        static void Main(string[] args)
        {
            bool flgHouveTroca = false;                         // true - indica que houve troca de posição
            int i, j, k, l, m, n, p;                            // índices genéricos
            Int64 x, y, z,                                      // variáveis da equação
                nValorEquacao = 0,                              // para calcular o valor da equação
                nResultado;                                     // resultado da equação a ser obtido
            int nIndice = QTDE_BITS_CADA_VARIAVEL - 2,          // índice do bit mais significativo do valor
                nPontoDeCorte = 0,                              // ponto de corte para o cruzamento
                nBitMutacao = 0,                                // índice do bit para mutação
                nPrimeiroPai = 0,                               // índice do primeiro pai para o cruzamento
                nSegundoPai = 0;                                // idem do segundo pai
            populacao = new 
                stINDIVIDUO[QTDE_INDIVIDUOS_POPULACAO];         // dimensiona a população
            // definir e calcular os pesos para a quantidade de bits especificado
            pesos = new Int64[nIndice + 1];                     // dimensiona o vetor de pesos
            // loop para o cálculo dos pesos
            for(i = 0; i < QTDE_BITS_CADA_VARIAVEL - 1; i++, nIndice--)
            {
                pesos[i] = Convert.ToInt64(Math.Pow(2, nIndice));
            }
            // pedir o valor da equação a ser resolvida
            do
            {
                try
                {
                    Console.Write("Valor da equação: 3x + 2z - 2y = ");
                    nResultado = Convert.ToInt64(Console.ReadLine());
                    break;                                      // cai fora do while
                }
                catch (Exception)
                {
                    Console.WriteLine("Redigite o resultado corretamente!");                    
                }
            } while (true);
            // vamos sortear os bits do cromossomo de cada indivíduo da população
            Random rndSorteio = new Random(SEMENTE_RANDOM);
            // loop para inicializar os cromossomos da população da geração inicial
            for(i = 0; i < QTDE_INDIVIDUOS_POPULACAO; i++)
            {
                // dimensionar o cromossomo de cada indivíduo da população inicial
                populacao[i].cIndividuo = new char[QTDE_BITS_CADA_INDIVIDUO];
                // loop para criar os bits '0' e '1' de cada cromossomo
                for(j = 0; j < QTDE_BITS_CADA_INDIVIDUO; j++)
                {
                    if (rndSorteio.Next() % 2 == 0)              // bit zero?
                        populacao[i].cIndividuo[j] = '0';
                    else
                        populacao[i].cIndividuo[j] = '1';
                } // for j
                populacao[i].nAvaliacao = 0;
            } // for i - população
            //
            // loop para todas as gerações possíveis
            for(j = 0; j < QTDE_MAXIMA_GERACOES; j ++)
            {
                // loop de avaliação de cada indivíduo da população
                for(i = 0; i < QTDE_INDIVIDUOS_POPULACAO; i++)
                {
                    // calcular x, y e z e aplicar na equação e obter a aptidão
                    x = CalcularVariavel(populacao[i].cIndividuo, INDICE_X);
                    y = CalcularVariavel(populacao[i].cIndividuo, INDICE_Y);
                    z = CalcularVariavel(populacao[i].cIndividuo, INDICE_Z);
                    // aplicar na equação
                    try
                    {
                        nValorEquacao = (3 * x) + (2 * z) - (2 * y);
                        populacao[i].nAvaliacao =
                            Math.Abs(nResultado - nValorEquacao);   // aptidão deste indivíduo
                    }
                    catch (Exception)
                    {
                        populacao[i].nAvaliacao = Int64.MaxValue;   // aptidão absurda
                    }
                    // verificar se a equação foi resolvida
                    if(populacao[i].nAvaliacao == 0)                // chegou ao resultado?
                    {
                        Console.WriteLine(
                            "Equação foi resolvida: x = {0} y = {1} z = {2}\nNa geração: {3}\nDigite qualquer tecla para encerrar o programa.",
                            x, y, z, (j + 1));
                        Console.ReadKey();                          // pausa
                        return;                                     // volta ao sistema operacional
                    }
                } // for i - população
                // a população foi avaliada e a solução não foi encontrada nesta geração
                // gerar uma nova população de soluções através dos seguintes operadores genéticos:
                // 1. Seleção de 2 pais através do seguinte:
                // 1.1 - Elitismo para isolar os melhores
                // 1.2 - Seleção através do rodeio de pais da elite
                // 2. Cruzamento entre dois pais selecionados da elite gerando 2 filhos
                // 3. Mutação nos dois filhos gerados pelo cruzamentos ou crossover

                // para fazer o elitismo precisamos classificar os indivíduos da população
                // em ordem crescente de avaliação.                
                do
                {
                    flgHouveTroca = false;                          // não houve troca ainda
                    for (i = 0; i < QTDE_INDIVIDUOS_POPULACAO - 1; i++)
                    {   // loop para comparação dois a dois indivíduos
                        if(populacao[i].nAvaliacao > populacao[i+1].nAvaliacao)     // fora de ordem?
                        {   // fazer a troca de posição entre eleme[i] e elem[i+1]
                            stTrabalho = populacao[i];              // salva o elem[i]
                            populacao[i] = populacao[i + 1];        // troca o elem[i+]
                            populacao[i + 1] = stTrabalho;          // troca o elem[i]
                            flgHouveTroca = true;
                        }
                    } // for i
                } while (flgHouveTroca);
            }// for j - loop de gerações
        } // main
        /// <summary>
        /// Método que converte a variável de binário para decimal
        /// </summary>
        /// <param name="cIndividuo">Cromossomo do indivíduo</param>
        /// <param name="iIndice">Índice da variável a ser convertida</param>
        /// <returns>valor inteiro de 64bits da variável desejada/returns>
        public static Int64 CalcularVariavel(char[] cIndividuo, int iIndice)
        {
            Int64 nValorDecimal = 0;                        // valor convertido
            int i = iIndice,                                // índice inicial da variável
                j;                                          // índice genérico do peso
            char cSinal = cIndividuo[i++];                  // para guardar o sinal da variável
            // loop para a conversão
            for (j = 0; j < QTDE_BITS_CADA_VARIAVEL - 1; j++, i++)  // j = índice do peso
            {
                if (cIndividuo[i] == '1')                   // bit um?
                    nValorDecimal += pesos[j];              // somatória para conversão
            } // for j
            if(cSinal == '1')                               // negativo?
            return nValorDecimal * -1;                      // devolve negativo
            return nValorDecimal;                           // devolve positivo
        }
    } // class
}
