using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HW.Profiling.PassHashGenerator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] salt = GetSalt(useConst: true);

            string passwordText = "passWord123  _";
            System.Console.WriteLine("passwordText: " + passwordText);



            #region GeneratePasswordHashUsingSalt
            //Run(GeneratePasswordHashUsingSalt, "GeneratePasswordHashUsingSalt", passwordText, salt);

            /* 
            Summary:

            Function Name	                                                            Number of Calls	        Elapsed Inclusive Time %	Elapsed Exclusive Time %	Avg Elapsed Inclusive Time	Avg Elapsed Exclusive Time	Module Name
            System.Security.Cryptography.DeriveBytes.GetBytes(int32)	                1	                    67.29	                    67.29	                    52.36	                    52.36	                    mscorlib.dll
            HW.Profiling.PassHashGenerator.Console.Program.Main(string[])	            1	                    100.00	                    27.79	                    77.81	                    21.62	                    HW.Profiling.PassHashGenerator.Console.exe
            System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string,uint8[],int32)	1	                    2.32	                    2.32	                    1.80	                    1.80	                    mscorlib.dll
            System.Console.WriteLine(string)	                                        2	                    1.79	                    1.79	                    0.70	                    0.70	                    mscorlib.dll


            Hot path shows that most of time is spent for System.Security.Cryptography.DeriveBytes.GetBytes() method.
            Since that library is 3d party (MS) it is possible to deal with that only as with "Black Box".

            It takes about ~54ms to execute that method. 

            Array.Copy method takes about 1-3 ms, so there is no sense to optimize that part.
            Same for Convert.ToBase64String() method.


            Obvious that the only thing that could be optimized is method GetBytes of class Rfc2898DeriveBytes.
            To proceed research Rfc2898DeriveBytes was downloaded and included into the project as Rfc2898DeriveBytesLocal class.

            */

            #endregion GeneratePasswordHashUsingSalt

            #region GeneratePasswordHashUsingSalt_Rfc2898DeriveBytes_Disassembled

            Run(GeneratePasswordHashUsingSalt_Rfc2898DeriveBytes_Disassembled, "GeneratePasswordHashUsingSalt_Rfc2898DeriveBytes_Disassembled", passwordText, salt);
            /*
             Summary:
            Function Name	                                                                                Number of Calls	 Elapsed Inclusive Time %	Elapsed Exclusive Time %	Avg Elapsed Inclusive Time	Avg Elapsed Exclusive Time	Module Name
            System.Security.Cryptography.HashAlgorithm.Initialize()	                                        10,000	         34.83	                    34.83	                    0.00	                    0.00	                    mscorlib.dll
            System.Security.Cryptography.HashAlgorithm.TransformFinalBlock(uint8[],int32,int32)	            10,000	         30.80	                    30.80	                    0.00	                    0.00	                    mscorlib.dll
            HW.Profiling.PassHashGenerator.Console.Program.Main(string[])	                                1	             100.00	                    12.81	                    69.76	                    8.94	                    HW.Profiling.PassHashGenerator.Console.exe
            System.Security.Cryptography.HashAlgorithm.TransformBlock(uint8[],int32,int32,uint8[],int32)	10,001	         8.16	                    8.16	                    0.00	                    0.00	                    mscorlib.dll
            System.Security.Cryptography.Rfc2898DeriveBytesLocal.Func() 	                                1	             79.38	                    3.40	                    55.37	                    2.37	                    HW.Profiling.PassHashGenerator.Console.exe
             

            Now it is possible to see that hot path goes to HashAlgorithm.Initialize() and HashAlgorithm.TransformFinalBlock methods.
            Both methods are invoked on each iteration in Func() method.
            Func() method is executed only single time. So, it should be executed at least once, and 1 is the lowest number.

            As for hot path methods, since by Task description there is not allowed to reduce iteration count, they cannot be optimized in Func method.
            
             
             */



            #endregion GeneratePasswordHashUsingSalt_Rfc2898DeriveBytes_Disassembled


            #region Reduce execution time

            /*
            The only ability to reduce execution time of Rfc2898DeriveBytes.GetBytes() is to to call for that several times with cb value less than BlockSize.
            For that case Func will be executed once internal array with size of BlockSize will be created and all other call will take time about 1 ms, until sum of requested cb bytes are less than BlockSize.
            BlockSize is 20 because internally Rfc2898DeriveBytes uses SHA1. Sha1 has 160 bit as block size which equals 20 bytes


            I don't see any optimization except this one.

            */


            #endregion Reduce execution time


        }



        public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {

            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }


        public static string GeneratePasswordHashUsingSalt_Rfc2898DeriveBytes_Disassembled(string passwordText, byte[] salt)
        {

            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytesLocal(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytesLocal(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }
        

        private static void Run(Func<string, byte[], string> gererateHash, string methodName, string passwordText, byte[] salt)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string hashedPass = gererateHash(passwordText, salt);
            sw.Stop();

            System.Console.WriteLine($"{methodName}: {sw.ElapsedMilliseconds}");
        }

        private static byte[] GetSalt(bool useConst)
        {
            byte[] saltConst = new byte[]
            {

                209,
                144,
                75,
                248,
                126,
                248,
                253,
                47,
                146,
                65,
                6,
                207,
                242,
                157,
                206,
                185,
            };

            if (useConst)
            {
                return saltConst;
            }
            byte[] saltGenerated = new byte[16];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(saltGenerated);
            }

            return saltGenerated;

        }

    }
}
