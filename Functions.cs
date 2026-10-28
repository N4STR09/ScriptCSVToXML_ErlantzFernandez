using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Practica2_2;

public class Functions {
    static string rutaCSV = "";
    static string rutaXML = "";

    public static void MenuCSVaXML() {
        if (!File.Exists(rutaCSV)) {
            Console.WriteLine("Selecciona primero un CSV válido.");
            return;
        }

        Console.WriteLine("\n===== CSV A XML =====");
        Console.WriteLine("1. Exportar todo el documento");
        Console.WriteLine("2. Exportar por género");
        Console.WriteLine("3. Exportar por año");
        Console.WriteLine("4. Exportar por idioma");
        Console.Write("Opción: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion)) {
            return;
        }

        List<Pelicula> peliculas = LeerCSV(rutaCSV);

        switch (opcion) {
            case 1:
                ExportarXML(peliculas);
                break;

            case 2:
                ExportarPorGenero(peliculas);
                break;

            case 3:
                ExportarPorAnio(peliculas);
                break;

            case 4:
                ExportarPorIdioma(peliculas);
                break;

            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }

    static List<Pelicula> LeerCSV(string ruta) {
        List<Pelicula> peliculas = new List<Pelicula>();

        using (TextFieldParser parser = new TextFieldParser(ruta)) {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TrimWhiteSpace = false;

            if (parser.EndOfData) {
                return peliculas;
            }

            string[] cabeceras = parser.ReadFields() ?? Array.Empty<string>();

            int Columna(string nombre) {
                for (int i = 0; i < cabeceras.Length; i++)
                {
                    if (cabeceras[i].Trim().Equals(
                        nombre, StringComparison.OrdinalIgnoreCase))
                        return i;
                }

                throw new Exception("No se encuentra la columna: " + nombre);
            }

            int title = Columna("Title");
            int genre = Columna("Genre");
            int premiere = Columna("Premiere");
            int runtime = Columna("Runtime");
            int score = Columna("IMDB Score");
            int language = Columna("Language");

            while (!parser.EndOfData) {
                string[] campos = parser.ReadFields() ?? Array.Empty<string>();

                int Maximo(params int[] indices) {
                    return indices.Max();
                }

                if (campos.Length <= Maximo(title, genre, premiere, runtime, score, language)) {
                    continue;
                }

                peliculas.Add(new Pelicula(campos[title], campos[genre], campos[premiere], campos[runtime], campos[score], campos[language]));
            }
        }

        return peliculas;
    }

    static void ExportarXML(List<Pelicula> peliculas) {
        if (string.IsNullOrWhiteSpace(rutaXML)) {
            Console.WriteLine("Selecciona primero una ruta XML.");
            return;
        }

        XDocument documento = CrearXML(peliculas);
        documento.Save(rutaXML);

        Console.WriteLine("XML exportado correctamente.");
    }

    static XDocument CrearXML(List<Pelicula> peliculas) {
        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Peliculas",
                peliculas.Select(p => new XElement("Pelicula",
                    new XElement("Title", p.Title),
                    new XElement("Genre", p.Genre),
                    new XElement("Premiere", p.Premiere),
                    new XElement("Runtime", p.Runtime),
                    new XElement("IMDBScore", p.IMDBScore),
                    new XElement("Language", p.Language)
                ))
            )
        );
    }

    static void ExportarPorGenero(List<Pelicula> peliculas) {
        List<string> generos = peliculas
            .SelectMany(p => p.Genre.Split(
                new[] { ",", ";" },
                StringSplitOptions.RemoveEmptyEntries))
            .Select(g => g.Trim())
            .Where(g => g != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g)
            .ToList();

        Console.WriteLine("\nGéneros disponibles:");

        for (int i = 0; i < generos.Count; i++) {
            Console.WriteLine($"{i + 1}. {generos[i]}");
        }

        Console.Write("Selecciona un género: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion) || opcion < 1 || opcion > generos.Count) {
            Console.WriteLine("Selección no válida.");
            return;
        }

        string genero = generos[opcion - 1];

        List<Pelicula> resultado = peliculas
            .Where(p => p.Genre.Split(
                new[] { ",", ";" },
                StringSplitOptions.RemoveEmptyEntries)
                .Any(g => g.Trim().Equals(
                    genero, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        ExportarXML(resultado);
    }

    static void ExportarPorAnio(List<Pelicula> peliculas) {
        Console.Write("Introduce el año mínimo: ");

        if (!int.TryParse(Console.ReadLine(), out int anio)) {
            Console.WriteLine("Año no válido.");
            return;
        }

        List<Pelicula> resultado = peliculas.Where(p => {
                
            string valor = p.Premiere.Trim();

            if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha)) {
                return fecha.Year >= anio;
            }

            if (int.TryParse(valor, out int anioPelicula)) {
                return anioPelicula >= anio;
            }

            return false;

        }).ToList();

        ExportarXML(resultado);
    }

    static void ExportarPorIdioma(List<Pelicula> peliculas) {
        List<string> idiomas = peliculas
            .Select(p => p.Language.Trim())
            .Where(i => i != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(i => i)
            .ToList();

        Console.WriteLine("\nIdiomas disponibles:");

        for (int i = 0; i < idiomas.Count; i++) {
            Console.WriteLine($"{i + 1}. {idiomas[i]}");
        }

        Console.Write("Selecciona un idioma: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion) || opcion < 1 || opcion > idiomas.Count) {
            Console.WriteLine("Selección no válida.");
            return;
        }

        string idioma = idiomas[opcion - 1];

        List<Pelicula> resultado = peliculas
            .Where(p => p.Language.Trim().Equals(
                idioma, StringComparison.OrdinalIgnoreCase))
            .ToList();

        ExportarXML(resultado);
    }

    public static void MenuXMLaCSV() {
        if (!File.Exists(rutaXML)) {
            Console.WriteLine(
                "El XML de origen no existe. Selecciona un XML válido.");
            return;
        }

        Console.WriteLine("\n===== XML A CSV =====");
        Console.WriteLine("1. Exportar todo el documento");
        Console.WriteLine("2. Exportar por género");
        Console.WriteLine("3. Exportar por año");
        Console.WriteLine("4. Exportar por idioma");
        Console.Write("Opción: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion)) {
            return;
        }

        List<Pelicula> peliculas = LeerXML(rutaXML);

        switch (opcion) {
            case 1:
                ExportarCSV(peliculas);
                break;

            case 2:
                ExportarPorGeneroCSV(peliculas);
                break;

            case 3:
                ExportarPorAnioCSV(peliculas);
                break;

            case 4:
                ExportarPorIdiomaCSV(peliculas);
                break;

            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }

    static List<Pelicula> LeerXML(string ruta) {
        XDocument documento = XDocument.Load(ruta);

        return documento.Root?
            .Elements("Pelicula")
            .Select(p => new Pelicula(
                (string?)p.Element("Title") ?? "",
                (string?)p.Element("Genre") ?? "",
                (string?)p.Element("Premiere") ?? "",
                (string?)p.Element("Runtime") ?? "",
                (string?)p.Element("IMDBScore") ?? "",
                (string?)p.Element("Language") ?? ""
            )).ToList() ?? new List<Pelicula>();
    }

    static void ExportarCSV(List<Pelicula> peliculas) {
        string destino = PedirDestinoCSV();

        if (destino == "") {
            return;
        }

        using (StreamWriter escritor = new StreamWriter(destino, false, System.Text.Encoding.UTF8)) {
            
            escritor.WriteLine("Title,Genre,Premiere,Runtime,IMDB Score,Language");

            foreach (Pelicula p in peliculas) {
                escritor.WriteLine(string.Join(",", CampoCSV(p.Title), CampoCSV(p.Genre), CampoCSV(p.Premiere), CampoCSV(p.Runtime), CampoCSV(p.IMDBScore), CampoCSV(p.Language)));
            }
        }

        Console.WriteLine("CSV exportado correctamente.");
    }

    static string PedirDestinoCSV() {
        Console.Write("Introduce la ruta del CSV de destino: ");
        string destino = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(destino)) {
            Console.WriteLine("La ruta no puede estar vacía.");
            return "";
        }

        return destino;
    }

    static string CampoCSV(string valor) {
        valor ??= "";

        return "\"" + valor.Replace("\"", "\"\"") + "\"";
    }

    static void ExportarPorGeneroCSV(List<Pelicula> peliculas) {
        List<string> generos = peliculas
            .SelectMany(p => p.Genre.Split(
                new[] { ",", ";" },
                StringSplitOptions.RemoveEmptyEntries))
            .Select(g => g.Trim())
            .Where(g => g != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g)
            .ToList();

        if (generos.Count == 0) {
            Console.WriteLine("No hay géneros disponibles.");
            return;
        }

        for (int i = 0; i < generos.Count; i++) {
            Console.WriteLine($"{i + 1}. {generos[i]}");
        }

        Console.Write("Selecciona un género: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion) || opcion < 1 || opcion > generos.Count) {
            Console.WriteLine("Selección no válida.");
            return;
        }

        string genero = generos[opcion - 1];

        ExportarCSV(peliculas.Where(p => p.Genre.Split(
            new[] { ",", ";" },
            StringSplitOptions.RemoveEmptyEntries)
            .Any(g => g.Trim().Equals(
                genero, StringComparison.OrdinalIgnoreCase)))
            .ToList());
    }

    static void ExportarPorAnioCSV(List<Pelicula> peliculas) {
        Console.Write("Introduce el año mínimo: ");

        if (!int.TryParse(Console.ReadLine(), out int anio)) {
            Console.WriteLine("Año no válido.");
            return;
        }

        List<Pelicula> resultado = peliculas.Where(p => 
        {
            if (DateTime.TryParse(p.Premiere, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha)) {
                return fecha.Year >= anio;
            }

            return int.TryParse(p.Premiere, out int anioPelicula) && anioPelicula >= anio;
        }).ToList();

        ExportarCSV(resultado);
    }

    static void ExportarPorIdiomaCSV(List<Pelicula> peliculas) {
        List<string> idiomas = peliculas
            .Select(p => p.Language.Trim())
            .Where(i => i != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(i => i)
            .ToList();

        if (idiomas.Count == 0) {
            Console.WriteLine("No hay idiomas disponibles.");
            return;
        }

        for (int i = 0; i < idiomas.Count; i++) {
            Console.WriteLine($"{i + 1}. {idiomas[i]}");
        }

        Console.Write("Selecciona un idioma: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion) || opcion < 1 || opcion > idiomas.Count) {
            Console.WriteLine("Selección no válida.");
            return;
        }

        string idioma = idiomas[opcion - 1];

        ExportarCSV(peliculas.Where(p => p.Language.Trim().Equals(idioma, StringComparison.OrdinalIgnoreCase)).ToList());
    }
}