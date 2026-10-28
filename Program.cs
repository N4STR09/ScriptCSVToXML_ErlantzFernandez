using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using Practica2_2;

namespace Practica2_2;

class Program {
    static string rutaCSV = "";
    static string rutaXML = "";

    static void Main() {
        int opcion;

        do {
            Console.WriteLine("\n===== CONVERSOR CSV - XML =====");
            Console.WriteLine("1. Elegir archivo CSV");
            Console.WriteLine("2. Elegir archivo XML de destino");
            Console.WriteLine("3. Exportar CSV a XML");
            Console.WriteLine("4. Exportar XML a CSV");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion)) {
                opcion = -1;
            }

            switch (opcion) {
                case 1:
                    Console.Write("Introduce la ruta del CSV: ");
                    rutaCSV = Console.ReadLine() ?? "";

                    if (File.Exists(rutaCSV)) {
                        Console.WriteLine("Archivo CSV seleccionado.");
                    } else {
                        Console.WriteLine("El archivo no existe.");
                        rutaCSV = "";
                    }

                    break;

                case 2:
                    Console.Write("Introduce la ruta del XML de destino: ");
                    rutaXML = Console.ReadLine() ?? "";
                    Console.WriteLine("Ruta XML configurada.");
                    break;

                case 3:
                    Functions.MenuCSVaXML();
                    break;

                case 4:
                    Functions.MenuXMLaCSV();
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
