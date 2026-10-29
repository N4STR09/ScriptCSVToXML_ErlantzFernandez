using System;
using System.IO;

namespace Practica2_2 {
    internal class Program {
        static string rutaCSV = "";
        static string rutaXML = "";

        static void Main() {
            int opcion;

            do {
                Console.WriteLine("\n===== CONVERSOR CSV / XML =====");
                Console.WriteLine("1. Configurar ruta del CSV");
                Console.WriteLine("2. Configurar ruta del XML");
                Console.WriteLine("3. Convertir CSV a XML");
                Console.WriteLine("4. Convertir XML a CSV");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out opcion)) {
                    opcion = -1;
                }

                switch (opcion) {
                    case 1:
                        Functions.ConfigurarArchivo("CSV");
                        rutaCSV = Functions.RutaCsvOrigen;
                        break;

                    case 2:
                        Functions.ConfigurarArchivo("XML");
                        rutaXML = Functions.RutaXmlDestino;
                        break;

                    case 3:
                        // Si se configuró vía opción 1/2, asegurar que las propiedades estén actualizadas
                        if (!string.IsNullOrWhiteSpace(rutaCSV))
                            Functions.RutaCsvOrigen = rutaCSV;
                        if (!string.IsNullOrWhiteSpace(rutaXML))
                            Functions.RutaXmlDestino = rutaXML;
                        Functions.ConvertirCSVaXML();
                        break;

                    case 4:
                        if (!string.IsNullOrWhiteSpace(rutaXML))
                            Functions.RutaXmlOrigen = rutaXML;
                        if (!string.IsNullOrWhiteSpace(rutaCSV))
                            Functions.RutaCsvDestino = rutaCSV;
                        Functions.ConvertirXMLaCSV();
                        break;

                    case 0:
                        Console.WriteLine("Programa finalizado.");
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }

            } while (opcion != 0);
        }
    }
}