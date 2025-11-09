// Plan w pseudokodzie (szczegó³owo, po polsku):
// 1. Zdefiniowaæ statyczn¹ klasê rozszerzeñ `ServiceCollectionExtensions`.
// 2. Dodaæ statyczn¹ metodê rozszerzaj¹c¹ `AddModulesMediator(this IServiceCollection services)`,
//    która zwraca `IServiceCollection` (umo¿liwia chainowanie).
// 3. W metodzie zebraæ listê za³adowanych assembly:
//    a) Pobierz aktualne assembly z AppDomain (pomijaj¹c dynamiczne i bez lokacji).
//    b) Spróbuj odczytaæ `DependencyContext.Default` i dodaæ assembly, których nazwy zaczynaj¹
//       siê od `Flashcard`, `FlashCard` lub `Modules` (bezpieczniejsze Ÿród³o).
//    c) Fallback: przeszukaj katalog aplikacji (`AppContext.BaseDirectory`) i za³aduj DLL-e
//       pasuj¹ce do wzorca nazw (tylko te, których jeszcze nie ma na liœcie).
// 4. Wywo³aj istniej¹c¹ metodê `RegisterMediator` przekazuj¹c `assemblies.Distinct().ToArray()`.
// 5. Zwróæ `services`.
// 6. Dziêki temu w `Program.cs` wystarczy jedna linijka: `builder.Services.AddModulesMediator();`
//
// Implementacja poni¿ej.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyInjection;
using FlashCard.Shared.CQRS.Application.Logic;

namespace FlashcardApp.Botstraper.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Skanuje assembly aplikacji oraz runtime libraries zaczynaj¹ce siê od
        /// "Flashcard", "FlashCard" lub "Modules" i rejestruje mediator
        /// wywo³uj¹c istniej¹c¹ metodê `RegisterMediator`.
        /// Umo¿liwia zast¹pienie rozbudowanego kodu w Program.cs jedn¹ linijk¹.
        /// </summary>
        public static IServiceCollection AddModulesMediator(this IServiceCollection services)
        {
            // Pobierz ju¿ za³adowane assembly
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToList();

            // 1) spróbuj za³adowaæ z DependencyContext (bezpieczniejsze)
            if (DependencyContext.Default != null)
            {
                var libs = DependencyContext.Default.RuntimeLibraries
                    .Where(lib => lib.Name.StartsWith("Flashcard", StringComparison.OrdinalIgnoreCase)
                               || lib.Name.StartsWith("FlashCard", StringComparison.OrdinalIgnoreCase)
                               || lib.Name.StartsWith("Modules", StringComparison.OrdinalIgnoreCase));

                foreach (var lib in libs)
                {
                    foreach (var asmName in lib.GetDefaultAssemblyNames(DependencyContext.Default))
                    {
                        if (assemblies.Any(a => a.GetName().Name == asmName.Name)) continue;
                        try
                        {
                            assemblies.Add(Assembly.Load(asmName));
                        }
                        catch
                        {
                            // ignoruj nieudane ³adowanie poszczególnych assembly
                        }
                    }
                }
            }

            // 2) fallback: za³aduj DLL-e z katalogu aplikacji (tylko pasuj¹ce do wzorca)
            var baseDir = AppContext.BaseDirectory;
            foreach (var dll in Directory.EnumerateFiles(baseDir, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(dll);
                if (assemblies.Any(a => a.GetName().Name == name)) continue;
                if (!(name.StartsWith("Flashcard", StringComparison.OrdinalIgnoreCase)
                   || name.StartsWith("FlashCard", StringComparison.OrdinalIgnoreCase)
                   || name.StartsWith("Modules", StringComparison.OrdinalIgnoreCase))) continue;

                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dll));
                }
                catch
                {
                    // ignoruj b³êdy ³adowania
                }
            }

            // Zarejestruj mediator z zebran¹ list¹ assembly
            services.RegisterMediator(assemblies.Distinct().ToArray());

            return services;
        }
    }
}