
using TransportClient.transport.client;
namespace TransportClientFX;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var client1 = new StartRpcClient();
            
        Task.Run(() => client1.Start());
            
        Application.Run();
    }
}