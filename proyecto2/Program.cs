using System;

namespace SimuladorGranja
{
    class Program
    {
        // Variables globales 
        static decimal dinero;
        static int empleados;
        static decimal sueldoPorEmpleado;
        static int mesesTotales;
        static int filas, columnas;
        static int[,] tipoCultivo;
        static int[,] mesesCrecimiento;
        static bool[,] regadaEsteMes;
        static int mesesPasados = 0;
        static decimal ingresosTotal = 0;
        static decimal egresosTotal = 0;
        static int[] sembradas = new int[4];
        static int[] cosechadas = new int[4];
        static int totalRiegos = 0;

        static void Main(string[] args)
        {
            Console.Write("Dinero inicial: ");
            dinero = decimal.Parse(Console.ReadLine());
            Console.Write("Número de empleados: ");
            empleados = int.Parse(Console.ReadLine());
            Console.Write("Sueldo por empleado: ");
            sueldoPorEmpleado = decimal.Parse(Console.ReadLine());
            Console.Write("Meses a simular: ");
            mesesTotales = int.Parse(Console.ReadLine());
            Console.Write("Filas de parcelas: ");
            filas = int.Parse(Console.ReadLine());
            Console.Write("Columnas: ");
            columnas = int.Parse(Console.ReadLine());

            tipoCultivo = new int[filas, columnas];
            mesesCrecimiento = new int[filas, columnas];
            regadaEsteMes = new bool[filas, columnas];

            while (mesesPasados < mesesTotales && dinero > 0)
            {
                MostrarMenu();
                int opcion = int.Parse(Console.ReadLine());

                if (opcion == 1) Sembrar();
                else if (opcion == 2) Regar();
                else if (opcion == 3) Consultar();
                else if (opcion == 4) AvanzarMes();
                else if (opcion == 5)
                {
                    MostrarReporte();
                    return;
                }
                else Console.WriteLine("Opción no válida.");
            }
            MostrarReporte();
        }
    
        // CULTIVOS
        static int ObtenerMesesNecesarios(int cultivo)
        {
            if (cultivo == 1) return 2;
            if (cultivo == 2) return 3;
            if (cultivo == 3) return 4;
            return 0;
        }

        static decimal ObtenerIngreso(int cultivo)
        {
            if (cultivo == 1) return 450;
            if (cultivo == 2) return 650;
            return 900;
        }

        // AVANZAR MES (pago, crecimiento, cosecha)
        static void AvanzarMes()
        {
            decimal nomina = empleados * sueldoPorEmpleado;
            dinero -= nomina;
            egresosTotal += nomina;
            Console.WriteLine($"Se pagó Q{nomina} a los empleados.");

            if (dinero <= 0)
            {
                Console.WriteLine("¡Dinero agotado! Fin de la simulación.");
                return;
            }

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if (tipoCultivo[i, j] != 0)
                    {
                        int incremento = regadaEsteMes[i, j] ? 2 : 1;
                        mesesCrecimiento[i, j] += incremento;

                        int necesario = ObtenerMesesNecesarios(tipoCultivo[i, j]);
                        if (mesesCrecimiento[i, j] >= necesario)
                        {
                            decimal ingreso = ObtenerIngreso(tipoCultivo[i, j]);
                            dinero += ingreso;
                            ingresosTotal += ingreso;
                            cosechadas[tipoCultivo[i, j]]++;
                            string nombre = (tipoCultivo[i, j] == 1) ? "Papa" :
                                            (tipoCultivo[i, j] == 2) ? "Tomate" : "Fresa";
                            Console.WriteLine($"¡Cosechado {nombre} en ({i+1},{j+1})! +Q{ingreso}");

                            tipoCultivo[i, j] = 0;
                            mesesCrecimiento[i, j] = 0;
                        }
                    }
                }
            }

            for (int i = 0; i < filas; i++)
                for (int j = 0; j < columnas; j++)
                    regadaEsteMes[i, j] = false;

            mesesPasados++;
            Console.WriteLine($"Avanzado al mes {mesesPasados + 1}");
        }


        // REGAR (costo Q40)
        static void Regar()
        {
            Console.Write("Fila (1.." + filas + "): ");
            int f = int.Parse(Console.ReadLine()) - 1;   // convertir a base 0
            Console.Write("Columna (1.." + columnas + "): ");
            int c = int.Parse(Console.ReadLine()) - 1;

            if (f < 0 || f >= filas || c < 0 || c >= columnas)
            {
                Console.WriteLine($"Coordenada inválida. Debe estar entre 1 y {filas} para filas, 1 y {columnas} para columnas.");
                return;
            }
            if (tipoCultivo[f, c] == 0)
            {
                Console.WriteLine("No hay cultivo en esta parcela.");
                return;
            }
            if (regadaEsteMes[f, c])
            {
                Console.WriteLine("Esta parcela ya fue regada este mes.");
                return;
            }
            if (dinero < 40)
            {
                Console.WriteLine("No tienes suficiente dinero para regar.");
                return;
            }

            dinero -= 40;
            egresosTotal += 40;
            totalRiegos++;
            regadaEsteMes[f, c] = true;
            Console.WriteLine($"Parcela ({f+1},{c+1}) regada. Costo Q40.");
        }

       
        // SEMBRAR 
        static void Sembrar()
        {
            Console.Write("Fila (1.." + filas + "): ");
            int f = int.Parse(Console.ReadLine()) - 1;
            Console.Write("Columna (1.." + columnas + "): ");
            int c = int.Parse(Console.ReadLine()) - 1;

            if (f < 0 || f >= filas || c < 0 || c >= columnas)
            {
                Console.WriteLine($"Coordenada inválida. Debe estar entre 1 y {filas} para filas, 1 y {columnas} para columnas.");
                return;
            }
            if (tipoCultivo[f, c] != 0)
            {
                Console.WriteLine("Ya hay un cultivo en esa parcela.");
                return;
            }

            Console.Write("Tipo (1=Papa, 2=Tomate, 3=Fresa): ");
            int tipo = int.Parse(Console.ReadLine());
            if (tipo < 1 || tipo > 3)
            {
                Console.WriteLine("Tipo de cultivo no válido.");
                return;
            }

            tipoCultivo[f, c] = tipo;
            mesesCrecimiento[f, c] = 0;
            regadaEsteMes[f, c] = false;
            sembradas[tipo]++;
            Console.WriteLine($"Siembra exitosa en ({f+1},{c+1}).");
        }

    
        // CONSULTAR
        static void Consultar()
        {
            Console.Write("Fila (1.." + filas + "): ");
            int f = int.Parse(Console.ReadLine()) - 1;
            Console.Write("Columna (1.." + columnas + "): ");
            int c = int.Parse(Console.ReadLine()) - 1;

            if (f < 0 || f >= filas || c < 0 || c >= columnas)
            {
                Console.WriteLine($"Coordenada inválida. Debe estar entre 1 y {filas} para filas, 1 y {columnas} para columnas.");
                return;
            }

            if (tipoCultivo[f, c] == 0)
            {
                Console.WriteLine($"Parcela ({f+1},{c+1}) está vacía. Disponible para sembrar.");
            }
            else
            {
                string nombre = (tipoCultivo[f, c] == 1) ? "Papa" :
                                (tipoCultivo[f, c] == 2) ? "Tomate" : "Fresa";
                int necesario = ObtenerMesesNecesarios(tipoCultivo[f, c]);
                Console.WriteLine($"Cultivo: {nombre}");
                Console.WriteLine($"Crecimiento: {mesesCrecimiento[f, c]} / {necesario} meses");
                Console.WriteLine($"Regada este mes: {(regadaEsteMes[f, c] ? "Sí" : "No")}");
            }
        }


        // MENÚ Y REPORTE
        static void MostrarMenu()
        {
            Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
            Console.WriteLine("1. Sembrar");
            Console.WriteLine("2. Regar parcela");
            Console.WriteLine("3. Consultar parcela");
            Console.WriteLine("4. Avanzar de mes");
            Console.WriteLine("5. Salir");
            Console.WriteLine($"Dinero actual: Q{dinero}");
            Console.WriteLine($"Mes actual: {mesesPasados + 1}/{mesesTotales}");
            Console.Write("Opción: ");
        }

        static void MostrarReporte()
        {
            Console.WriteLine("\n=== REPORTE FINAL ===");
            Console.WriteLine($"Dinero final: Q{dinero}");
            Console.WriteLine($"Total de ingresos: Q{ingresosTotal}");
            Console.WriteLine($"Total de egresos: Q{egresosTotal}");
            Console.WriteLine($"Meses simulados: {mesesPasados}");
            Console.WriteLine($"Sembradas - Papa: {sembradas[1]}, Tomate: {sembradas[2]}, Fresa: {sembradas[3]}");
            Console.WriteLine($"Cosechadas - Papa: {cosechadas[1]}, Tomate: {cosechadas[2]}, Fresa: {cosechadas[3]}");
            Console.WriteLine($"Total de riegos realizados: {totalRiegos}");

            int vacias = 0;
            for (int i = 0; i < filas; i++)
                for (int j = 0; j < columnas; j++)
                    if (tipoCultivo[i, j] == 0) vacias++;
            Console.WriteLine($"Parcelas vacías al final: {vacias}");
        }
    }
}