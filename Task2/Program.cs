using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

const int port = 443;
string response = "y";
while (response == "y")
{
    Console.Write("Enter the server address (example: google.com): ");
    string server = Console.ReadLine();

    try
    {
        using (TcpClient client = new TcpClient(server, port))
        using (SslStream sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate))
        {
            sslStream.AuthenticateAsClient(server);

            Console.WriteLine("TLS connection established successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }

    Console.Write("Do you want to check another server? (y/n): ");
    response = Console.ReadLine().ToLower();
}

static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
{
    if (sslPolicyErrors == SslPolicyErrors.None)
    {
        Console.WriteLine("The certificate is valid.");
    }
    else
    {
        Console.WriteLine("Certificate errors: " + sslPolicyErrors);
        return false;
    }

    var x509Cert = new X509Certificate2(certificate);
    byte[] publicKeyHash = SHA256.HashData(x509Cert.GetPublicKey());

    Console.WriteLine("Public key hash (SHA-256): " + BitConverter.ToString(publicKeyHash).Replace("-", ""));

    return true;
}