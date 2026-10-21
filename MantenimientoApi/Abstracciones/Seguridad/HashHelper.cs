using System.Security.Cryptography;

namespace Abstracciones.Seguridad;

/// <summary>
/// Provee utilidades criptográficas para generar y verificar contraseñas con PBKDF2 y HMAC-SHA256.
/// </summary>
public static class HashHelper
{
    private const int Iteraciones = 100_000;
    private const int TamanoSalt = 16;
    private const int TamanoSubclave = 32;

    public static string Hashear(string password)
    {
        byte[] salt = new byte[TamanoSalt];
        RandomNumberGenerator.Fill(salt);
        return HashearConSalt(password, salt);
    }

    public static string HashearConSalt(string password, byte[] salt)
    {
        if (salt.Length != TamanoSalt)
            throw new ArgumentException($"El salt debe ser de {TamanoSalt} bytes.", nameof(salt));

        byte[] subclave = Rfc2898DeriveBytes.Pbkdf2(
            password, 
            salt, 
            Iteraciones, 
            HashAlgorithmName.SHA256, 
            TamanoSubclave);

        byte[] resultado = new byte[1 + TamanoSalt + TamanoSubclave];
        resultado[0] = 0x01; // Versión de formato
        Buffer.BlockCopy(salt, 0, resultado, 1, TamanoSalt);
        Buffer.BlockCopy(subclave, 0, resultado, 1 + TamanoSalt, TamanoSubclave);

        return Convert.ToBase64String(resultado);
    }

    public static bool Verificar(string password, string hashAlmacenado)
    {
        if (string.IsNullOrWhiteSpace(hashAlmacenado))
            return false;

        try
        {
            byte[] bytes = Convert.FromBase64String(hashAlmacenado);
            if (bytes.Length != 1 + TamanoSalt + TamanoSubclave || bytes[0] != 0x01)
                return false;

            byte[] salt = new byte[TamanoSalt];
            Buffer.BlockCopy(bytes, 1, salt, 0, TamanoSalt);

            byte[] subclaveEsperada = new byte[TamanoSubclave];
            Buffer.BlockCopy(bytes, 1 + TamanoSalt, subclaveEsperada, 0, TamanoSubclave);

            byte[] subclaveReal = Rfc2898DeriveBytes.Pbkdf2(
                password, 
                salt, 
                Iteraciones, 
                HashAlgorithmName.SHA256, 
                TamanoSubclave);

            return CryptographicOperations.FixedTimeEquals(subclaveReal, subclaveEsperada);
        }
        catch
        {
            return false;
        }
    }
}
