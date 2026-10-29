using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Practica2_2
{
    public static class Functions
    {
        public static string RutaCsvOrigen { get; set; } = "";
        public static string RutaXmlDestino { get; set; } = "";
        public static string RutaXmlOrigen { get; set; } = "";
        public static string RutaCsvDestino { get; set; } = "";

        public static void MenuCSVaXML()
        {
            if (!ArchivoValido(RutaCsvOrigen, "CSV de origen") ||
                !RutaDestinoValida(RutaXmlDestino, "XML de destino"))
            {
                return;
            }

            try
            {
                List<Pelicula> peliculas = LeerCSV(RutaCsvOrigen);

                Console.WriteLine("\n===== CSV A XML =====");
                Console.WriteLine("1. Exportar todas las películas");
                Console.WriteLine("2. Exportar por género");
                Console.WriteLine("3. Exportar por año (ese año o posteriores)");
                Console.WriteLine("4. Exportar por idioma");
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out int opcion))
                {
                    Console.WriteLine("Opción no válida.");
                    return;
                }

                List<Pelicula>? resultado;

                switch (opcion)
                {
                    case 1:
                        resultado = peliculas;
                        break;

                    case 2:
                        resultado = FiltrarPorGenero(peliculas);
                        break;

                    case 3:
                        resultado = FiltrarPorAnio(peliculas);
                        break;

                    case 4:
                        resultado = FiltrarPorIdioma(peliculas);
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        return;
                }

                if (resultado == null)
                    return;

                GuardarXML(resultado, RutaXmlDestino);

                Console.WriteLine(
                    $"Exportación terminada. Películas exportadas: {resultado.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "No se pudo convertir el CSV a XML: " + ex.Message);
            }
        }

        public static void MenuXMLaCSV()
        {
            if (!ArchivoValido(RutaXmlOrigen, "XML de origen") ||
                !RutaDestinoValida(RutaCsvDestino, "CSV de destino"))
            {
                return;
            }

            try
            {
                List<Pelicula> peliculas = LeerXML(RutaXmlOrigen);

                Console.WriteLine("\n===== XML A CSV =====");
                Console.WriteLine("1. Exportar todas las películas");
                Console.WriteLine("2. Exportar por género");
                Console.WriteLine("3. Exportar por año (ese año o posteriores)");
                Console.WriteLine("4. Exportar por idioma");
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out int opcion))
                {
                    Console.WriteLine("Opción no válida.");
                    return;
                }

                List<Pelicula>? resultado;

                switch (opcion)
                {
                    case 1:
                        resultado = peliculas;
                        break;

                    case 2:
                        resultado = FiltrarPorGenero(peliculas);
                        break;

                    case 3:
                        resultado = FiltrarPorAnio(peliculas);
                        break;

                    case 4:
                        resultado = FiltrarPorIdioma(peliculas);
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        return;
                }

                if (resultado == null)
                    return;

                GuardarCSV(resultado, RutaCsvDestino);

                Console.WriteLine(
                    $"Exportación terminada. Películas exportadas: {resultado.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "No se pudo convertir el XML a CSV: " + ex.Message);
            }
        }

        private static List<Pelicula> LeerCSV(string ruta)
        {
            List<Pelicula> peliculas = new List<Pelicula>();

            using (TextFieldParser parser = new TextFieldParser(ruta))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = false;

                if (parser.EndOfData)
                    return peliculas;

                string[] cabeceras =
                    parser.ReadFields() ?? Array.Empty<string>();

                int Columna(string nombre)
                {
                    for (int i = 0; i < cabeceras.Length; i++)
                    {
                        if (cabeceras[i].Trim().Equals(
                            nombre,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return i;
                        }
                    }

                    throw new InvalidDataException(
                        $"No se encuentra la columna obligatoria '{nombre}' en el CSV.");
                }

                int title = Columna("Title");
                int genre = Columna("Genre");
                int premiere = Columna("Premiere");
                int runtime = Columna("Runtime");
                int score = Columna("IMDB Score");
                int language = Columna("Language");

                while (!parser.EndOfData)
                {
                    string[] campos =
                        parser.ReadFields() ?? Array.Empty<string>();

                    int maxIndice = Math.Max(
                        title,
                        Math.Max(
                            genre,
                            Math.Max(
                                premiere,
                                Math.Max(runtime, Math.Max(score, language)))));

                    if (campos.Length <= maxIndice)
                        continue;

                    peliculas.Add(new Pelicula(
                        campos[title],
                        campos[genre],
                        campos[premiere],
                        campos[runtime],
                        campos[score],
                        campos[language]));
                }
            }

            return peliculas;
        }

        private static void GuardarXML(
            List<Pelicula> peliculas,
            string ruta)
        {
            XDocument documento = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(
                    "Peliculas",
                    peliculas.Select(p =>
                        new XElement(
                            "Pelicula",
                            new XElement("Title", p.Title),
                            new XElement("Genre", p.Genre),
                            new XElement("Premiere", p.Premiere),
                            new XElement("Runtime", p.Runtime),
                            new XElement("IMDBScore", p.IMDBScore),
                            new XElement("Language", p.Language)
                        ))
                )
            );

            // Save crea el archivo si todavía no existe.
            documento.Save(ruta);
        }

        private static List<Pelicula> LeerXML(string ruta)
        {
            XDocument documento = XDocument.Load(ruta);

            if (documento.Root == null ||
                documento.Root.Name != "Peliculas")
            {
                throw new InvalidDataException(
                    "El XML debe tener el elemento raíz <Peliculas>.");
            }

            return documento.Root.Elements("Pelicula")
                .Select(p => new Pelicula(
                    (string?)p.Element("Title") ?? "",
                    (string?)p.Element("Genre") ?? "",
                    (string?)p.Element("Premiere") ?? "",
                    (string?)p.Element("Runtime") ?? "",
                    (string?)p.Element("IMDBScore") ?? "",
                    (string?)p.Element("Language") ?? ""))
                .ToList();
        }

        private static void GuardarCSV(
            List<Pelicula> peliculas,
            string ruta)
        {
            // StreamWriter crea el archivo si no existe y lo sobrescribe
            // si existe y el usuario ha confirmado previamente.
            using (StreamWriter escritor = new StreamWriter(
                ruta, false, new UTF8Encoding(true)))
            {
                escritor.WriteLine(
                    "Title,Genre,Premiere,Runtime,IMDB Score,Language");

                foreach (Pelicula p in peliculas)
                {
                    escritor.WriteLine(string.Join(",",
                        CampoCSV(p.Title),
                        CampoCSV(p.Genre),
                        CampoCSV(p.Premiere),
                        CampoCSV(p.Runtime),
                        CampoCSV(p.IMDBScore),
                        CampoCSV(p.Language)));
                }
            }
        }

        private static string CampoCSV(string? valor)
        {
            valor ??= "";

            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        private static List<Pelicula>? FiltrarPorGenero(
            List<Pelicula> peliculas)
        {
            List<string> generos = peliculas
                .SelectMany(p => p.Genre.Split(
                    new[] { ",", ";" },
                    StringSplitOptions.RemoveEmptyEntries))
                .Select(g => g.Trim())
                .Where(g => g.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();

            string? genero = ElegirValor("género", generos);

            if (genero == null)
                return null;

            return peliculas
                .Where(p => p.Genre.Split(
                    new[] { ",", ";" },
                    StringSplitOptions.RemoveEmptyEntries)
                    .Any(g => g.Trim().Equals(
                        genero,
                        StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        private static List<Pelicula>? FiltrarPorIdioma(
            List<Pelicula> peliculas)
        {
            List<string> idiomas = peliculas
                .Select(p => p.Language.Trim())
                .Where(i => i.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(i => i)
                .ToList();

            string? idioma = ElegirValor("idioma", idiomas);

            if (idioma == null)
                return null;

            return peliculas
                .Where(p => p.Language.Trim().Equals(
                    idioma,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static List<Pelicula>? FiltrarPorAnio(
            List<Pelicula> peliculas)
        {
            Console.Write("Introduce el año mínimo: ");

            if (!int.TryParse(Console.ReadLine(), out int anio))
            {
                Console.WriteLine("Año no válido.");
                return null;
            }

            return peliculas
                .Where(p => ObtenerAnio(p.Premiere) >= anio)
                .ToList();
        }

        private static int ObtenerAnio(string premiere)
        {
            if (int.TryParse(premiere.Trim(), out int anio))
                return anio;

            if (DateTime.TryParse(
                premiere,
                CultureInfo.GetCultureInfo("en-US"),
                DateTimeStyles.AllowWhiteSpaces,
                out DateTime fecha))
            {
                return fecha.Year;
            }

            if (DateTime.TryParse(
                premiere,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out fecha))
            {
                return fecha.Year;
            }

            return 0;
        }

        private static string? ElegirValor(
            string tipo,
            List<string> valores)
        {
            if (valores.Count == 0)
            {
                Console.WriteLine($"No hay {tipo}s disponibles.");
                return null;
            }

            Console.WriteLine($"\nSelecciona un {tipo}:");

            for (int i = 0; i < valores.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {valores[i]}");
            }

            Console.Write("Número: ");

            if (!int.TryParse(Console.ReadLine(), out int opcion) ||
                opcion < 1 ||
                opcion > valores.Count)
            {
                Console.WriteLine("Selección no válida.");
                return null;
            }

            return valores[opcion - 1];
        }
        private static bool ArchivoValido(
            string ruta,
            string descripcion)
        {
            if (!string.IsNullOrWhiteSpace(ruta) && File.Exists(ruta))
                return true;

            Console.WriteLine(
                $"Selecciona una ruta válida para el {descripcion} desde el menú principal.");

            return false;
        }

        private static bool RutaDestinoValida(
            string ruta,
            string descripcion)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                Console.WriteLine(
                    $"Configura una ruta para el {descripcion} desde el menú principal.");

                return false;
            }

            try
            {
                string rutaCompleta = Path.GetFullPath(ruta);
                string? carpeta = Path.GetDirectoryName(rutaCompleta);

                if (carpeta != null && !Directory.Exists(carpeta))
                {
                    Console.WriteLine(
                        "La carpeta de destino no existe. Créala primero.");

                    return false;
                }

                // El archivo de destino puede no existir.
                // La exportación lo creará automáticamente.
                return true;
            }
            catch
            {
                Console.WriteLine(
                    $"La ruta del {descripcion} no es válida.");

                return false;
            }
        }
        
        // Configurar la ruta de un CSV o XML,
        // independientemente de si será origen o destino.
        public static void ConfigurarArchivo(string tipo)
        {
            Console.Write($"Introduce la ruta completa del archivo {tipo}: ");

            string ruta = (Console.ReadLine() ?? "")
                .Trim()
                .Trim('"');

            if (string.IsNullOrWhiteSpace(ruta))
            {
                Console.WriteLine("La ruta no puede estar vacía.");
                return;
            }

            try
            {
                ruta = Path.GetFullPath(ruta);

                string? carpeta = Path.GetDirectoryName(ruta);

                // Crear la carpeta si no existe.
                if (!string.IsNullOrEmpty(carpeta) &&
                    !Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // Si el archivo no existe, preguntar si se quiere crear.
                if (!File.Exists(ruta))
                {
                    Console.Write(
                        "El archivo no existe. ¿Quieres crearlo? (S/N): ");

                    string respuesta = (Console.ReadLine() ?? "").Trim();

                    if (!respuesta.Equals(
                        "S", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Operación cancelada.");
                        return;
                    }

                    using (File.Create(ruta))
                    {
                    }

                    Console.WriteLine("Archivo creado correctamente.");
                }

                // Guardar la ruta según el tipo de archivo.
                if (tipo == "CSV")
                {
                    RutaCsvOrigen = ruta;
                }
                else
                {
                    RutaXmlDestino = ruta;
                }

                Console.WriteLine($"Ruta del {tipo} configurada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al configurar el archivo: " + ex.Message);
            }
        }

        // CSV = origen, XML = destino.
        public static void ConvertirCSVaXML()
        {
            if (string.IsNullOrWhiteSpace(RutaCsvOrigen) ||
                !File.Exists(RutaCsvOrigen))
            {
                Console.WriteLine("Configura un CSV de origen válido primero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(RutaXmlDestino))
            {
                Console.WriteLine("Configura una ruta para el XML primero.");
                return;
            }

            if (!ConfirmarSobrescritura(RutaXmlDestino))
                return;

            Functions.MenuCSVaXML();
        }

        // XML = origen, CSV = destino.
        public static void ConvertirXMLaCSV()
        {
            if (string.IsNullOrWhiteSpace(RutaXmlOrigen) ||
                !File.Exists(RutaXmlOrigen))
            {
                Console.WriteLine("Configura un XML de origen válido primero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(RutaCsvDestino))
            {
                Console.WriteLine("Configura una ruta para el CSV primero.");
                return;
            }

            if (!ConfirmarSobrescritura(RutaCsvDestino))
                return;

            Functions.MenuXMLaCSV();
        }

        // Preguntar antes de sobrescribir el destino.
        public static bool ConfirmarSobrescritura(string ruta)
        {
            if (!File.Exists(ruta))
                return true;

            Console.Write(
                "El archivo de destino ya existe. ¿Quieres sobrescribirlo? (S/N): ");

            string respuesta = (Console.ReadLine() ?? "").Trim();

            if (respuesta.Equals(
                "S", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            Console.WriteLine("Conversión cancelada.");
            return false;
        }
    }
}