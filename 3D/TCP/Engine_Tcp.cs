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
    class Engine_Tcp
    {
        public Engine_Tcp()
        {

            Utility.IO.Net.SetupServer(handelconnection, "127.0.0.1", 120);
        }

        async private void handelconnection(TcpClient tcpc)
        {
            var client = (TcpClient)tcpc;
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
                    var js = new JStruct();
                    js.JsModel = ex.ToString();
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

        public class JStruct
        {
            public string JsModel { get; internal set; }
            public static JStruct FromServer(Net.ResponseBytes resp)
            {
                try
                {
                    var js = new JStruct().JDeserializemyData(Encoding.ASCII.GetString(System.IO.File.ReadAllBytes(resp.SavedDataPath)));
                    return js;
                }
                catch (Exception ex)
                {
                    ex.Log("Error Reading json from Server");
                }

                return null;
            }
        }

        private void ResolveTcpRequest(int code, TcpClient client, Net.ResponseBytes resp)
        {
            switch (code)
            {
                case Engine_Code.Message:
                    {
                        var js = JStruct.FromServer(resp);
                        DefaultScene.Actions.Add(() =>
                        {
                            new Urho3DNet.MessageBox(DefaultScene.scene.Context, js.JsModel);
                            js = new JStruct();
                            js.JsModel = "Message Sent";
                            client.SendByStream2(js.JSerialize().ToByteArray(Encoding.ASCII), Engine_Code.Received);
                        });
                        break;
                    }
                default:
                    break;
            }
        }

        public static class Engine_Code
        {
            public const int Failed = 0;
            public const int Received = 1;
            public const int Message = 2;
        }
    }
}
