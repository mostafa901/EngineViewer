using Shared_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

using UT = Utility.IO;

using Shared_Utility.Logger;

namespace EngineViewer._3D.TCP
{
    public class Engine_Tcp
    {
        public static bool Started;
        public static int EnginePort = 120;

        public Engine_Tcp()
        {
            Task.Run(async () =>
          {
              await Task.Delay(1000); //allow some time to get the window rendered
              Started = true;

              UT.Net.SetupServer(handelconnection, "127.0.0.1", EnginePort);
          });
        }

        async private void handelconnection(TcpClient _client)
        {
            var client = _client;
            var nstream = client.GetStream();

            try
            {
                Logger.Log($"Received data: from {_client.Client.RemoteEndPoint.AddressFamily.ToString()}");
                var resp = await client.ReadfromStream2();
                if (resp.ResponseByte.Length == 0)
                {
                    client.Dispose();
                    return;
                }
                var data = File.ReadAllBytes(resp.SavedDataPath);

                int Code = BitConverter.ToInt32(resp.ResponseByte, 0);
                Logger.Log($"Sending Code: {Code}");

                try
                {
                    ResolveTcpRequest(Code, client, resp);
                }
                catch (Exception ex)
                {
                    var js = new JStruct();
                    js.JsMessage = ex.ToString();
                    client.SendByStream2(js.JSerialize().ToByteArray(Encoding.ASCII), Engine_Code.Failed);
                    Logger.Log($"Method Number not found");
                    ex.Log("Connecting from Engine", Logger.ErrorType.Warrning);
                }
            }
            catch (Exception ex)
            {
                ex.Log(ex.Message, Logger.ErrorType.Error);
            }

            client?.Close();
        }

        private void ResolveTcpRequest(int code, TcpClient client, Net.ResponseBytes resp)
        {
            switch (code)
            {
                case Engine_Code.Message:
                    {
                        var js = new JStruct().FromServer<JStruct>(resp);
                        DefaultScene.Actions.Add(() =>
                      {
                          new Urho3DNet.MessageBox(DefaultScene.scene.Context, js.JsMessage);
                          js = new JStruct();
                          js.JsMessage = "Message Sent";
                          SendRequestToClient(js);
                      });
                        break;
                    }
                case Engine_Code.DrawGeometry:
                    {
                        var js = new JStruct().FromServer<JStruct>(resp);
                        var geometryFolderPaths = "".JDeserializemyData(js.JsData);
                        var geometryFilePaths = Directory.GetFiles(geometryFolderPaths).ToList();
                        DefaultScene.Actions.Add(() =>
                        {
                            DefaultScene.Instance.DrawGeometryFromRevit(geometryFilePaths);
                        });

                        break;
                    }
                default:
                    break;
            }
        }

        private static async Task<TcpClient> CallExternalAppliation(Action<TcpClient> callback = null)
        {
            callback = callback == null ? (c) => { } : callback;
            Logger.Log("Calling External App on: 191");
            return await Net.TCP.Connect("127.0.0.1", 191, callback);
        }

        async public static void SendRequestToClient(JStruct js)
        {
            var client = await CallExternalAppliation((c) =>
            {
                c.SendByStream2(js.JSerialize().ToByteArray(Encoding.ASCII), Engine_Code.Received);
                Logger.Log($"Request Sent from Engine: {js.JsMessage}");
            });
            if (client == null)
            {
                Logger.Log($"Couldn't Connect to External App", "", Logger.ErrorType.Warrning);
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