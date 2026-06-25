using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS2026_PT1_GrupaA.Test
{
    public class GameTestDataFromFile
    {
        public static IEnumerable Get_CalculateIncome_OKInput_SuccessfulCalculation_TestData(string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                bool isFirstLine = true;

                while ((line = sr.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    string[] parts = line.Split('\t');

                    if (parts.Length == 4)
                    {
                        int amountOfFish = int.Parse(parts[0]);
                        int amountOfBait = int.Parse(parts[1]);
                        bool hasBoat = bool.Parse(parts[2]);

                        Game.Score expectedScore = (Game.Score)Enum.Parse(typeof(Game.Score), parts[3]);

                        yield return new TestCaseData(amountOfFish, amountOfBait, hasBoat, expectedScore);
                    }
                }
            }
        }
    }
}