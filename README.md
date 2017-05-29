# HW.Profiling

## Profiling task (main)  

Для генерации хэш-пароля используется следующий метод:public string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
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

Попытайтесь не уменьшая количество итераций, которые содержаться в переменной  iterate  ускорить работу метода.
 

## Profiling 2 - task  

Напишите пример приложения, вызывающего утечки управляемой и неуправляемой памяти. Укажите способы их устранения.
