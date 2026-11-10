using System;

namespace GestionACB
{
    // Creo la clase partido para saber los campos que tengo en mi csv
    public class Partido
    {
        public int Jornada { get; set; }
        public string EquipoLocal { get; set; } = "";
        public string EquipoVisitante { get; set; } = "";
        public int PuntosLocal { get; set; }
        public int PuntosVisitante { get; set; }
        public string MVP { get; set; } = "";
        public int PuntosMVP { get; set; }
        public int AsistenciasMVP { get; set; }
        public int RebotesMVP { get; set; }
        public string EquipoMVP { get; set; } = "";
    }

    class Program
    {
        // Pongo la ruta relativa al archivo CSV
        static string rutaOrigen = "./acb2526.csv";

        static void Main(string[] args)
        {
            //Cargar Partidos
            List<Partido> listaPartidos = CargarDatosCSV(rutaOrigen);

            Console.WriteLine($"\n[EXITO] Se han cargado exitosamente {listaPartidos.Count} partidos del archivo CSV.");

            bool salir = false; //Controla el bucle del menu: si el usuario le da a la opcion salir cambia a True

            while (!salir) //Mientras no le dé a salir, seguirá en el menú. Pero si elegimos el 0, se saldrá del menú
            {
                Console.WriteLine("\n=====================================");
                Console.WriteLine("     GESTION TEMPORADA ACB 25-26");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Ver resultados de un equipo");
                Console.WriteLine("2. Ver resultados de una jornada");
                Console.WriteLine("3. Ver top 3 MVPs");
                Console.WriteLine("4. Ver clasificacion general");
                Console.WriteLine("5. Generar ficheros TXT de todos los equipos");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine()!.Trim();

                switch (opcion)
                {
                    case "1":
                        VerResultadosEquipo(listaPartidos);
                        break;

                    case "2":
                        VerResultadosJornada(listaPartidos);
                        break;

                    case "3":
                        VerTop3MVPs(listaPartidos);
                        break;

                    case "4":
                        VerClasificacionGeneral(listaPartidos);
                        break;
                    case "5":
                        GenerarFicherosTodosEquipos(listaPartidos);
                        break;

                    case "0":
                        Console.WriteLine("\n¡Hasta luego! Gracias por usar la aplicacion.");
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Por favor, introduce un número del 0 al 5.");
                        break;
                }
            }
        }

        //Metodo para leer el CSV y llenar la lista de partidos
        private static List<Partido> CargarDatosCSV(string rutaOrigen)
        {
            List<Partido> listaPartidos = new List<Partido>();

            if (!File.Exists(rutaOrigen))
            {
                Console.WriteLine($"[Error]: No se encuentra el archivo en la ruta '{rutaOrigen}'.");
                return listaPartidos;
            }

            //Leo las lineas del fichero CSV
            string[] lineas = File.ReadAllLines(rutaOrigen);

            //Hago un for para recorrer todas las linea una por una pero desde i = 1 porque i = 0 es la cabecera
            for (int i = 1; i < lineas.Length; i++)
            {
                string lineaActual = lineas[i];

                // Si la linea no esta vacia ni llena de espacios entra en el if sino va al else
                if (!string.IsNullOrWhiteSpace(lineaActual))
                {
                    string[] campos = lineaActual.Split(',');

                    if (campos.Length >= 9)
                    {
                        // Separo "Jornada 1" por el espacio para coger solo el numero "1"
                        string[] partesJornada = campos[0].Trim().Split(' ');
                        // Paso la segunda parte ("1") a entero
                        int numeroJornada = int.Parse(partesJornada[1]);

                        //Separo el campo resultado ("79-69" --> ["79", "69"])
                        string[] partesResultado = campos[3].Split('-');

                        //Creo el objeto Partido metiendo las propiedades con los nuevos campos cambiados
                        Partido partidoActual = new Partido
                        {
                            Jornada = numeroJornada,
                            EquipoLocal = campos[1].Trim(),
                            EquipoVisitante = campos[2].Trim(),
                            PuntosLocal = int.Parse(partesResultado[0].Trim()),
                            PuntosVisitante = int.Parse(partesResultado[1].Trim()),
                            MVP = campos[4].Trim(),
                            PuntosMVP = int.Parse(campos[5].Trim()),
                            AsistenciasMVP = int.Parse(campos[6].Trim()),
                            RebotesMVP = int.Parse(campos[7].Trim()),
                            EquipoMVP = campos[8].Trim()
                        };

                        listaPartidos.Add(partidoActual);
                    }
                }
                else
                {
                    Console.WriteLine($"[Aviso]: Se ha detectado y omitido una línea vacía en la fila {i + 1}.");
                }
            }

            return listaPartidos;
        }

        // ---METODOS PARA EL MENU ---

        //Metodo para la opcion 1 (Ver Resultados por equipo)
        private static void VerResultadosEquipo(List<Partido> listaPartidos)
        {
            Console.Write("\nIntroduce el nombre del equipo: ");
            string equipoBuscado = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(equipoBuscado))
            {
                Console.WriteLine("[Error]: El nombre del equipo no puede estar vacío.");
                return;
            }

            //Busco los partidos donde el equipo sea local o visitante
            List<Partido> partidosEquipo = new List<Partido>();

            foreach (Partido partidoActual in listaPartidos)
            {
                //paso todo a minusculas
                if (partidoActual.EquipoLocal.ToLower() == equipoBuscado.ToLower() ||
                    partidoActual.EquipoVisitante.ToLower() == equipoBuscado.ToLower())
                {
                    partidosEquipo.Add(partidoActual);
                }
            }

            // Si no encontramos partidos para ese equipo, avisamos y salimos
            if (partidosEquipo.Count == 0)
            {
                Console.WriteLine($"[Aviso]: No se han encontrado partidos para el equipo '{equipoBuscado}'.");
                return;
            }

            // Preparo la lista de lineas para imprimirlas por consola y guardarlas en el fichero
            List<string> lineasResultado = new List<string>();
            lineasResultado.Add($"=== RESULTADOS DE {equipoBuscado.ToUpper()} ===");

            foreach (Partido partidoActual in partidosEquipo)
            {
                // Creo la linea formateada con el resultado del partido y en que jornada esta ese partido
                string linea = $"Jornada {partidoActual.Jornada}: {partidoActual.EquipoLocal} {partidoActual.PuntosLocal} - {partidoActual.PuntosVisitante} {partidoActual.EquipoVisitante}";
                lineasResultado.Add(linea);
            }

            //Muestro todas las lineas del resultado por consola
            Console.WriteLine();
            foreach (string linea in lineasResultado)
            {
                Console.WriteLine(linea);
            }

            GuardarEnArchivoPedirNombre(lineasResultado);
        }

        //Metodo para la opcion 2 (Ver Resultados de una jornada)
        private static void VerResultadosJornada(List<Partido> listaPartidos)
        {
            Console.Write("\nIntroduce el número de jornada: ");
            string entradaJornada = Console.ReadLine()!.Trim();

            //Valido de que el usuario ha escrito un número entero válido
            if (!int.TryParse(entradaJornada, out int jornadaBuscada))
            {
                Console.WriteLine("[Error]: Debes introducir un número de jornada válido.");
                return;
            }

            // Busco todos los partidos de esa jornada
            List<Partido> partidosJornada = new List<Partido>();

            foreach (Partido partidoActual in listaPartidos)
            {
                if (partidoActual.Jornada == jornadaBuscada)
                {
                    partidosJornada.Add(partidoActual);
                }
            }

            // Si no hay partidos para esa jornada, avisamos y salimos

            if (partidosJornada.Count == 0)
            {
                Console.WriteLine($"[Aviso]: No se encontraron partidos para la jornada '{jornadaBuscada}'.");
                return;
            }

            // Preparo la lista de lineas para imprimirlas por consola y guardarlas en el fichero
            List<string> lineasResultado = new List<string>();
            lineasResultado.Add($"=== RESULTADOS JORNADA {jornadaBuscada} ===");

            foreach (Partido partidoActual in partidosJornada)
            {
                // Creo la linea formateada con el resultado del partido
                string linea = $"{partidoActual.EquipoLocal} {partidoActual.PuntosLocal} - {partidoActual.PuntosVisitante} {partidoActual.EquipoVisitante}";
                lineasResultado.Add(linea);
            }

            // Muestro los resultados por consola
            Console.WriteLine();
            foreach (string linea in lineasResultado)
            {
                Console.WriteLine(linea);
            }

            GuardarEnArchivoPedirNombre(lineasResultado);
        }

        private static void VerTop3MVPs(List<Partido> listaPartidos)
        {
            //Creo una copia de la lista para no modificar la lista original al buscar el top 3 MVPs
            List<Partido> copiaPartidos = new List<Partido>(listaPartidos);

            //Lista para guardar las lineas de texto que mostramos y guardamos en el .txt al final
            List<string> lineasResultado = new List<string>();
            lineasResultado.Add("=== TOP 3 MVPs DE LA TEMPORADA ===");

            //Hago un for para que saque solo el 1º, 2º y 3º MVP de toda la temporada
            for (int puesto = 1; puesto <= 3; puesto++)
            {
                if (copiaPartidos.Count == 0)
                {
                    break; //uso break para salir del bucle
                }

                Partido mejorPartido = copiaPartidos[0];

                //Recorro la lista copia buscando el partido con mas PuntosMVP
                foreach (Partido partidoActual in copiaPartidos)
                {
                    if (partidoActual.PuntosMVP > mejorPartido.PuntosMVP)
                    {
                        mejorPartido = partidoActual;
                    }
                }

                //Guardo la linea con los datos del MVP encontrado
                string linea = $"{puesto}º Posición: {mejorPartido.MVP} ({mejorPartido.EquipoMVP}) - Puntos: {mejorPartido.PuntosMVP}, Asistencias: {mejorPartido.AsistenciasMVP}, Rebotes: {mejorPartido.RebotesMVP}";
                lineasResultado.Add(linea);

                //Lo quito de la copia para que la próxima vez salga el siguiente mejor
                copiaPartidos.Remove(mejorPartido);
            }

            //Muestro todas las lineas por consola
            Console.WriteLine(); //salto de linea vacio
            foreach (string linea in lineasResultado)
            {
                Console.WriteLine(linea);
            }

            GuardarEnArchivoPedirNombre(lineasResultado);
        }

        private static void VerClasificacionGeneral(List<Partido> listaPartidos)
        {
            //Obtengo una lista de nombres de equipos no repetidos
            List<string> nombresEquipos = new List<string>();

            foreach (Partido partidoAcual in listaPartidos)
            {
                //Si la lista de equipos no contiene al equipo local lo añade
                if (!nombresEquipos.Contains(partidoAcual.EquipoLocal))
                {
                    nombresEquipos.Add(partidoAcual.EquipoLocal);
                }
                if (!nombresEquipos.Contains(partidoAcual.EquipoVisitante))
                {
                    nombresEquipos.Add(partidoAcual.EquipoVisitante);
                }
            }

            //Calculo las victorias y las derrotas de cada equipo
            List<int> victoriasEquipos = new List<int>();
            List<int> derrotasEquipos = new List<int>();

            foreach (string equipoActual in nombresEquipos)
            {
                int victorias = 0;
                int derrotas = 0;

                foreach (Partido partidoActual in listaPartidos)
                {
                    // Compruebo cuando juega como local
                    if (partidoActual.EquipoLocal.ToLower() == equipoActual.ToLower())
                    {
                        if (partidoActual.PuntosLocal > partidoActual.PuntosVisitante)
                        {
                            victorias++;
                        }
                        else if (partidoActual.PuntosLocal < partidoActual.PuntosVisitante)
                        {
                            derrotas++;
                        }
                    }
                    // Compruebo cuando juega como visitante
                    else if (partidoActual.EquipoVisitante.ToLower() == equipoActual.ToLower())
                    {
                        if (partidoActual.PuntosVisitante > partidoActual.PuntosLocal)
                        {
                            victorias++;
                        }
                        else if (partidoActual.PuntosVisitante < partidoActual.PuntosLocal)
                        {
                            derrotas++;
                        }
                    }
                }
                victoriasEquipos.Add(victorias);
                derrotasEquipos.Add(derrotas);
            }

            //Ordeno y hago la tabla de Clasificacion
            List<string> lineasResultado = new List<string>();
            lineasResultado.Add("=== CLASIFICACIÓN GENERAL DE LA TEMPORADA ===");

            List<string> copiaNombres = new List<string>(nombresEquipos);
            List<int> copiaVictorias = new List<int>(victoriasEquipos);
            List<int> copiaDerrotas = new List<int>(derrotasEquipos);

            int posicion = 1;

            while (copiaNombres.Count > 0)
            {
                int indiceMax = 0;

                for (int i = 1; i < copiaVictorias.Count; i++)
                {
                    if (copiaVictorias[i] > copiaVictorias[indiceMax])
                    {
                        indiceMax = i;
                    }
                }

                // Creo la linea formateada para saber cuantas derrotas y victorias tiene cada equipo
                string linea = $"{posicion}º - {copiaNombres[indiceMax]}: {copiaVictorias[indiceMax]}V - {copiaDerrotas[indiceMax]}D";
                lineasResultado.Add(linea);

                // Elimino de las copias para que en la siguiente vuelta sacamos al siguiente equipo
                copiaNombres.RemoveAt(indiceMax);
                copiaVictorias.RemoveAt(indiceMax);
                copiaDerrotas.RemoveAt(indiceMax);

                posicion++;
            }

            // Muestro la clasificación por consola
            Console.WriteLine();
            foreach (string linea in lineasResultado)
            {
                Console.WriteLine(linea);
            }

            GuardarEnArchivoPedirNombre(lineasResultado);
        }

        // Método para la opción 5 (Generar ficheros TXT de todos los equipos)
        private static void GenerarFicherosTodosEquipos(List<Partido> listaPartidos)
        {
            //Obtengo una lista de nombres de equipos no repetidos
            List<string> nombresEquipos = new List<string>();

            foreach (Partido partidoAcual in listaPartidos)
            {
                //Si la lista de equipos no contiene al equipo local lo añade
                if (!nombresEquipos.Contains(partidoAcual.EquipoLocal))
                {
                    nombresEquipos.Add(partidoAcual.EquipoLocal);
                }
                if (!nombresEquipos.Contains(partidoAcual.EquipoVisitante))
                {
                    nombresEquipos.Add(partidoAcual.EquipoVisitante);
                }
            }
            Console.WriteLine("\n[Procesando]: Generando archivos .txt para cada equipo...");
            int contArchivos = 0;

            //Recorro equipo por equipo para hacer su archivo de texto
            foreach (string equipoActual in nombresEquipos)
            {
                List<string> lineasResultado = new List<string>();
                lineasResultado.Add($"=== RESULTADOS DE {equipoActual.ToUpper()} ===");

                //busco todos los partidos donde juegue este equipo
                foreach (Partido partidoActual in listaPartidos)
                {
                    if (partidoActual.EquipoLocal.ToLower() == equipoActual.ToLower() || partidoActual.EquipoVisitante.ToLower() == equipoActual.ToLower())
                    {
                        string linea = $"Jornada {partidoActual.Jornada}: {partidoActual.EquipoLocal} {partidoActual.PuntosLocal} - {partidoActual.PuntosVisitante} {partidoActual.EquipoVisitante}";
                        lineasResultado.Add(linea);
                    }
                }

                string nombreArchivo = equipoActual.Replace(" ", "_") + ".txt";
                string rutaDestino = $"./{nombreArchivo}";

                File.WriteAllLines(rutaDestino, lineasResultado);
                contArchivos++;
            }

            Console.WriteLine($"[EXITO]: Se han generado correctamente {contArchivos} archivos .txt de equipos en el directorio actual.");
        }

        // Método auxiliar para pedir el nombre del fichero por consola y guardarlo (cumple estrictamente el enunciado)
        private static void GuardarEnArchivoPedirNombre(List<string> lineasResultado)
        {
            Console.Write("\n¿Quieres guardar estos datos en un archivo .txt? (s/n): ");
            string respuesta = Console.ReadLine()!.Trim().ToLower();

            if (respuesta == "s" || respuesta == "si")
            {
                Console.Write("Introduce el nombre del archivo (Por ejemplo: mi_resultado.txt): ");
                string nombreFichero = Console.ReadLine()!.Trim();

                if (string.IsNullOrWhiteSpace(nombreFichero))
                {
                    Console.WriteLine("[Error]: El nombre del archivo no puede estar vacío. Cancelando guardado.");
                    return;
                }

                if (!nombreFichero.EndsWith(".txt"))
                {
                    nombreFichero += ".txt";
                }

                string rutaDestino = $"./{nombreFichero}";
                File.WriteAllLines(rutaDestino, lineasResultado);
                Console.WriteLine($"[EXITO]: Archivo guardado correctamente en la ruta '{rutaDestino}'.");
            }
        }
    }
}