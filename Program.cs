﻿using System;

namespace ConsoleApp3
{
    class Program
    {
        public const int total_ICU_beds = 640;
        public const double fraction_of_ICU_occupancy_COVID_19Neg = 0.55;

        private const double LOS_Floor_1 = 5; // Length of stay 5 days
        private const double LOS_ICU_11d = 11; // Length of stay 11 days
        private const double LOS_ICU_9d = 9; // Lenght of stay 9 days
        private const double LOS_Floor_2 = 4; // Length of Stay 4 days
        private const double LOS_Floor_3 = 6; // Length of Stay 6 days
        private const double LOS_Floor_4 = 4; // Length of Stay 4 days

        private const double fraction_floor = 0.704; // Cohort 1
        private const double fraction_floor_icu_floor = 0.13; // Cohort 2
        private const double fraction_floor_icu = 0.018; // Cohort 3
        private const double fraction_icu_floor = 0.13; // Cohort 4
        private const double fraction_icu = 0.018; // Cohort 5
        static double doubling_time;
        static int initial_cases;
        static int days;

        private const string V1 = " DAY";
        private const string V2 = "First Initial Cases of COVID-19-Carriers requiring hospitalization in your country during the first 14 Days: ";
       
        static void get_the_doubling_time()
        {
            //private const double doubling_time = 3.84;
            Console.WriteLine("Doubling time for new COVID-19+ admits: ");
            Console.WriteLine("If you do not know it, you can enter 3,84 from the doubling_Time taken from Wuhan at the begining of the Epidemic https://doi.org/10.1101/2020.03.03.20030593");
            
            Console.WriteLine("Doubling time of New Covid Admits? (please enter decimal numbers with a , e.g. 2,5 or 6: ");
            string doubling_time_string = Console.ReadLine();
           
            while ((string.IsNullOrEmpty(doubling_time_string)) || (!double.TryParse(doubling_time_string, out doubling_time)))
            {
                Console.WriteLine("Number invalid");
                doubling_time_string = Console.ReadLine();
            }
            doubling_time = Convert.ToDouble(doubling_time_string);

            Console.WriteLine("Number of inital Cases admitted to hospital during the first 14 Days after e.g. One COVID-19+ Carrier was detected at day X.X.2020: ");
            string string_initial_cases = Console.ReadLine();
            
            while ((string.IsNullOrEmpty(string_initial_cases)) || (!int.TryParse(string_initial_cases, out initial_cases)))
            {
                Console.WriteLine("Number invalid");
                string_initial_cases = Console.ReadLine();
            }
            initial_cases = Convert.ToInt32(string_initial_cases);
            Console.WriteLine(V2 + initial_cases.ToString());

            Console.WriteLine("Days of projections: "); // days of stay in ICU
            string string_days = Console.ReadLine();
            while ((string.IsNullOrEmpty(string_days)) || (!int.TryParse(string_days, out days)))
            {
                Console.WriteLine("Number invalid");
                string_days = Console.ReadLine();
            }
            days = Convert.ToInt32(string_days);

        }
        static double icu_patients_discount_Cohort5(int actual_day_of_epidemic, int initial_cases) // dispatched from ICU
        {
            //bool isDay_ICU_X_arrived = ((number_of_day/LOS_ICU_X) % 1) == 0; // check of the number of days reached 11 days (Day / Counted Day should be integer)
            //bool isDay_ICU_Y_arrived = ((number_of_day/LOS_ICU_X) % 1) == 0; // check of the number of days reached 9 days (Day / Counted Day should be integer)
            double dispatched_icu_patients_cohort_5 = initial_cases * Math.Pow(2, ((actual_day_of_epidemic - LOS_ICU_11d) / doubling_time)) * fraction_icu; // Cohort5
            double dispatched_icu_patients_cohort_2_and_4 = initial_cases * Math.Pow(2, ((actual_day_of_epidemic - LOS_ICU_9d) / doubling_time)) * (fraction_floor_icu_floor + fraction_floor_icu); // Cohort 2

            double dispatched_icu_patients = dispatched_icu_patients_cohort_5 + dispatched_icu_patients_cohort_2_and_4;

            return dispatched_icu_patients;
        }
        static double AC_patients_discount_Cohort1(int actual_day_of_epidemic, int initial_cases) // Dismiss from Hospital
        {
            double dispatched_AC_patients = initial_cases * Math.Pow(2, ((actual_day_of_epidemic - LOS_Floor_1) / doubling_time)) * fraction_floor;

            return dispatched_AC_patients;
        }
        static double Cohort_2(int actual_day_of_epidemic, int initial_cases) // Flow of Floor patients to the ICU
        {
            double floor_to_ICU_flow_Cohort2 = initial_cases * Math.Pow(2, ((actual_day_of_epidemic - LOS_Floor_2) / doubling_time)) * fraction_floor_icu_floor; // AC Patients 4 days ago going to the ICU
            double floor_to_ICU_flow_Cohort3 = initial_cases * Math.Pow(2, ((actual_day_of_epidemic - LOS_Floor_3) / doubling_time)) * fraction_floor_icu; // AC Patients 6 days ago going to the ICU
            double floor_to_ICU_flow = floor_to_ICU_flow_Cohort2 + floor_to_ICU_flow_Cohort3;
            return floor_to_ICU_flow;
        }

        static double Flow_From_ICU_to_Floor(int actual_day_of_epidemic, int initial_cases) // Flow of Patients from ICU to Floor
        {
            double income_AC_patients = initial_cases * Math.Pow(2, ((actual_day_of_epidemic - LOS_ICU_9d) / doubling_time)) * (fraction_floor_icu_floor + fraction_floor_icu); // flow to floor
            return income_AC_patients;
        }

        static void Main(string[] args)
        {
            DateTime dat = DateTime.Now;
            Console.WriteLine("");

            Console.WriteLine("\nToday is {0:d} at {0:T}.", dat);
            Console.WriteLine("Rober-Koch_Institut 29.3.2020 12am: There have been over 638,146 confirmed cases WHO " +
                "of coronavirus disease 2019 (COVID-19) in over 202 countries with 30,105 deaths, the World Health Organization characterized COVID-19 as a pandemic.");


            get_the_doubling_time();
            Console.WriteLine("doubling time: " + doubling_time);
            
            
            for (int cnt = 14; cnt <= 14 + days; ++cnt)
            {
                double pow_ab = initial_cases * Math.Pow(2, (cnt / doubling_time)); // Total COVID-19(+) Admissions to Hospital

                double Cohort_1_dispatched = AC_patients_discount_Cohort1(cnt, initial_cases); // dispatched patients from AC 5 days ago
                double AC_patients_total = pow_ab * fraction_floor - Cohort_1_dispatched + Flow_From_ICU_to_Floor(cnt, initial_cases); // Actual occupation of AC 

                double dispatched_icu_beds = icu_patients_discount_Cohort5(cnt, initial_cases);
                double cohort_2_icu = Cohort_2(cnt, initial_cases); //+++
                double icu_patients_total = pow_ab * fraction_icu + pow_ab * fraction_icu_floor - dispatched_icu_beds + cohort_2_icu;

                Console.WriteLine(cnt.ToString() + V1);
                Console.WriteLine("COVID-19+ Total Hospital Admissions: " + pow_ab);
                Console.WriteLine("COVID-19+ Total Hospital Admissions in Normal Floor: " + pow_ab * fraction_floor);
                Console.WriteLine("COVID-19+ Acute Care Patients (AC) total occupation at day " + cnt + " " + AC_patients_total);

                Console.WriteLine("COVID-19+ Hospital Admissions in Intensiv Care Unit (ICU) at day " + cnt + " " + pow_ab * fraction_icu);
                Console.WriteLine("COVID-19+ Hospital Admission to ICU from Floor: " + cohort_2_icu);
                Console.WriteLine("Patients ICU dispatched: " + dispatched_icu_beds);
                Console.WriteLine("Total ICU Patients: " + icu_patients_total);
            }
            Console.ReadLine();
        }
    }
}