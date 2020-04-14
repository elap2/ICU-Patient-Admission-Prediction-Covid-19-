
namespace ConsoleApp1
{
    using System;

    internal class Program
    {
        /// <summary>
        /// Total ICU beds.
        /// </summary>
        internal const int TotalICUBeds = 640;

        /// <summary>
        /// fraction of ICU occupancy COVID 19 neg.
        /// </summary>
        internal const double FractionOfICUOccupancyCOVID19Neg = 0.55;

        /// <summary>
        /// Lenght of stay 5 days.
        /// </summary>
        private const double LOSFloor1 = 5;

        /// <summary>
        /// Lenght of stay 11 days.
        /// </summary>
        private const double LOSICU11d = 11;

        /// <summary>
        /// Lenght of stay 9 days.
        /// </summary>
        private const double LOSICU9d = 9;

        /// <summary>
        /// Length of Stay 4 days.
        /// </summary>
        private const double LOSFloor2 = 4;

        /// <summary>
        /// Length of Stay 6 days.
        /// </summary>
        private const double LOSFloor3 = 6;

        /// <summary>
        /// Length of Stay 4 days.
        /// </summary>
        private const double LOSFloor4 = 4;

        /// <summary>
        /// Cohort 1.
        /// </summary>
        private const double FractionFloor = 0.704;

        /// <summary>
        /// Cohort 2.
        /// </summary>
        private const double FractionFloorIcuFloor = 0.13;

        /// <summary>
        /// Cohort 3.
        /// </summary>
        private const double FractionFloorIcu = 0.018;

        /// <summary>
        /// Cohort 4.
        /// </summary>
        private const double FractionIcuFloor = 0.13;

        /// <summary>
        /// Cohort 5.
        /// </summary>
        private const double FractionIcu = 0.018;

        private const string V1 = " DAY";
        private const string V2 = "First Initial Cases of COVID-19-Carriers requiring hospitalization in your country during the first 14 Days: ";

        private static double doublingTime;
        private static int initialCases;
        private static int days;

        internal static void Main()
        {
            DateTime dat = DateTime.Now;
            Console.WriteLine("\n\nToday is {0:d} at {0:T}.".Normalize(), dat);
            Console.WriteLine("Rober-Koch_Institut 29.3.2020 12am: There have been over 638,146 confirmed cases WHO " +
                "of coronavirus disease 2019 (COVID-19) in over 202 countries with 30,105 deaths, the World Health Organization characterized COVID-19 as a pandemic.".Normalize());
            Get_the_doubling_time();
            Console.WriteLine("doubling time: " + doublingTime);

            for (int cnt = 14; cnt <= 14 + days; ++cnt)
            {
                double pow_ab = initialCases * Math.Pow(2, cnt / doublingTime); // Total COVID-19(+) Admissions to Hospital

                double cohort_1_dispatched = AC_patients_discount_Cohort1(cnt, initialCases); // dispatched patients from AC 5 days ago
                double aC_patients_total = (pow_ab * FractionFloor) - cohort_1_dispatched + Flow_From_ICU_to_Floor(cnt, initialCases); // Actual occupation of AC

                double dispatched_icu_beds = Icu_patients_discount_Cohort5(cnt, initialCases);
                double cohort_2_icu = Cohort_2(cnt, initialCases); // +++
                double icu_patients_total = (pow_ab * FractionIcu) + (pow_ab * FractionIcuFloor) - dispatched_icu_beds + cohort_2_icu;

                Console.WriteLine("{0}{1}".Normalize(), cnt, V1);
                Console.WriteLine("COVID-19+ Total Hospital Admissions: {0}".Normalize(), pow_ab);
                Console.WriteLine("COVID-19+ Total Hospital Admissions in Normal Floor: {0}".Normalize(), pow_ab * FractionFloor);
                Console.WriteLine("COVID-19+ Acute Care Patients (AC) total occupation at day {0} {1}".Normalize(), cnt, aC_patients_total);
                Console.WriteLine("COVID-19+ Hospital Admissions in Intensiv Care Unit (ICU) at day {0} {1}".Normalize(), cnt, pow_ab * FractionIcu);
                Console.WriteLine("COVID-19+ Hospital Admission to ICU from Floor: {0}".Normalize(), cohort_2_icu);
                Console.WriteLine("Patients ICU dispatched: {0}".Normalize(), dispatched_icu_beds);
                Console.WriteLine("Total ICU Patients: {0}".Normalize(), icu_patients_total);
            }

            Console.ReadLine();
        }
        /// <summary>
        /// gets doubling the time.
        /// </summary>
        private static void Get_the_doubling_time()
        {
            Console.WriteLine("Doubling time for new COVID-19+ admits: ".Normalize());
            Console.WriteLine("If you do not know it, you can enter 3,84 from the doubling_Time taken from Wuhan at the begining of the Epidemic https://doi.org/10.1101/2020.03.03.20030593".Normalize());

            Console.WriteLine("Doubling time of New Covid Admits? (please enter decimal numbers with a , e.g. 2,5 or 6: ".Normalize());
            string doubling_time_string = Console.ReadLine();

            while (string.IsNullOrEmpty(doubling_time_string) || (!double.TryParse(doubling_time_string, out doublingTime)))
            {
                Console.WriteLine("Number invalid".Normalize());
                doubling_time_string = Console.ReadLine();
            }

            doublingTime = Convert.ToDouble(doubling_time_string);

            Console.WriteLine("Number of inital Cases admitted to hospital during the first 14 Days after e.g. One COVID-19+ Carrier was detected at day X.X.2020: ".Normalize());
            string string_initial_cases = Console.ReadLine();

            while (string.IsNullOrEmpty(string_initial_cases) || (!int.TryParse(string_initial_cases, out initialCases)))
            {
                Console.WriteLine("Number invalid".Normalize());
                string_initial_cases = Console.ReadLine();
            }

            initialCases = Convert.ToInt32(string_initial_cases);
            Console.WriteLine(V2 + initialCases.ToString().Normalize());

            Console.WriteLine("Days of projections: ".Normalize()); // days of stay in ICU
            string string_days = Console.ReadLine();
            while (string.IsNullOrEmpty(string_days) || (!int.TryParse(string_days, out days)))
            {
                Console.WriteLine("Number invalid".Normalize());
                string_days = Console.ReadLine();
            }

            days = Convert.ToInt32(string_days);

        }

        /// <summary>
        /// ICU patients discount Chohort 5 dispatched from ICU.
        /// </summary>
        /// <param name="actual_day_of_epidemic">Actual day of epidemic.</param>
        /// <param name="initial_cases">Initial dases.</param>
        /// <returns>Dispatched from ICU.</returns>
        private static double Icu_patients_discount_Cohort5(int actual_day_of_epidemic, int initial_cases)
        {
            double dispatched_icu_patients_cohort_5 = initial_cases * Math.Pow(2, (actual_day_of_epidemic - LOSICU11d) / doublingTime) * FractionIcu; // Cohort5
            double dispatched_icu_patients_cohort_2_and_4 = initial_cases * Math.Pow(2, (actual_day_of_epidemic - LOSICU9d) / doublingTime) * (FractionFloorIcuFloor + FractionFloorIcu); // Cohort 2

            double dispatched_icu_patients = dispatched_icu_patients_cohort_5 + dispatched_icu_patients_cohort_2_and_4;

            return dispatched_icu_patients;
        }

        /// <summary>
        /// AC patients discount Cohort 1 dismissed from hospital.
        /// </summary>
        /// <param name="actual_day_of_epidemic">Actual epidemic day.</param>
        /// <param name="initial_cases">Inital cases.</param>
        /// <returns>dismissed from hospital.</returns>
        private static double AC_patients_discount_Cohort1(int actual_day_of_epidemic, int initial_cases) // Dismiss from Hospital
        {
            double dispatched_AC_patients = initial_cases * Math.Pow(2, (actual_day_of_epidemic - LOSFloor1) / doublingTime) * FractionFloor;

            return dispatched_AC_patients;
        }

        private static double Cohort_2(int actual_day_of_epidemic, int initial_cases) // Flow of Floor patients to the ICU
        {
            double floor_to_ICU_flow_Cohort2 = initial_cases * Math.Pow(2, (actual_day_of_epidemic - LOSFloor2) / doublingTime) * FractionFloorIcuFloor; // AC Patients 4 days ago going to the ICU
            double floor_to_ICU_flow_Cohort3 = initial_cases * Math.Pow(2, (actual_day_of_epidemic - LOSFloor3) / doublingTime) * FractionFloorIcu; // AC Patients 6 days ago going to the ICU
            double floor_to_ICU_flow = floor_to_ICU_flow_Cohort2 + floor_to_ICU_flow_Cohort3;
            return floor_to_ICU_flow;
        }

        private static double Flow_From_ICU_to_Floor(int actual_day_of_epidemic, int initial_cases) // Flow of Patients from ICU to Floor
        {
            double income_AC_patients = initial_cases * Math.Pow(2, (actual_day_of_epidemic - LOSICU9d) / doublingTime) * (FractionFloorIcuFloor + FractionFloorIcu); // flow to floor
            return income_AC_patients;
        }


    }
}

