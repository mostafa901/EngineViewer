using Shared_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace EngineViewer._3D.TCP
{
    public class Engine_Tcp
    {
        public static bool Started;


        public Engine_Tcp()
        {
            Started = true;
            Task.Run(async () =>
            {
                await Task.Delay(3000); //allow some time to get the window rendered
                Net.SetupServer(handelconnection, "127.0.0.1", 120);
            });
        }
        async private void handelconnection(TcpClient _client)
        {
            var client = _client;
            var nstream = client.GetStream();

            try
            {
                Console.WriteLine($"[{DateTime.Now.ToString()}] Received data: ");
                var resp = await client.ReadfromStream2();
                if (resp.ResponseByte.Length == 0)
                {
                    client.Dispose();
                    return;
                }
                var data = File.ReadAllBytes(resp.SavedDataPath);

                int Code = BitConverter.ToInt32(resp.ResponseByte, 0);
                Console.Write($"[{DateTime.Now.ToString()}] Sending Code: {Code}");
                Console.WriteLine();

                try
                {
                    ResolveTcpRequest(Code, client, resp);
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    var js = new JStructBase();
                    js.JsMessage = ex.ToString();
                    client.SendByStream2(js.JSerialize().ToByteArray(Encoding.ASCII), Engine_Code.Failed);
                    Console.WriteLine($"[{DateTime.Now.ToString()}] Method Number not found");
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"[{DateTime.Now.ToString()}] {ex.Message}");
                Console.WriteLine();
            }

            client.Close();
        }

        private void ResolveTcpRequest(int code, TcpClient client, Net.ResponseBytes resp)
        {
            switch (code)
            {
                case Engine_Code.Message:
                    {
                        var js = JStructBase.FromServer(resp);
                        DefaultScene.Actions.Add(async () =>
                        {
                            new Urho3DNet.MessageBox(DefaultScene.scene.Context, js.JsMessage);
                            js = new JStructBase();
                            js.JsMessage = "Message Sent";
                            SendRequestToClient(js);
                        });
                        break;
                    }
                case Engine_Code.DrawGeometry:
                    {
                        var js = JStructBase.FromServer(resp);
                        var geos = new List<Serializable.Engine_Geometry>().JDeserializemyData(js.JsData);

                        DefaultScene.Actions.Add(() =>
                        {
                            DefaultScene.Instance.CreateCustomShape2(geos);
                        });

                        break;
                    }
                default:
                    break;
            }
        }

        async public static void SendRequestToClient(JStructBase js)
        {
            var client = await Net.TCP.Connect("127.0.0.1", 121, (c) =>
             {
                 c.SendByStream2(js.JSerialize().ToByteArray(Encoding.ASCII), Engine_Code.Received);
                 File.AppendAllText(@"C:\Users\mostafa90\AppData\Local\Temp\test.txt",$"[{DateTime.Now.ToLongTimeString()}]:    Request Sent from Engine\r\n");
             });
             if(client==null)
             {
                File.AppendAllText(@"C:\Users\mostafa90\AppData\Local\Temp\test.txt", $"[{DateTime.Now.ToLongTimeString()}]:    Receiver not Initialized\r\n");
            }
        }

        public static class Engine_Code
        {
            public const int Failed = 0;
            public const int Received = 1;
            public const int Message = 2;
            public const int DrawGeometry = 3;
        }
    }
}
