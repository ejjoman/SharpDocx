﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if !NET35 
using System.Threading.Tasks;
#endif
using Model.Models;
using Model.Repositories;
using SharpDocx;

namespace Model
{
    internal class Program
    {
        private static readonly string BasePath =
            Path.GetDirectoryName(typeof(Program).Assembly.Location) + @"/../../../../..";

        private static void Main()
        {
#if !DEBUG
            var viewPath = $"{BasePath}/Views/Model.cs.docx";
            var documentPath = $"{BasePath}/Documents/Model.docx";            
            var model = CreateViewModel();

            Ide.Start<MyViewModel>(viewPath, documentPath, model);
#else
            var startTime = DateTime.Now;
            var documentCount = 100;

            // Single threaded performance test.
            for (int i = 0; i < documentCount; ++i)
            {
                GenerateDocument(i);
            }

            // Multi threaded performance test.
            //Parallel.For(0, documentCount, GenerateDocument);

            var totalSeconds = (DateTime.Now - startTime).TotalSeconds;
            Console.WriteLine($"Generated {documentCount} documents in {totalSeconds:N2}s ({documentCount / totalSeconds:N2} documents/s).");
            Console.ReadKey();
#endif
        }

#if DEBUG
        private static void GenerateDocument(int i)
        {
            var viewPath = $"{BasePath}\\Views\\Model.cs.docx";
            var documentPath = $"{BasePath}\\Documents\\Model {i}.docx";
            var model = CreateViewModel();
            model.Title += i.ToString();

            File.Copy(viewPath, documentPath, true);

            using (var documentStream = File.Open(documentPath, FileMode.Open))
            {
                documentStream.Seek(0, SeekOrigin.Begin);

                var document = DocumentFactory.Create<MyViewModel>(documentStream);
                document.Generate(model);

                documentStream.Close();
            }
        }
#endif

        private static MyViewModel CreateViewModel()
        {
            //var countries = new List<Country>();
            //var countries = new List<Country> { new Country { Name = "Porosika" } };
            var countries = CountryRepository.GetCountries();

            return new MyViewModel
            {
                Title = "Model Sample",
                Date = DateTime.Now.ToShortDateString(),
                Countries = countries,
                AveragePopulation = countries.Count > 0
                    ? (int) countries.Average(c => c.Population)
                    : (int?) null,
                AverageDateProclaimed = GetAverageDateProclaimed(countries)
            };
        }

        private static DateTime? GetAverageDateProclaimed(List<Country> countries)
        {
            var dates = countries
                .Where(c => c.DateProclaimed.HasValue)
                .Select(c => c.DateProclaimed.Value)
                .ToList();

            if (dates.Count == 0)
            {
                return null;
            }

            return new DateTime((long) dates
                .Aggregate<DateTime, double>(0, (current, date) => current + date.Ticks / dates.Count));
        }
    }
}