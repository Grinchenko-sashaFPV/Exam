using System.Security.Cryptography;

const string inputFilePath = "../../../Files/input.txt";
const string encryptedFilePath = "../../../EncryptedFiles/encrypted.bin";

// Key & init vector (IV) for AES-GCM
byte[] key = new byte[32]; // 256-bit key
byte[] iv = new byte[12]; // 96-bit IV
RandomNumberGenerator.Fill(key);
RandomNumberGenerator.Fill(iv);

try
{
    string plaintext = File.ReadAllText(inputFilePath);
    byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);

    // Буфери для шифротексту і тегу автентифікації
    byte[] ciphertext = new byte[plaintextBytes.Length];
    byte[] tag = new byte[16];

    using (AesGcm aes = new(key))
    {
        aes.Encrypt(iv, plaintextBytes, ciphertext, tag);
    }

    WriteEncryptedFile(ciphertext);

    // Display result
    Console.WriteLine("Encryption completed.");
    Console.WriteLine("Ciphertext written to file: " + encryptedFilePath);
    Console.WriteLine("IV: " + Convert.ToBase64String(iv));
    Console.WriteLine("Key: " + Convert.ToBase64String(key));
    Console.WriteLine("Authentication tag: " + Convert.ToBase64String(tag));

    // Decryption
    Console.WriteLine("\n--- Decryption ---");
    Console.Write("Pls, provide IV (Base64): ");
    iv = Convert.FromBase64String(Console.ReadLine());

    Console.Write("Provide key (Base64): ");
    key = Convert.FromBase64String(Console.ReadLine());

    Console.Write("And authentication tag (Base64): ");
    tag = Convert.FromBase64String(Console.ReadLine());

    byte[] decryptedPlaintext = new byte[ciphertext.Length];

    using (AesGcm aes = new AesGcm(key))
    {
        aes.Decrypt(iv, ciphertext, tag, decryptedPlaintext);
    }

    string decryptedText = System.Text.Encoding.UTF8.GetString(decryptedPlaintext);
    DisplayDecryptedText(decryptedText);
}
catch (Exception ex)
{
    Console.WriteLine("Exception: " + ex.Message);
} 

void WriteEncryptedFile(byte[] ciphertext)
{
    using (FileStream fs = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
    {
        fs.Write(ciphertext);
    }
}

void DisplayDecryptedText(string text)
{
    Console.WriteLine("Decrypted text: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(text);
    Console.ForegroundColor = ConsoleColor.White;
}