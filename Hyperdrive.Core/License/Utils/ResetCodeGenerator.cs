using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperdrive.Core.License.Utils
{
    public static class ResetCodeGenerator
    {
        public static string GetCode()
        {
            string code = "";
            
            // 1 is number, 2 is letter
            int oldNumber = 0;
            int newNumber = 0;

            for(int x = 0; x < 6; x++)
            {
                if (oldNumber == 1 && newNumber == 1)
                {
                    code += GetRandomLetter();
                    oldNumber = newNumber;
                    newNumber = 2;
                }
                else if (oldNumber == 2 && newNumber == 2)
                {
                    code += GetRandomNumber();
                    oldNumber = newNumber;
                    newNumber = 1;
                }
                else
                {
                    bool randomBool = RandomNumber(0, 2) > 0;
                    if (randomBool)
                    {
                        code += GetRandomLetter();
                        oldNumber = newNumber;
                        newNumber = 2;
                    }
                    else
                    {
                        code += GetRandomNumber();
                        oldNumber = newNumber;
                        newNumber = 1;
                    }
                }
            }

            return code;
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        private static string GetRandomNumber() { return RandomNumber(2, 10).ToString(); }

        private static string GetRandomLetter()
        {
            string[] characterList = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            int index = RandomNumber(0, characterList.Length);

            return characterList[index];
        }
    }
}
