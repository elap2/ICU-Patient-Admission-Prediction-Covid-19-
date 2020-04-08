# A ICU-Patient-Admission-Prediction-Covid-19+
To do an estimation of how many patients are being admitted to the intensive care unit or acute care patients is essential for hospital planning during the period of NPI (Non-Pharmacological Interventions) and also during their release (Lockdown Release). For this purpose the doubling is an essential parameter.

This App created in Microsoft Visual Studio 2019 is a Console App (.NET Core) written in C sharp. It can run on Windows, Linux and MacOs. 

#### MacOS: https://dotnet.microsoft.com/download/dotnet-core/3.1
#### Linux: https://dotnet.microsoft.com/download

## B Background
A model of prediction of ICU Beds needed was online on March 26th 2020 as a pre-print: https://doi.org/10.1101/2020.03.24.20042762 
Corresponding Author: David Scheinker from Lucile Packard Children’s Hospital, Stanford, CA. 
First Authors: Teng Zhang and Kelly McFarlane, Harvard Medical School, Standford Department of Management Science and Engineering, Stanford University School of Engineering, Stanford, CA.
#### To view the on-line calculator please click on this link:
https://surf.stanford.edu/covid-19-tools/covid-19-hospital-projections/ 

#### The doubling time is an essential parameter for understanding how all of us is being affected by SARS-Cov-2. For further reading follow these publications: 

Quantifying SARS-CoV-2 transmission suggests epidemic control with digital contact tracing
Luca Ferretti1,*, Chris Wymant1,*, Michelle Kendall1, Lele Zhao1, Anel Nurtay1, Lucie Abeler-Dörner1, Michael Parker2, David Bonsall1,3,†, Christophe Fraser1,4,†,‡
Science  31 Mar 2020: eabb6936 DOI: 10.1126/science.abb6936
https://science.sciencemag.org/content/early/2020/03/30/science.abb6936

I. Dorigatti, L. Okell, A. Cori, N. Imai, M. Baguelin, S. Bhatia, A. Boonyasiri, Z. Cucunubá, G. Cuomo-Dannenburg, R. FitzJohn, H. Fu, K. Gaythorpe, A. Hamlet, W. Hinsley, N. Hong, M. Kwun, D. Laydon, G. Nedjati-Gilani, S. Riley, S. van Elsland, E. Volz, H. Wang, R. Wang, C. Walters, X. Xi, C. Donnelly, A. Ghani, N. Ferguson, Report 4: Severity of 2019-Novel Coronavirus (nCoV) (10 February 2020); www.imperial.ac.uk/media/imperial-college/medicine/sph/ide/gida-fellowships/Imperial-College-COVID19-severity-10-02-2020.pdf.

#### Changes: The ConsoleApp3's Output prints the number of ICU beds accumulation over time according to the doubling time of disease transmision. The ConsoleApp3 does not include the percentage of ICU beds needed due to NON-COVID-19+ Patients (unlike from https://doi.org/10.1101/2020.03.24.20042762). 
 
# C Run this App doing the following steps: 
### 1. Clone or Download all the files into one folder. 
### 2. Run the .exe

# D What is the advantage of this App?

## The ConsoleApp3 
### 1. does not need to be online 
### 2. It is straight forward and it's output gives you the number of patients admitted to the hospital, to the ICU, to the AC (acute care beds

# D Example of usage:

<table>
    <thead>
        <tr>
            <th colspan="3">Table 1: Patient Cohorts and Length of Stay Estimates</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>COVID Patient Cohorts
                <table>
                    <tbody>
                        <tr><th>Index</th><th>Path</th><th>Fraction of Patients %</th></tr>
                        <tr><td>1</td><td>Floor</td><td>70.4</td></tr>
                        <tr><td>2</td><td>Floor->ICU->Floor</td><td>13.0</td></tr>
                        <tr><td>3</td><td>Floor->ICU</td><td>1.8</td></tr>
                        <tr><td>4</td><td>ICU->Floor</td><td>13.0</td></tr>
                        <tr><td>5</td><td>ICU</td><td>1,8</td></tr>
                    </tbody>
                </table>
            </td>
            <td>(Length of Stay)Estimates in Model
            <table>
                    <tbody>
                      <tr><th>LOS Floor</th><th>LOS ICU</th><th>LOS Floor</th><th>Total LOS</th></tr>
                      <tr><td>5</td><td>  </td><td> </td><td>5</td></tr>
                      <tr><td>4</td><td>9</td><td>4</td><td>17</td></tr>
                      <tr><td>6</td><td>9</td><td> </td><td>15</td></tr>
                      <tr><td> </td><td>9</td><td>4</td><td>13</td></tr>
                      <tr><td> </td><td>11</td><td> </td><td>11</td></tr>  
                    </tbody>
                </table>
            </td>            
        </tr>
    </tbody>
</table>

<html>
    <body>
        <code>


    using System;

    namespace ConsoleApp3
    {
        class Program
        {
        private const double LOS_Floor_1 = 5; // Length of stay 5 days
        private const double LOS_ICU_11d = 11; // Length of stay 11 days
        private const double LOS_ICU_9d = 9; // Lenght of stay 9 days
        private const double LOS_Floor_2 = 4; // Length of Stay 4 days
        private const double LOS_Floor_3 = 6; // Length of Stay 6 days

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
   </code>
   </body>
</html>

