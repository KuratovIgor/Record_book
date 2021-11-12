using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecordBook
{
    public static class RedDiplomaCalculator
    {
        private static SqlConnection _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RecordBookDB"].ConnectionString);

        private static RecBook _recordBook;

        private static int _fiveProbability;
        private static int _fourProbability;

        private static Dictionary<int, int> Marks = new Dictionary<int, int>
        {
            {2, 0},
            {3, 0},
            {4, 0},
            {5, 0}
        };

        public static void CalculateProbabilityRedDiploma(RecBook recordBook)
        {
            _recordBook = recordBook;

            try
            {
                ClearMarks();
                SetMarks();
                Calculate();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void ClearMarks()
        {
            for (int i = 2; i <= 5; i++)
                Marks[i] = 0;
        }

        private static void SetMarks()
        {
            _sqlConnection.Open();

            try
            {
                string command =
                    $"select Оценка, count(Оценка) from Marks where [Номер зачетки] = N'{_recordBook.Number}' group by Оценка";

                SqlCommand sqlCommand = new SqlCommand(command, _sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int mark = Convert.ToInt32(reader.GetValue(0));
                        int count = Convert.ToInt32(reader.GetValue(1));

                        Marks[mark] = count;
                    }
                }
            }
            catch (InvalidCastException)
            {
                throw new Exception("Есть не выставленные оценки!");
            }
            catch (NullReferenceException)
            {
                throw new Exception("Выберите зачетную книжку!");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        private static void Calculate()
        {
            int sumOfCountMarks = Marks.Values.Sum();

            if (sumOfCountMarks != 0)
            {
                _fiveProbability = Marks[5] * 100 / sumOfCountMarks;
                _fourProbability = Marks[4] * 100 / sumOfCountMarks;
            }

            if (Marks[3] != 0 || Marks[2] != 0)
            {
                MessageBox.Show("Получение диплома невозможно, так как в зачетной книжке присутствуют 3 либо 2!");
            }
            else if (_fiveProbability >= 75 && _fourProbability <= 25)
            {
                MessageBox.Show(
                    $"Возможно получение красного диплома!\nПроцентное соотноошение 5 и 4 = {_fiveProbability}% / {_fourProbability}%");
            }
            else
            {
                MessageBox.Show(
                    $"Получение диплома невозможно!\nПроцентное соотноошение 5 и 4 = {_fiveProbability}% / {_fourProbability}%");
            }
        }
    }
}
