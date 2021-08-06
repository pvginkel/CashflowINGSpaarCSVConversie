using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashflowINGSpaarCSVConversie
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string sourceFileName = args[0];
            string targetFileName = sourceFileName;
            if (args.Length > 1)
                targetFileName = args[1];

            var source = ReadCsv(sourceFileName);

            var target = new List<List<string>>
            {
                new List<string>
                {
                    "Datum",
                    "Naam / Omschrijving",
                    "Rekening",
                    "Tegenrekening",
                    "Code",
                    "Af Bij",
                    "Bedrag (EUR)",
                    "Mutatiesoort",
                    "Mededelingen"
                }
            };


            foreach (var line in source.Skip(1))
            {
                target.Add(new List<string>
                {
                    line[0].Replace("-", ""), // Datum -> Datum
                    line[1], // Omschrijving -> Naam / Omschrijving
                    line[2], // Rekening -> Rekening
                    line[4], // Tegenrekening -> Tegenrekening
                    "", // _ -> Code
                    line[5], // Ad Bij -> Af Bij
                    line[6], // Bedrag -> Bedrag (EUR)
                    line[8], // Mutatiesoort -> Mutatiesoort
                    line[9] // Mededeling -> Mededeling
                });
            }

            WriteCsv(targetFileName, target);
        }

        private static List<List<string>> ReadCsv(string fileName)
        {
            return File.ReadAllLines(fileName)
                .Select(p => ParseCsv(p).ToList())
                .ToList();
        }

        private static IEnumerable<string> ParseCsv(string line)
        {
            bool inQuotes = false;
            char lastChar = (char)0;
            var sb = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    if (inQuotes)
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        inQuotes = true;
                        if (lastChar == '"')
                            sb.Append('"');
                    }
                }
                else if (c == ';' && !inQuotes)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }

                lastChar = c;
            }

            if (inQuotes)
                throw new InvalidOperationException("Start quote without end quote");

            yield return sb.ToString();
        }

        private static void WriteCsv(string fileName, List<List<string>> lines)
        {
            using (var writer = File.CreateText(fileName))
            {
                foreach (var line in lines)
                {
                    bool hadOne = false;

                    foreach (string part in line)
                    {
                        if (hadOne)
                            writer.Write(',');
                        else
                            hadOne = true;

                        writer.Write('"');
                        writer.Write(part.Replace("\"", "\"\""));
                        writer.Write('"');
                    }

                    writer.WriteLine();
                }
            }
        }
    }
}
